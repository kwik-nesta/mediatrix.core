namespace KwikNesta.Mediatrix.Core
{
    public interface IKwikNotificationHandler<TNotification>
        where TNotification : IKwikNotification
    {
        Task HandleAsyncs(TNotification notification, CancellationToken cancellationToken);
    }

}
