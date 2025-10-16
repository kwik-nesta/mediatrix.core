using KwikNesta.Mediatrix.Core.Abstractions;

namespace KwikNesta.Mediatrix.Playground.Commands
{
    public record WelcomeEmailCommand(string Email, string Name) : IKwikRequest<string>;
}