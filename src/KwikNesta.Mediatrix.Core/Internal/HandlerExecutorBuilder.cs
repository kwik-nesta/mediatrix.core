namespace KwikNesta.Mediatrix.Core.Internal
{
    internal static class HandlerExecutorBuilder
    {
        public static Func<object, object, CancellationToken, Task<object>> Build(Type handlerInterface)
        {
            var method = handlerInterface.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException($"No HandleAsync method found for {handlerInterface.Name}");

            return async (handler, request, token) =>
            {
                var task = (Task)method.Invoke(handler, new[] { request, token })!;

                await task.ConfigureAwait(false);

                var resultProp = task.GetType().GetProperty("Result");
                return resultProp != null ? resultProp.GetValue(task)! : null!;
            };
        }
    }
}
