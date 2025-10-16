namespace KwikNesta.Mediatrix.Core.Abstractions
{
    public interface IKwikNotificationBehavior<TNotification>
       where TNotification : IKwikNotification
    {
        Task HandleAsync(TNotification notification,
            CancellationToken cancellationToken,
            Func<Task> next);
    }
}
