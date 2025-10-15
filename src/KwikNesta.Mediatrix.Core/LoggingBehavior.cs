using Microsoft.Extensions.Logging;

namespace KwikNesta.Mediatrix.Core
{
    public class LoggingBehavior<TRequest, TResponse> : IKwikPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            var name = typeof(TRequest).Name;
            _logger.LogInformation("Handling {Request}", name);
            var start = DateTime.UtcNow;

            var response = await next();

            var duration = DateTime.UtcNow - start;
            _logger.LogInformation("Handled {Request} in {Duration}ms", name, duration.TotalMilliseconds);

            return response;
        }
    }
}
