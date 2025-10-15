namespace KwikNesta.Mediatrix.Core
{
    public interface IKwikPipelineBehavior<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request,
            CancellationToken cancellationToken,
            Func<Task<TResponse>> next);
    }

}
