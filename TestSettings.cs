﻿using MBOptionScreen.Attributes;
using MBOptionScreen.Settings;

using System.Xml.Serialization;

namespace MBOptionScreen
{
    internal class TestSettings : AttributeSettings<TestSettings>
    {
        public override string ModName => "Testing";
        public override string ModuleFolderName => "Testing";
        public const string SettingsInstanceID = "Testing";

        [XmlElement]
        public override string Id { get; set; } = SettingsInstanceID;
        [XmlElement]
        [SettingProperty("Enable Crash Error Reporting", "When enabled, shows a message box showing the cause of a crash.")]
        [SettingPropertyGroup("Debugging")]
        public bool DebugMode { get; set; } = true;

        [XmlElement]
        [SettingProperty("Test Property 1", "")]
        [SettingPropertyGroup("Debugging/Test Group", true)]
        public bool TestProperty1 { get; set; } = false;

        [XmlElement]
        [SettingProperty("Test Property 5", "")]
        [SettingPropertyGroup("Debugging/Test Group")]
        public bool TestProperty5 { get; set; } = false;

        [XmlElement]
        [SettingProperty("Test Property 2", "")]
        [SettingPropertyGroup("Debugging/Test Group/Test Group 2", true)]
        public bool TestProperty2 { get; set; } = false;

        [XmlElement]
        [SettingProperty("Test Property 4", 0f, 0.5f, 0f, 100f, "")]
        [SettingPropertyGroup("Debugging/Test Group/Test Group 2")]
        public float TestProperty4 { get; set; } = 0.2f;

        [XmlElement]
        [SettingProperty("Test Property 3", 0, 10, "")]
        [SettingPropertyGroup("Debugging/Test Group/Test Group 3")]
        public int TestProperty3 { get; set; } = 2;

    }
}