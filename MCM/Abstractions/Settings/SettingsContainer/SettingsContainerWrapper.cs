﻿using HarmonyLib;

using MCM.Abstractions.Settings.Definitions;
using MCM.Abstractions.Settings.Definitions.Wrapper;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCM.Abstractions.Settings.SettingsContainer
{
    public sealed class SettingsContainerWrapper : IMBOptionScreenSettingsContainer, IModLibSettingsContainer, IWrapper
    {
        public object Object { get; }
        private PropertyInfo? CreateModSettingsDefinitionsProperty { get; }
        private MethodInfo? GetSettingsMethod { get; }
        private MethodInfo? OverrideSettingsMethod { get; }
        private MethodInfo? RegisterSettingsMethod { get; }
        private MethodInfo? ResetSettingsMethod { get; }
        private MethodInfo? SaveSettingsMethod { get; }
        public bool IsCorrect { get; }

        public SettingsContainerWrapper(object @object)
        {
            Object = @object;
            var type = @object.GetType();
            CreateModSettingsDefinitionsProperty = AccessTools.Property(type, nameof(CreateModSettingsDefinitions));
            GetSettingsMethod = AccessTools.Method(type, nameof(GetSettings));
            OverrideSettingsMethod = AccessTools.Method(type, nameof(OverrideSettings));
            RegisterSettingsMethod = AccessTools.Method(type, nameof(RegisterSettings));
            ResetSettingsMethod = AccessTools.Method(type, nameof(ResetSettings));
            SaveSettingsMethod = AccessTools.Method(type, nameof(SaveSettings));

            IsCorrect = CreateModSettingsDefinitionsProperty != null && GetSettingsMethod != null
                && OverrideSettingsMethod != null && RegisterSettingsMethod != null
                && ResetSettingsMethod != null && SaveSettingsMethod != null;
        }

        public List<SettingsDefinition> CreateModSettingsDefinitions =>
            ((IEnumerable<object>) CreateModSettingsDefinitionsProperty?.GetValue(Object)).Select(s => new SettingsDefinitionWrapper(s)).Cast<SettingsDefinition>().ToList();
        public SettingsBase? GetSettings(string id) => GetSettingsMethod?.Invoke(Object, new object[] { id }) is { } settings
                ? settings is SettingsBase settingsBase ? settingsBase : new SettingsWrapper(settings)
                : default;
        public bool OverrideSettings(SettingsBase settings) =>
            OverrideSettingsMethod?.Invoke(Object, new object[] { settings is SettingsWrapper wrapper ? wrapper : settings }) as bool? ?? false;
        public bool RegisterSettings(SettingsBase settings) =>
            RegisterSettingsMethod?.Invoke(Object, new object[] { settings is SettingsWrapper wrapper ? wrapper : settings }) as bool? ?? false;
        public SettingsBase? ResetSettings(string id) => ResetSettingsMethod?.Invoke(Object, new object[] { id }) is { } settings
                ? settings is SettingsBase settingsBase ? settingsBase : new SettingsWrapper(settings)
                : default;
        public void SaveSettings(SettingsBase settings) =>
            SaveSettingsMethod?.Invoke(Object, new object[] { settings is SettingsWrapper wrapper ? wrapper : settings });
    }
}