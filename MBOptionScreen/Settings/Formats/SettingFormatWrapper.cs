﻿using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBOptionScreen.Settings
{
    internal sealed class SettingFormatWrapper : ISettingsFormat
    {
        private readonly object _object;
        private PropertyInfo ProvidersProperty { get; }
        private MethodInfo LoadMethod { get; }
        private MethodInfo SaveMethod { get; }

        public IEnumerable<string> Providers => ProvidersProperty?.GetValue(_object) as IEnumerable<string> ?? Array.Empty<string>();

        public SettingFormatWrapper(object @object)
        {
            _object = @object;
            var type = @object.GetType();

            ProvidersProperty = AccessTools.Property(type, "Providers");
            LoadMethod = AccessTools.Method(type, "Load");
            SaveMethod = AccessTools.Method(type, "Save");
        }

        public SettingsBase? Load(SettingsBase settings, string path) => LoadMethod?.Invoke(_object, new object[] { settings, path }) as SettingsBase;
        public bool Save(SettingsBase settings, string path) => SaveMethod?.Invoke(_object, new object[] { settings, path }) as bool? ?? false;
    }
}