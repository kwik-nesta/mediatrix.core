namespace KwikNesta.Mediatrix.Core
{
    public interface IKwikRequestHandler<TRequest, TResponse>
        where TRequest : IKwikRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
