using System.Collections.Generic;

namespace Build1.PostMVC.Unity.Settings
{
    public interface ISettingsController
    {
        bool   Initialized          { get; }
        bool   DeviceSettingsLoaded { get; }
        bool   UserSettingsLoaded   { get; }
        string UserId               { get; }

        void Initialize(IEnumerable<Setting> settings);

        void LoadDeviceSettings();
        void LoadUserSettings(string userId);
        void UnloadUserSettings();

        bool CheckSettingSet<T>(Setting<T> setting) where T : struct;
        T    Get<T>(Setting<T> setting) where T : struct;
        void Set<T>(Setting<T> setting, T value) where T : struct;

        void Reset(SettingType type);
        void Save(SettingType type);
    }
}