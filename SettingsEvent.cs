using System;
using Build1.PostMVC.Core.MVCS.Events;

namespace Build1.PostMVC.Unity.Settings
{
    public static class SettingsEvent
    {
        public static readonly Event            LoadSuccess = new();
        public static readonly Event<Exception> LoadFail    = new();

        public static readonly Event            UnloadSuccess = new();
        public static readonly Event<Exception> UnloadFail    = new();

        public static readonly Event<Setting> SettingChanged = new();

        public static readonly Event<Exception> SaveFail = new();

        public static readonly Event Reset = new();
    }
}