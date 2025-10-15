### KwikNesta.Mediatrix.Core

[![NuGet](https://img.shields.io/nuget/v/KwikNesta.Mediatrix.Core.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core)
[![NuGet Downloads](https://img.shields.io/nuget/dt/KwikNesta.Mediatrix.Core.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core)

### KwikNesta.Mediatrix.Hangfire

[![NuGet](https://img.shields.io/nuget/v/KwikNesta.Mediatrix.Hangfire.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire)
[![NuGet Downloads](https://img.shields.io/nuget/dt/KwikNesta.Mediatrix.Hangfire.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire)

````markdown
# KwikNesta.Mediatrix

Lightweight, dependency-free mediator for .NET with optional Hangfire background publishing.

## Install

```bash
dotnet add package KwikNesta.Mediatrix.Core
dotnet add package KwikNesta.Mediatrix.Hangfire
````

## Quick Start

```csharp
public record GreetCommand(string Name) : IRequest<string>;

public class GreetHandler : IRequestHandler<GreetCommand, string>
{
    public Task<string> Handle(GreetCommand request, CancellationToken ct) =>
        Task.FromResult($"Hello, {request.Name}!");
}

// registration
services.AddMediatrix();

// usage
var mediator = provider.GetRequiredService<IMediator>();
var result = await mediator.Send(new GreetCommand("Toba"));
Console.WriteLine(result);
```

## Hangfire Background Example

```csharp
var backgroundMediator = provider.GetRequiredService<IBackgroundMediator>();
backgroundMediator.Publish(new UserRegisteredNotification("toba@example.com"));
```

---

**NuGet:** [KwikNesta.Mediatrix.Core](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core) <br/>
**NuGet:** [KwikNesta.Mediatrix.Hangfire](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire) <br/>
**License:** MIT <br/>
**Author:** Ojo Toba R. <br/>
**GitHub:** [https://github.com/kwik-nesta/mediatrix.core](https://github.com/kwik-nesta/mediatrix.core) <br />
