namespace Name.Bayfaderix.Darxxemiyur.General
{
    public sealed class ExternalOnFinalization
    {
        public Action? @Delegate
        {
            get; set;
        }
    }

    public sealed class ExternalOnFinalization<T>
    {
        public Action<T>? @Delegate
        {
            get; set;
        }
    }
}