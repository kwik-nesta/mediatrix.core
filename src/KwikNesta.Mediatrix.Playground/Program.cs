using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Hangfire.Abstractions;
using KwikNesta.Mediatrix.Playground.Commands;
using KwikNesta.Mediatrix.Playground.Notifications;
using KwikNesta.Mediatrix.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KwikNesta.Mediatrix.Hangfire.Extensions;
using KwikNesta.Mediatrix.Core.Implementations.Pipelines;
using Hangfire;
using Hangfire.MemoryStorage;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(config => config.AddConsole());

        // Register core mediator and background mediator
        services
            .AddKwikMediators(typeof(WelcomeEmailCommandHandler).Assembly)
            .AddTransient(typeof(IKwikPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IKwikNotificationBehavior<>), typeof(NotificationLoggingBehavior<>))
            .AddKwikBackgroundMediators();

        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseMemoryStorage();
        });

        services.AddHangfireServer();
    })
    .Build();

var mediator = host.Services.GetRequiredService<IKwikMediator>();
var backgroundMediator = host.Services.GetRequiredService<IKwikBackgroundMediator>();

// Send a command
var result = await mediator.SendAsync(new WelcomeEmailCommand("toba@example.com", "Toba"));
Console.WriteLine(result);

// Publish a notification (immediate)
await mediator.PublishAsync(new UserRegisteredNotification("U123", "toba@example.com"));

// Schedule it as a background job
backgroundMediator.Publish(new UserRegisteredNotification("U123", "toba-public@example.com"));

Console.WriteLine("All operations queued/executed successfully.");
