namespace KwikNesta.Mediatrix.Core.Abstractions
{
    public interface IKwikMediator
    {
        Task<TResponse> SendAsync<TResponse>(IKwikRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : IKwikNotification;
    }

}
