````markdown
# KwikNesta.Mediatrix

Lightweight, dependency-free mediator for .NET with optional **Hangfire background publishing** and **logging behaviors**.

### KwikNesta.Mediatrix.Core

[![NuGet](https://img.shields.io/nuget/v/KwikNesta.Mediatrix.Core.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core)
[![NuGet Downloads](https://img.shields.io/nuget/dt/KwikNesta.Mediatrix.Core.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core)

### KwikNesta.Mediatrix.Hangfire

[![NuGet](https://img.shields.io/nuget/v/KwikNesta.Mediatrix.Hangfire.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire)
[![NuGet Downloads](https://img.shields.io/nuget/dt/KwikNesta.Mediatrix.Hangfire.svg)](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire)

## Installation

Install the core mediator:

```bash
dotnet add package KwikNesta.Mediatrix.Core
````

Optional: add background publishing with Hangfire:

```bash
dotnet add package KwikNesta.Mediatrix.Hangfire
```

---

## Quick Start

### Define a Request and Handler

```csharp
public record GreetCommand(string Name) : IRequest<string>;

public class GreetHandler : IRequestHandler<GreetCommand, string>
{
    public Task<string> Handle(GreetCommand request, CancellationToken ct)
        => Task.FromResult($"Hello, {request.Name}!");
}
```

---

### Register Mediatrix

```csharp
using KwikNesta.Mediatrix.Core.Extensions;

var services = new ServiceCollection();
services.AddMediatrix();
```

---

### Use the Mediator

```csharp
var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IKwikMediator>();

var result = await mediator.SendAsync(new GreetCommand("Toba"));
Console.WriteLine(result); // "Hello, Toba!"
```

---

## Publishing Notifications

```csharp
public record UserRegisteredNotification(string Email) : INotification;

public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredNotification>
{
    public Task Handle(UserRegisteredNotification notification, CancellationToken ct)
    {
        Console.WriteLine($"Welcome email sent to {notification.Email}");
        return Task.CompletedTask;
    }
}
```

Usage:

```csharp
await mediator.PublishAsync(new UserRegisteredNotification("toba@example.com"));
```

---

## Logging Behaviors

Behaviors allow you to run code before and after every request.
The logging behavior included in **KwikNesta.Mediatrix.Core** automatically logs:

* Request start and completion
* Execution duration
* Exception details (if any)

### Enabling Behaviors

```csharp
services
    .AddKwikMediators(typeof(GreetCommandHandler).Assembly)
    .AddTransient(typeof(IKwikPipelineBehavior<,>), typeof(LoggingBehavior<,>))
    .AddTransient(typeof(IKwikNotificationBehavior<>), typeof(NotificationLoggingBehavior<>))
    .AddKwikBackgroundMediators();
```

The `UseLoggingBehavior()` extension wires in `LoggingBehavior<TRequest, TResponse>`, which wraps all handler calls transparently.

---

## Hangfire Background Mediator

When you install **KwikNesta.Mediatrix.Hangfire**, you can offload notifications to Hangfire jobs.

### Configure Hangfire

```csharp
services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMemoryStorage();
});
services.AddHangfireServer();

// Adds background mediator
services
    .AddKwikMediators(typeof(GreetCommandHandler).Assembly)
    .AddKwikBackgroundMediators();```

---

### Publish Notifications in the Background

```csharp
var backgroundMediator = provider.GetRequiredService<IKwikBackgroundMediator>();

await backgroundMediator.Publish(new UserRegisteredNotification("toba@example.com"));
```

When Hangfire is available, this enqueues a background job for processing the notification handler.
If Hangfire isn�t configured, it automatically falls back to in-process execution � no crashes, no configuration headaches.

---

## Example Program

```csharp
var services = new ServiceCollection();

// core mediator
services.AddMediatrix(o => o.UseLoggingBehavior());

// background mediator with Hangfire
services.AddHangfire(config => config.UseMemoryStorage());
services.AddHangfireServer();
services.AddBackgroundMediatrix();

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IKwikMediator>();
var backgroundMediator = provider.GetRequiredService<IKwikBackgroundMediator>();

await mediator.SendAsync(new GreetCommand("Toba"));
await backgroundMediator.Publish(new UserRegisteredNotification("toba@example.com"));
```

---

## License

**MIT**

## Author

**Ojo Toba R.**

* **NuGet:** [KwikNesta.Mediatrix.Core](https://www.nuget.org/packages/KwikNesta.Mediatrix.Core)
* **NuGet:** [KwikNesta.Mediatrix.Hangfire](https://www.nuget.org/packages/KwikNesta.Mediatrix.Hangfire)
* **GitHub:** [https://github.com/kwik-nesta/mediatrix.core](https://github.com/kwik-nesta/mediatrix.core)