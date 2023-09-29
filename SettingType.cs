using System;

namespace Build1.PostMVC.Unity.Settings
{
    [Flags]
    public enum SettingType
    {
        Device = 1 << 0,
        User   = 1 << 1,
        
        All = Device | User
    }
}