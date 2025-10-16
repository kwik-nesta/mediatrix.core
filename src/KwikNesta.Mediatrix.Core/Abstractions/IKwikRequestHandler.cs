namespace KwikNesta.Mediatrix.Core.Abstractions
{
    public interface IKwikRequestHandler<TRequest, TResponse>
        where TRequest : IKwikRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
