namespace Build1.PostMVC.Unity.Settings
{
    public abstract class Setting
    {
        public readonly SettingType type;
        public readonly string      key;
        public          object      DefaultValue { get; private set; }

        protected Setting(SettingType type, string key, object defaultValue)
        {
            this.type = type;
            this.key = key;
            DefaultValue = defaultValue;
        }

        public void SetDefaultValue(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            return key;
        }
    }

    public abstract class Setting<T> : Setting where T : struct
    {
        public new T DefaultValue { get; private set; }

        protected Setting(SettingType type, string key, T defaultValue) : base(type, key, defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public void SetDefaultValue(T defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }

    public sealed class DeviceSetting<T> : Setting<T> where T : struct
    {
        public DeviceSetting(string key, T defaultValue) : base(SettingType.Device, key, defaultValue)
        {
        }
    }
    
    public sealed class UserSetting<T> : Setting<T> where T : struct
    {
        public UserSetting(string key, T defaultValue) : base(SettingType.User, key, defaultValue)
        {
        }
    }
}