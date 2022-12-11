using System.Diagnostics;
using Apsitvarkom.Models;
using Castle.DynamicProxy;

namespace Apsitvarkom.Api.Middleware;

public class GeocoderRequestInterceptorAsync : IAsyncInterceptor
{
    private int _requestCount = 0;

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var currentRequest = Interlocked.Increment(ref _requestCount);
        var methodName = invocation.GetConcreteMethod().Name;
        var methodArguments = invocation.Arguments;

        if (methodArguments.IsFixedSize && methodArguments.Length == 1 && methodArguments.Single() is Coordinates coordinates)
            Console.WriteLine($"[{currentRequest}] Method {methodName} was called on coordinates '{coordinates}'.");

        var sw = Stopwatch.StartNew();

        invocation.Proceed();

        var task = (Task<TResult>)invocation.ReturnValue;
        var result = await task;

        if (result is Translated<string> translatedStringResult)
            Console.WriteLine($"[{currentRequest}] Response: {translatedStringResult}. Request took {sw.ElapsedMilliseconds} ms.");

        return result;
    }

    public void InterceptSynchronous(IInvocation invocation) => throw new NotSupportedException();

    public void InterceptAsynchronous(IInvocation invocation) => throw new NotSupportedException();
}