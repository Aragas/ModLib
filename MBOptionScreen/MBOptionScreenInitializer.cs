﻿using MBOptionScreen.ApplicationContainer;
using MBOptionScreen.Attributes;
using MBOptionScreen.Functionality;
using MBOptionScreen.Legacy.v1;
using MBOptionScreen.ResourceInjection;
using MBOptionScreen.ResourceInjection.EmbedLoaders;
using MBOptionScreen.Settings;
using MBOptionScreen.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MBOptionScreen
{
    [Version("e1.0.0",  200)]
    [Version("e1.0.1",  200)]
    [Version("e1.0.2",  200)]
    [Version("e1.0.3",  200)]
    [Version("e1.0.4",  200)]
    [Version("e1.0.5",  200)]
    [Version("e1.0.6",  200)]
    [Version("e1.0.7",  200)]
    [Version("e1.0.8",  200)]
    [Version("e1.0.9",  200)]
    [Version("e1.0.10", 200)]
    [Version("e1.0.11", 200)]
    [Version("e1.1.0",  200)]
    [Version("e1.2.0",  200)]
    public sealed class MBOptionScreenInitializer : IMBOptionScreenInitializer
    {
        protected ApplicationVersion GameVerion { get; private set; }
        protected IApplicationContainerProvider ApplicationContainerProvider { get; private set; }
        private StateProviderV1 StateProviderV1 { get; } = new StateProviderV1();

        public void StartInitialization(ApplicationVersion gameVerion, bool first)
        {
            GameVerion = gameVerion;

            ApplicationContainerProvider = ReflectionUtils.GetImplementation<IApplicationContainerProvider, ApplicationContainerProviderWrapper>(GameVerion);
            if (first)
            {
                var settingsProvider = ReflectionUtils.GetImplementation<IMBOptionScreenSettingsProvider, SettingsProviderWrapper>(GameVerion);
                var modLibSettingsProvider = ReflectionUtils.GetImplementation<IModLibSettingsProvider, SettingsProviderWrapper>(GameVerion);
                ApplicationContainerProvider.Set("MBOptionScreenSettingsProvider", settingsProvider);
                ApplicationContainerProvider.Set("ModLibSettingsProvider", modLibSettingsProvider);

                Resolver.GameMenuScreenHandler.AddScreen(
                    "ModOptionsMenu_MBOptionScreen_v2",
                    9990,
                    () => (ScreenBase) ReflectionUtils.GetImplementation(GameVerion, typeof(MBOptionScreen).FullName),
                    new TextObject("{=HiZbHGvYG}Mod Options"));
            }

            // Settings providers are the only classes that need 'true' global access between MBOptionMods
            // get the global settings provider
            SettingsDatabase.MBOptionScreenSettingsProvider = new SettingsProviderWrapper(ApplicationContainerProvider.Get("MBOptionScreenSettingsProvider"));
            SettingsDatabase.ModLibSettingsProvider = new SettingsProviderWrapper(ApplicationContainerProvider.Get("ModLibSettingsProvider"));

            if (first)
            {
                PreventV1Loading();

                RegisterSettings();
            }
        }
        public void EndInitialization(bool first)
        {
            if (first)
            {
                Resolver.IngameMenuScreenHandler.AddScreen(
                    1,
                    () => (ScreenBase) ReflectionUtils.GetImplementation(GameVerion, typeof(MBOptionScreen).FullName),
                    new TextObject("{=NqarFr4P}Mod Options", null));

                if (MBOptionScreenSettings.Instance!.OverrideModLib)
                    Resolver.ModLibScreenOverrider.OverrideModLibScreen();

                BrushLoaderv1b.Inject(BaseResourceInjector.Instance);
                PrefabsLoaderv1b.Inject(BaseResourceInjector.Instance);

                ApplicationContainerProvider.Clear();
                StateProviderV1?.Clear();
            }
        }

        /// <summary>
        /// Find all implementations of SettingBase and register them
        /// </summary>
        private void RegisterSettings()
        {
            var settings = new List<SettingsBase>();
            var allTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a => !a.FullName.StartsWith("System"))
                .Where(a => !a.FullName.StartsWith("Microsoft"))
                .Where(a => !a.FullName.StartsWith("mscorlib"))
                .Where(a => Path.GetFileName(a.Location) != "MBOptionScreen.dll")
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();

            // ModLib
            var modLibSettings = allTypes
                .Where(t => ReflectionUtils.ImplementsOrImplementsEquivalent(t, "ModLib.SettingsBase"))
                .Select(obj => new ModLibSettingsWrapper(Activator.CreateInstance(obj)));
            settings.AddRange(modLibSettings);
            if (settings.Count > 0)
                ModLibStub.LoadIfNeeded();

            var mbOptionScreenSettings = allTypes
                .Where(t => ReflectionUtils.ImplementsOrImplementsEquivalent(t, "MBOptionScreen.Settings.SettingsBase"));
            var thisSettings = mbOptionScreenSettings
                .Where(t => ReflectionUtils.Implements(t, typeof(SettingsBase)))
                .Where(t => t != typeof(StubSettings))
                .Where(t => t != typeof(SettingsWrapper))
                .Where(t => t != typeof(ModLibSettingsWrapper))
#if !DEBUG
                .Where(t => t != typeof(TestSettingsV1))
                .Where(t => t != typeof(TestSettingsV2))
#endif
                .Select(obj => (SettingsBase) Activator.CreateInstance(obj));
            settings.AddRange(thisSettings);
            var externalSettings = mbOptionScreenSettings
                .Where(t => !ReflectionUtils.Implements(t, typeof(SettingsBase)))
#if !DEBUG
                .Where(t => t.FullName != "MBOptionScreen.TestSettings")
                .Where(t => t.FullName != "MBOptionScreen.TestSettingsV1")
                .Where(t => t.FullName != "MBOptionScreen.TestSettingsV2")
#endif
                .Select(obj => new SettingsWrapper(Activator.CreateInstance(obj)));
            settings.AddRange(externalSettings);

            foreach (var setting in settings)
                SettingsDatabase.RegisterSettings(setting);
        }


        private void PreventV1Loading()
        {
            var sharedStateObject = StateProviderV1.Get("MBOptionScreen.State.SharedStateObject");

            // Was not loaded
            // Replace SettingsProvider with our own implementation
            if (sharedStateObject == null)
            {
                StateProviderV1.CreateNewSharedState(SettingsDatabase.MBOptionScreenSettingsProvider);
            }
            // Was already loaded
            // Set SettingsProvider with our own implementation
            // Set HasInitialized to true to prevent EndInitialization()
            else
            {
                sharedStateObject.SetSettings(SettingsDatabase.MBOptionScreenSettingsProvider);
                sharedStateObject.SetInitialized();
                Resolver.GameMenuScreenHandler.RemoveScreen("ModOptionsMenu_MBOptionScreen");
            }
        }
    }
}