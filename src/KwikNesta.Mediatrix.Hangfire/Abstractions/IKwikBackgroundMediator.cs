using KwikNesta.Mediatrix.Core.Abstractions;

namespace KwikNesta.Mediatrix.Hangfire.Abstractions
{
    public interface IKwikBackgroundMediator
    {
        void Publish<TNotification>(TNotification notification) where TNotification : IKwikNotification;
        void Send<TResponse>(IKwikRequest<TResponse> request);
    }
}
