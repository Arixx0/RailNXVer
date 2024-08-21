namespace Units
{
    public interface IDestroyedEventExecutor
    {
        public void RegisterDestroyedEvent(OnDestroyedEventDelegate onDestroyedEvent);
        
        public void UnregisterDestroyedEvent(OnDestroyedEventDelegate onDestroyedEvent);
        
        public delegate void OnDestroyedEventDelegate();
    }
}