﻿using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.PerCampaign;

using System.IO;

using TaleWorlds.Core;

namespace MCM.Abstractions.Settings.Containers.PerCampaign
{
    public abstract class BasePerCampaignSettingsContainer : BaseSettingsContainer<PerCampaignSettings>, IPerCampaignSettingsContainer
    {
        /// <inheritdoc/>
        protected override string RootFolder { get; }

        protected BasePerCampaignSettingsContainer()
        {
            RootFolder = Path.Combine(base.RootFolder, "PerCampaign");
        }

        /// <inheritdoc/>
        protected override void RegisterSettings(PerCampaignSettings tSettings)
        {
            if (Game.Current?.PlayerTroop?.StringId == null)
                return;

            if (tSettings == null || LoadedSettings.ContainsKey(tSettings.Id))
                return;

            LoadedSettings.Add(tSettings.Id, tSettings);

            var directoryPath = Path.Combine(RootFolder, tSettings.FolderName, tSettings.SubFolder ?? string.Empty);
            if (AvailableSettingsFormats.ContainsKey(tSettings.Format))
                AvailableSettingsFormats[tSettings.Format].Load(tSettings, directoryPath, tSettings.Id);
            else
                AvailableSettingsFormats["memory"].Load(tSettings, directoryPath, tSettings.Id);
        }

        /// <inheritdoc/>
        public override bool SaveSettings(BaseSettings settings)
        {
            if (Game.Current?.PlayerTroop?.StringId == null)
                return false;

            if (!(settings is PerCampaignSettings tSettings) || !LoadedSettings.ContainsKey(tSettings.Id))
                return false;

            var directoryPath = Path.Combine(RootFolder, tSettings.FolderName, tSettings.SubFolder ?? string.Empty);
            if (AvailableSettingsFormats.ContainsKey(tSettings.Format))
                AvailableSettingsFormats[tSettings.Format].Save(tSettings, directoryPath, tSettings.Id);
            else
                AvailableSettingsFormats["memory"].Save(tSettings, directoryPath, tSettings.Id);

            return true;
        }

        /// <inheritdoc/>
        public override bool ResetSettings(BaseSettings settings)
        {
            if (Game.Current?.PlayerTroop?.StringId == null)
                return false;

            return base.ResetSettings(settings);
        }
        /// <inheritdoc/>
        public override bool OverrideSettings(BaseSettings settings)
        {
            if (Game.Current?.PlayerTroop?.StringId == null)
                return false;

            return base.OverrideSettings(settings);
        }

        /// <inheritdoc/>
        public abstract void OnGameStarted(Game game);
        /// <inheritdoc/>
        public abstract void OnGameEnded(Game game);
    }
}