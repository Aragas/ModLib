﻿using MCM.Abstractions.Settings.Models;
using MCM.Utils;

using System.Collections.Generic;

using TaleWorlds.Core;

namespace MCM.Abstractions.Settings.SettingsProvider
{
    public abstract class BaseSettingsProvider
    {
        private static BaseSettingsProvider? _instance;
        public static BaseSettingsProvider Instance =>
            _instance ??= DI.GetImplementation<BaseSettingsProvider, SettingsProviderWrapper>()!;

        public abstract IEnumerable<SettingsDefinition> CreateModSettingsDefinitions { get; }
        public abstract BaseSettings? GetSettings(string id);
        public abstract void SaveSettings(BaseSettings settings);
        public abstract void ResetSettings(BaseSettings settings);
        public abstract void OverrideSettings(BaseSettings settings);

        public abstract void OnGameStarted(Game game);
        public abstract void OnGameEnded(Game game);
    }
}