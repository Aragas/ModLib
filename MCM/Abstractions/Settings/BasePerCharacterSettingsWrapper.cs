﻿using MCM.Utils;

using System.Linq;

namespace MCM.Abstractions.Settings
{
    public abstract class BasePerCharacterSettingsWrapper : PerCharacterSettings, IWrapper
    {
        public static BasePerCharacterSettingsWrapper Create(object @object) =>
            DI.GetImplementations<BasePerCharacterSettingsWrapper, PerCharacterSettingsWrapper>(ApplicationVersionUtils.GameVersion(), @object)
                .FirstOrDefault(w => w.IsCorrect);

        public object Object { get; protected set; }

        public bool IsCorrect { get; protected set; }

        protected BasePerCharacterSettingsWrapper(object @object)
        {
            Object = @object;
        }
    }
}