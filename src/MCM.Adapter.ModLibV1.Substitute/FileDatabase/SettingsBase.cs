﻿using ModLib.Interfaces;

namespace ModLib
{
    public abstract class SettingsBase : ISerialisableFile, ISubFolder
    {
        public abstract string ID { get; set; }
        public abstract string ModuleFolderName { get; }
        public abstract string ModName { get; }
        public virtual string SubFolder => string.Empty;
    }
}