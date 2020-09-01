﻿using Bannerlord.ButterLib.Common.Helpers;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;

using HarmonyLib;

using MCM.UI.GUI.ViewModels;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace MCM.UI.UIExtenderEx
{
    [ViewModelMixin]
    internal sealed class OptionsVMMixin : BaseViewModelMixin<OptionsVM>
    {
        private delegate void ExecuteDoneDelegate(OptionsVM instance);
        private delegate void ExecuteCancelDelegate(OptionsVM instance);

        private static readonly ExecuteDoneDelegate? ExecuteDoneMethod = AccessTools2.GetDelegate<ExecuteDoneDelegate>(typeof(OptionsVM), "ExecuteDone");
        private static readonly ExecuteCancelDelegate? ExecuteCancelMethod = AccessTools2.GetDelegate<ExecuteCancelDelegate>(typeof(OptionsVM), "ExecuteCancel");

        private static readonly AccessTools.FieldRef<ViewModel, Dictionary<string, PropertyInfo>> PropertyInfos =
            AccessTools.FieldRefAccess<ViewModel, Dictionary<string, PropertyInfo>>("_propertyInfos");

        static OptionsVMMixin()
        {
            var harmony = new Harmony("bannerlord.mcm.ui.optionsvm");

            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.VideoOptions)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));
            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.AudioOptions)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));
            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.GameKeyOptionGroups)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));
            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.GamepadOptions)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));
            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.GameplayOptions)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));
            harmony.Patch(
                AccessTools.Property(typeof(OptionsVM), nameof(OptionsVM.GraphicsOptions)).GetMethod,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(OptionsPostfix)), 300));

            harmony.CreateReversePatcher(
                AccessTools.Method(typeof(OptionsVM), nameof(OptionsVM.ExecuteCloseOptions)),
                new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => OriginalExecuteCloseOptions(null!)))).Patch();

            harmony.Patch(
                AccessTools.Method(typeof(OptionsVM), nameof(OptionsVM.ExecuteCloseOptions)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsVMMixin), nameof(ExecuteCloseOptionsPostfix)), 300));
        }

        private static void OptionsPostfix(OptionsVM __instance)
        {
            __instance.SetPropertyValue(nameof(ModOptionsSelected), false);
        }
        private static void ExecuteCloseOptionsPostfix(OptionsVM __instance)
        {
            if (__instance.GetPropertyValue("MCMMixin") is WeakReference<OptionsVMMixin> weakReference && weakReference.TryGetTarget(out var mixin))
            {
                mixin?.ExecuteCloseOptions();
            }
        }
        
        private static void OriginalExecuteCloseOptions(OptionsVM instance) => throw new NotImplementedException("It's a stub");

        private readonly ModOptionsVM _modOptions = new ModOptionsVM();
        private bool _modOptionsSelected;
        private int _descriptionWidth = 650;

        [DataSourceProperty]
        public WeakReference<OptionsVMMixin> MCMMixin => new WeakReference<OptionsVMMixin>(this);

        [DataSourceProperty]
        public ModOptionsVM ModOptions
        {
            get
            {
                ModOptionsSelected = true;
                return _modOptions;
            }
        }

        [DataSourceProperty]
        public int DescriptionWidth
        {
            get => _descriptionWidth;
            private set
            {
                if (_descriptionWidth == value)
                    return;

                _descriptionWidth = value;
                ViewModel?.OnPropertyChanged(nameof(DescriptionWidth));
            }
        }

        [DataSourceProperty]
        public bool ModOptionsSelected
        {
            get => _modOptionsSelected;
            set
            {
                if (_modOptionsSelected == value)
                    return;

                _modOptionsSelected = value;
                DescriptionWidth = ModOptionsSelected ? 0 : 650;
            }
        }

        public OptionsVMMixin(OptionsVM vm) : base(vm)
        {
            vm.PropertyChanged += OptionsVM_PropertyChanged;
        }

        private void OptionsVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _modOptions.OnPropertyChanged(e.PropertyName);
        }

        public override void OnFinalize()
        {
            if (ViewModel != null)
                ViewModel.PropertyChanged -= OptionsVM_PropertyChanged;

            base.OnFinalize();
        }

        [DataSourceMethod]
        public void ExecuteCloseOptions()
        {
            ModOptions.ExecuteCancelInternal(false);
            if (ViewModel != null)
                OriginalExecuteCloseOptions(ViewModel);
            OnFinalize();
        }

        [DataSourceMethod]
        public void ExecuteDone()
        {
            ModOptions.ExecuteDoneInternal(false, () =>
            {
                if (ViewModel != null)
                    ExecuteDoneMethod?.Invoke(ViewModel);
            });
        }

        [DataSourceMethod]
        public void ExecuteCancel()
        {
            ModOptions.ExecuteCancelInternal(false, () =>
            {
                if (ViewModel != null)
                    ExecuteCancelMethod?.Invoke(ViewModel);
            });
        }
    }
}