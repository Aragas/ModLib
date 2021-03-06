﻿using Bannerlord.BUTR.Shared.Helpers;

using HarmonyLib.BUTR.Extensions;

using System;

using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace MCM.UI.Utils
{
    internal static class EscapeMenuItemVMUtils
    {
        private delegate EscapeMenuItemVM V1Delegate(TextObject item, Action<object> onExecute, object identifier, bool isDisabled, bool isPositiveBehavioured = false);
        private delegate EscapeMenuItemVM V2Delegate(TextObject item, Action<object> onExecute, object identifier, Tuple<bool, TextObject> isDisabledAndReason, bool isPositiveBehavioured = false);

        private static readonly V1Delegate? V1;
        private static readonly V2Delegate? V2;

        static EscapeMenuItemVMUtils()
        {
            foreach (var constructorInfo in HarmonyLib.AccessTools.GetDeclaredConstructors(typeof(EscapeMenuItemVM), false))
            {
                var @params = constructorInfo.GetParameters();
                if (@params.Length >= 5 && @params[4].ParameterType == typeof(bool))
                    V1 = AccessTools2.GetDelegate<V1Delegate>(constructorInfo);
                if (@params.Length >= 5 && @params[4].ParameterType == typeof(Tuple<bool, TextObject>))
                    V2 = AccessTools2.GetDelegate<V2Delegate>(constructorInfo);
            }
        }

        public static EscapeMenuItemVM? Create(TextObject item, Action<object> onExecute, object identifier, bool isDisabled, bool isPositiveBehavioured = false)
        {
            if (V1 is not null)
                return V1(item, onExecute, identifier, isDisabled, isPositiveBehavioured);
            if (V2 is not null)
                return V2(item, onExecute, identifier, new Tuple<bool, TextObject>(isDisabled, TextObjectHelper.Create(string.Empty)!), isPositiveBehavioured);
            return null;
        }
    }
}