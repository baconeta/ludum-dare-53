namespace UI.StateSwitcher
{
    /// <summary>
    /// Describes what has to be changed and how
    /// </summary>
    /// <typeparam name="TData"> Data needed to apply the changes </typeparam>
    /// <typeparam name="TTarget"> Target object to modify </typeparam>
    public abstract class StateBase<TData, TTarget> where TData : IStateData
    {
        public abstract void Apply(TData value, TTarget target);
    }
}