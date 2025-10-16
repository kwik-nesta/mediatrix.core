using KwikNesta.Mediatrix.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace KwikNesta.Mediatrix.Playground.Notifications
{
    public class UserRegisteredNotificationHandler : IKwikNotificationHandler<UserRegisteredNotification>
    {
        private readonly ILogger<UserRegisteredNotificationHandler> _logger;

        public UserRegisteredNotificationHandler(ILogger<UserRegisteredNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(UserRegisteredNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User registered: {UserId}, {Email}", notification.UserId, notification.Email);
            await Task.Delay(300, cancellationToken);
        }
    }
}
