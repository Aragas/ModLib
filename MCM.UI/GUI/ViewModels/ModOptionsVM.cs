﻿using MCM.Abstractions.Settings.SettingsProvider;
using MCM.UI.ExtensionMethods;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MCM.UI.GUI.ViewModels
{
    internal class ModOptionsVM : ViewModel
    {
        private string _titleLabel = "";
        private string _cancelButtonText = "";
        private string _doneButtonText = "";
        private SettingsVM? _selectedMod;
        private MBBindingList<SettingsVM> _modSettingsList = new MBBindingList<SettingsVM>();
        private string _hintText = "";
        private string _searchText = "";

        [DataSourceProperty]
        public string Name
        {
            get => _titleLabel;
            set
            {
                _titleLabel = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        [DataSourceProperty]
        public bool ChangesMade => ModSettingsList.Any(x => x.URS.ChangesMade());
        [DataSourceProperty]
        public string DoneButtonText
        {
            get => _doneButtonText;
            set
            {
                _doneButtonText = value;
                OnPropertyChanged(nameof(DoneButtonText));
            }
        }
        [DataSourceProperty]
        public string CancelButtonText
        {
            get => _cancelButtonText; set
            {
                _cancelButtonText = value;
                OnPropertyChanged(nameof(CancelButtonText));
            }
        }
        [DataSourceProperty]
        public MBBindingList<SettingsVM> ModSettingsList
        {
            get => _modSettingsList;
            set
            {
                if (_modSettingsList != value)
                {
                    _modSettingsList = value;
                    OnPropertyChanged(nameof(ModSettingsList));
                }
            }
        }
        [DataSourceProperty]
        public SettingsVM? SelectedMod
        {
            get => _selectedMod;
            set
            {
                if (_selectedMod != value)
                {
                    _selectedMod = value;
                    OnPropertyChanged(nameof(SelectedMod));
                    OnPropertyChanged(nameof(SelectedDisplayName));
                    OnPropertyChanged(nameof(SomethingSelected));
                }
            }
        }
        [DataSourceProperty]
        public string SelectedDisplayName => SelectedMod == null ? "Mod Name not Specified" : SelectedMod.DisplayName;
        [DataSourceProperty]
        public bool SomethingSelected => SelectedMod != null;
        [DataSourceProperty]
        public string HintText
        {
            get => _hintText;
            set
            {
                if (_hintText != value)
                {
                    _hintText = value;
                    OnPropertyChanged(nameof(HintText));
                    OnPropertyChanged(nameof(IsHintVisible));
                }
            }
        }
        [DataSourceProperty]
        public bool IsHintVisible => !string.IsNullOrWhiteSpace(HintText);
        [DataSourceProperty]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    if (SelectedMod?.SettingPropertyGroups.Count > 0)
                    {
                        foreach (var group in SelectedMod.SettingPropertyGroups)
                            group.NotifySearchChanged();
                    }
                }
            }
        }

        public ModOptionsVM()
        {
            Name = new TextObject("{=XiGPhfsm}Mod Options", null).ToString();
            DoneButtonText = new TextObject("{=WiNRdfsm}Done", null).ToString();
            CancelButtonText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
            SearchText = "";


            ModSettingsList = new MBBindingList<SettingsVM>();

            // Build the options in a separate context if possible
            new TaskFactory().StartNew(syncContext =>
            {
                if (!(syncContext is SynchronizationContext uiContext))
                    return;

                foreach (var viewModel in BaseSettingsProvider.Instance.CreateModSettingsDefinitions.Select(s => new SettingsVM(s, this)))
                {
                    uiContext.Send(o =>
                    {
                        if (!(o is SettingsVM vm))
                            return;

                        vm.AddSelectCommand(ExecuteSelect);
                        ModSettingsList.Add(vm);
                        vm.RefreshValues();
                    }, viewModel);
                }
            }, SynchronizationContext.Current);

            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            foreach (var viewModel in ModSettingsList)
                viewModel.RefreshValues();

            OnPropertyChanged(nameof(SelectedMod));
        }

        public void ExecuteClose()
        {
            foreach (var viewModel in ModSettingsList)
            {
                viewModel.URS.UndoAll();
                viewModel.URS.ClearStack();
            }
        }

        public bool ExecuteCancel() => ExecuteCancelInternal(true);
        public bool ExecuteCancelInternal(bool popScreen, Action? onClose = null)
        {
            OnFinalize();
            if (popScreen) ScreenManager.PopScreen();
            else onClose?.Invoke();
            foreach (var viewModel in ModSettingsList)
            {
                viewModel.URS.UndoAll();
                viewModel.URS.ClearStack();
            }
            return true;
        }

        public void ExecuteDone() => ExecuteDoneInternal(true);
        public void ExecuteDoneInternal(bool popScreen, Action? onClose = null)
        {
            if (!ModSettingsList.Any(x => x.URS.ChangesMade()))
            {
                OnFinalize();
                if (popScreen) ScreenManager.PopScreen();
                else onClose?.Invoke();
                return;
            }

            //Save the changes to file.
            var changedModSettings = ModSettingsList.Where(x => x.URS.ChangesMade()).ToList();

            var requireRestart = ModSettingsList.Any(x => x.RestartRequired());
            if (requireRestart)
            {
                InformationManager.ShowInquiry(new InquiryData("Game Needs to Restart",
                    "The game needs to be restarted to apply mod settings changes. Do you want to close the game now?",
                    true, true, "Yes", "No",
                    () =>
                    {
                        changedModSettings
                            .Do(x => BaseSettingsProvider.Instance.SaveSettings(x.SettingsInstance))
                            .Do(x => x.URS.ClearStack())
                            .ToList();

                        OnFinalize();
                        onClose?.Invoke();
                        Utilities.QuitGame();
                    }, () => { }));
            }
            else
            {
                changedModSettings
                    .Do(x => BaseSettingsProvider.Instance.SaveSettings(x.SettingsInstance))
                    .Do(x => x.URS.ClearStack())
                    .ToList();

                OnFinalize();
                if (popScreen) ScreenManager.PopScreen();
                else onClose?.Invoke();
            }
        }

        public void ExecuteRevert()
        {
            if (SelectedMod != null)
            {
                InformationManager.ShowInquiry(new InquiryData("Revert mod settings to defaults",
                    $"Are you sure you wish to revert all settings for {SelectedMod.DisplayName} to their default values?",
                    true, true, "Yes", "No",
                    () =>
                    {
                        SelectedMod.ResetSettings();
                        var selectedMod = SelectedMod;
                        ExecuteSelect(null);
                        ExecuteSelect(selectedMod);
                    }, null));
            }
        }

        public void ExecuteSelect(SettingsVM? viewModel)
        {
            if (SelectedMod != viewModel)
            {
                if (SelectedMod != null)
                    SelectedMod.IsSelected = false;

                SelectedMod = viewModel;

                if (SelectedMod != null)
                {
                    SelectedMod.IsSelected = true;
                }
            }
        }


        public override void OnFinalize()
        {
            foreach (var modSettings in ModSettingsList)
                modSettings.OnFinalize();

            base.OnFinalize();
        }
    }
}