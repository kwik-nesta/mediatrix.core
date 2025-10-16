using KwikNesta.Mediatrix.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace KwikNesta.Mediatrix.Playground.Commands
{
    public class WelcomeEmailCommandHandler : IKwikRequestHandler<WelcomeEmailCommand, string>
    {
        private readonly ILogger<WelcomeEmailCommandHandler> _logger;

        public WelcomeEmailCommandHandler(ILogger<WelcomeEmailCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<string> HandleAsync(WelcomeEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending welcome email to {Email}", request.Email);
            await Task.Delay(500, cancellationToken);
            return $"Welcome email sent to {request.Name}";
        }
    }
}
