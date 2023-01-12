namespace Build1.PostMVC.Unity.Settings
{
    public abstract class Setting
    {
        public readonly string key;
        public readonly object defaultValue;

        protected Setting(string key, object defaultValue)
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }

        public override string ToString()
        {
            return key;
        }
    }

    public sealed class Setting<T> : Setting
    {
        public new readonly T defaultValue;

        public Setting(string key, T defaultValue) : base(key, defaultValue)
        {
            this.defaultValue = defaultValue;
        }
    }
}