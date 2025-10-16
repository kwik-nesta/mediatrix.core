namespace KwikNesta.Mediatrix.Core.Abstractions
{
    public interface IKwikNotificationHandler<TNotification>
        where TNotification : IKwikNotification
    {
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
    }

}
