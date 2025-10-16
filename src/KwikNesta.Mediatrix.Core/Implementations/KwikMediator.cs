using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Core.Internal;

namespace KwikNesta.Mediatrix.Core.Implementations
{
    public class KwikMediator : IKwikMediator
    {
        private readonly IServiceProvider _provider;
        private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>> _requestHandlerCache = new();
        private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task>> _notificationHandlerCache = new();

        public KwikMediator(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }

        public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : IKwikNotification
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var handlerInterface = typeof(IKwikNotificationHandler<>).MakeGenericType(typeof(TNotification));
            var handlers = (IEnumerable)(_provider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerInterface)) ?? Array.Empty<object>());

            var executor = _notificationHandlerCache.GetOrAdd(handlerInterface, static type =>
            {
                var handlerParam = Expression.Parameter(typeof(object), "handler");
                var notificationParam = Expression.Parameter(typeof(object), "notification");
                var tokenParam = Expression.Parameter(typeof(CancellationToken), "token");

                var method = type.GetMethod("HandleAsync")!;
                var notificationType = type.GetGenericArguments()[0];

                var castHandler = Expression.Convert(handlerParam, type);
                var castNotification = Expression.Convert(notificationParam, notificationType);

                var call = Expression.Call(castHandler, method, castNotification, tokenParam);

                var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task>>(
                    call, handlerParam, notificationParam, tokenParam);

                return lambda.Compile();
            });

            var behaviors = GetNotificationBehaviors(notification);
            var handlersList = handlers.Cast<object>().ToList();
            var index = 0;

            async Task Next()
            {
                if (index < behaviors.Count)
                {
                    await behaviors[index++].HandleAsync(notification, cancellationToken, Next).ConfigureAwait(false);
                }
                else
                {
                    foreach (var handler in handlersList)
                        await executor(handler, notification!, cancellationToken).ConfigureAwait(false);
                }
            }

            await Next().ConfigureAwait(false);

        }

        public async Task<TResponse> SendAsync<TResponse>(IKwikRequest<TResponse> request, 
                                                          CancellationToken cancellationToken = default)
        {
            if (request != null)
            {
                var handlerInterface = typeof(IKwikRequestHandler<,>)
                    .MakeGenericType(request.GetType(), typeof(TResponse));

                var handler = _provider.GetService(handlerInterface)
                    ?? throw new InvalidOperationException($"No handler registered for {handlerInterface.Name}");

                var executor = _requestHandlerCache.GetOrAdd(handlerInterface,
                    static type => HandlerExecutorBuilder.Build(type));

                var behaviors = GetPipelineBehaviors(request, typeof(TResponse));
                var index = 0;

                async Task<TResponse> Next()
                {
                    if (index < behaviors.Count)
                    {
                        var behavior = behaviors[index++];
                        var method = behavior.GetType().GetMethod("HandleAsync")!;
                        return await (Task<TResponse>)method.Invoke(behavior, new object[]
                        {
                            request,
                            cancellationToken,
                            (Func<Task<TResponse>>)Next
                        })!;
                    }
                    else
                    {
                        return (TResponse)await executor(handler, request!, cancellationToken);
                    }
                }

                return await Next();
            }

            throw new ArgumentNullException(nameof(request));
        }

        #region Private methods
        private List<dynamic> GetPipelineBehaviors(object request, Type responseType)
        {
            var behaviorType = typeof(IKwikPipelineBehavior<,>).MakeGenericType(request.GetType(), responseType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(behaviorType);
            var behaviors = (IEnumerable)(_provider.GetService(enumerableType) ?? Array.Empty<object>());
            return behaviors.Cast<dynamic>().ToList();
        }

        private List<IKwikNotificationBehavior<TNotification>> GetNotificationBehaviors<TNotification>(TNotification notification)
            where TNotification : IKwikNotification
        {
            var behaviorType = typeof(IKwikNotificationBehavior<>).MakeGenericType(typeof(TNotification));
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(behaviorType);
            var behaviors = (IEnumerable)(_provider.GetService(enumerableType) ?? Array.Empty<object>());
            return behaviors.Cast<IKwikNotificationBehavior<TNotification>>().ToList();
        }

        #endregion
    }
}
