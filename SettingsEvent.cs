using System;
using Build1.PostMVC.Core.MVCS.Events;

namespace Build1.PostMVC.Unity.Settings
{
    public static class SettingsEvent
    {
        public static readonly Event<SettingType, Exception> LoadResult     = new(typeof(SettingsEvent), nameof(LoadResult));
        public static readonly Event<SettingType>            Unload         = new(typeof(SettingsEvent), nameof(Unload));
        public static readonly Event<SettingType>            Reset          = new(typeof(SettingsEvent), nameof(Reset));
        public static readonly Event<Setting>                SettingChanged = new(typeof(SettingsEvent), nameof(SettingChanged));
    }
}