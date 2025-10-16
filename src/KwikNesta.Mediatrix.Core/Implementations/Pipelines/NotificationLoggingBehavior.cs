using KwikNesta.Mediatrix.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace KwikNesta.Mediatrix.Core.Implementations.Pipelines
{
    public class NotificationLoggingBehavior<TNotification> : IKwikNotificationBehavior<TNotification>
        where TNotification : IKwikNotification

    {
        private readonly ILogger<NotificationLoggingBehavior<TNotification>> _logger;

        public NotificationLoggingBehavior(ILogger<NotificationLoggingBehavior<TNotification>> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(TNotification notification, CancellationToken cancellationToken, Func<Task> next)
        {
            var name = typeof(TNotification).Name;
            _logger.LogInformation("Publishing {Notification}", name);
            var start = DateTime.UtcNow;

            await next();

            var duration = DateTime.UtcNow - start;
            _logger.LogInformation("Published {Notification} in {Duration}ms", name, duration.TotalMilliseconds);
        }

    }
}
