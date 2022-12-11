using Castle.DynamicProxy;

namespace Apsitvarkom.Api.Middleware;

// This class decorates IAsyncInterceptor providing the bridge between IInterceptor and IAsyncInterceptor.
public class GeocoderRequestInterceptor : IInterceptor
{
    IAsyncInterceptor _asyncInterceptor;

    public GeocoderRequestInterceptor(IAsyncInterceptor asyncInterceptor)
    {
        _asyncInterceptor = asyncInterceptor;
    }

    public void Intercept(IInvocation invocation)
    {
        _asyncInterceptor.ToInterceptor().Intercept(invocation);
    }
}