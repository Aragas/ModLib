﻿using HarmonyLib;

using MCM.Abstractions.Settings.Definitions;
using MCM.Abstractions.Settings.Definitions.Wrapper;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCM.Abstractions.Settings.SettingsProvider
{
    public sealed class SettingsProviderWrapper : BaseSettingsProvider, IWrapper
    {
        public object Object { get; }
        private PropertyInfo? CreateModSettingsDefinitionsProperty { get; }
        private MethodInfo? GetSettingsMethod { get; }
        private MethodInfo? SaveSettingsMethod { get; }
        private MethodInfo? ResetSettingsMethod { get; }
        private MethodInfo? OverrideSettingsMethod { get; }
        public bool IsCorrect { get; }

        public SettingsProviderWrapper(object @object)
        {
            Object = @object;
            var type = @object.GetType();

            CreateModSettingsDefinitionsProperty = AccessTools.Property(type, nameof(CreateModSettingsDefinitions));
            GetSettingsMethod = AccessTools.Method(type, nameof(GetSettings));
            SaveSettingsMethod = AccessTools.Method(type, nameof(SaveSettings));
            ResetSettingsMethod = AccessTools.Method(type, nameof(ResetSettings));
            OverrideSettingsMethod = AccessTools.Method(type, nameof(OverrideSettings));

            IsCorrect = CreateModSettingsDefinitionsProperty != null &&
                        GetSettingsMethod != null && SaveSettingsMethod != null &&
                        ResetSettingsMethod != null && OverrideSettingsMethod != null;
        }

        public override IEnumerable<SettingsDefinition> CreateModSettingsDefinitions =>
            ((IEnumerable<object>) (CreateModSettingsDefinitionsProperty?.GetValue(Object) ?? new List<object>()))
            .Select(s => new SettingsDefinitionWrapper(s));
        public override BaseSettings? GetSettings(string id) => 
            GetSettingsMethod?.Invoke(Object, new object[] { id }) as BaseSettings;
        public override void SaveSettings(BaseSettings settings) =>
            SaveSettingsMethod?.Invoke(Object, new object[] { settings });
        public override void ResetSettings(BaseSettings settings) =>
            ResetSettingsMethod?.Invoke(Object, new object[] { settings });
        public override void OverrideSettings(BaseSettings settings) =>
            OverrideSettingsMethod?.Invoke(Object, new object[] { settings });
    }
}