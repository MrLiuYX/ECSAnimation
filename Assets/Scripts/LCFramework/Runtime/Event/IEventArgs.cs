namespace Native.Event
{
    public interface IEventArgs : IReference
    {
        static long EventId { get; }
    }
}
