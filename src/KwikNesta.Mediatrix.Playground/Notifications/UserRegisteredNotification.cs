using KwikNesta.Mediatrix.Core.Abstractions;

namespace KwikNesta.Mediatrix.Playground.Notifications
{
    public record UserRegisteredNotification(string UserId, string Email) : IKwikNotification;
}
