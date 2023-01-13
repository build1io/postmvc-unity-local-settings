using System.Collections.Generic;

namespace Build1.PostMVC.Unity.Settings
{
    public interface ISettingsController
    {
        IReadOnlyList<Setting> ExistingSettings { get; }
        bool                   IsLoaded         { get; }

        void SetUserId(string userId);
        
        void Load(IReadOnlyList<Setting> existingSettings);
        void Load(IReadOnlyList<Setting> existingSettings, string userId);
        void Unload();
        void Save(bool force);
        void Reset();
        
        T    GetSetting<T>(Setting<T> setting) where T : struct;
        void SetSetting<T>(Setting<T> setting, T value) where T : struct;
    }
}