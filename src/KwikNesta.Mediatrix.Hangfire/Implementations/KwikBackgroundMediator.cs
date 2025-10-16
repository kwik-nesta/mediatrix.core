using Hangfire;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Hangfire.Abstractions;
using Microsoft.Extensions.Logging;

namespace KwikNesta.Mediatrix.Hangfire.Implementations
{
    public class KwikBackgroundMediator : IKwikBackgroundMediator
    {
        private readonly IKwikMediator _kwikMediator;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<KwikBackgroundMediator> _logger;

        public KwikBackgroundMediator(IKwikMediator kwikMediator,
                                      IBackgroundJobClient backgroundJobClient, 
                                      ILogger<KwikBackgroundMediator> logger)
        {
            _kwikMediator = kwikMediator;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public void Send<TResponse>(IKwikRequest<TResponse> request)
        {
            try
            {
                _logger.LogInformation("Enqueuing background Send for {Request}", request.GetType().Name);
                _backgroundJobClient.Enqueue<IKwikMediator>(m => m.SendAsync(request, default));
                _logger.LogInformation("Enqueued background job for {Type}", request.GetType().Name);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue background job for {Type}", request.GetType().Name);
                throw;
            }
        }

        public void Publish<TNotification>(TNotification notification)
            where TNotification : IKwikNotification
        {
            try
            {
                _logger.LogInformation("Enqueuing background Publish for {Notification}", notification.GetType().Name);
                _backgroundJobClient.Enqueue<IKwikMediator>(m => m.PublishAsync(notification, default));
                _logger.LogInformation("Enqueued background Publish for {Notification}", notification.GetType().Name);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue background job for {Type}", notification.GetType().Name);
                throw;
            }
        }

    }
}
