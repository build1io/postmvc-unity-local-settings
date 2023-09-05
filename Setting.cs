namespace Build1.PostMVC.Unity.Settings
{
    public abstract class Setting
    {
        public readonly string key;
        public          object DefaultValue { get; private set; }

        protected Setting(string key, object defaultValue)
        {
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

    public sealed class Setting<T> : Setting where T : struct
    {
        public new T DefaultValue { get; private set; }

        public Setting(string key, T defaultValue) : base(key, defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public void SetDefaultValue(T defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}