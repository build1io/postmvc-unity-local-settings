using System.Collections.Generic;

namespace Build1.PostMVC.UnityLocalSettings
{
    public interface ISettingsController
    {
        bool IsLoaded { get; }
        
        void SetSettingsSet(IReadOnlyList<Setting> settings);
        void SetSettingsFolder(string folder);
        
        void Load(IReadOnlyList<Setting> settings, string folder);
        void Load(IReadOnlyList<Setting> settings);
        void Load();

        void Unload();
        
        void Save(bool force);
        
        T    GetSetting<T>(Setting<T> setting) where T : struct;
        void SetSetting<T>(Setting<T> setting, T value) where T : struct;

        void Reset();
    }
}