using System.Text.RegularExpressions;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace StarterKit.BFF.ASPNETCoreWithVue.Server.Commerble;


public static class IReverseProxyBuilderExtensions
{
    const string ROUTE_ID = "commerble";
    const string CLUSTER_ID = "commerble";
    const string DESTINATION_ID = "commerble";

    public static void AddCommerbleProxy(this IReverseProxyBuilder builder, ProxySettings settings)
    {
        var (routes, clusters) = GetConfigs(settings);

        builder
            .LoadFromMemory(routes, clusters)
            .AddTransforms(CreateResponseTransform(settings));
    }

    static Action<TransformBuilderContext> CreateResponseTransform(ProxySettings settings)
    {
        return new Action<TransformBuilderContext>(builderContext =>
        {
            if (builderContext.Route.RouteId == ROUTE_ID)
            {
                builderContext.AddResponseTransform(transformContext =>
                {
                    var requestUri = transformContext.ProxyResponse?.RequestMessage?.RequestUri;
                    var locationUri = transformContext.ProxyResponse?.Headers.Location;

                    if (requestUri != null && locationUri != null)
                    {
                        transformContext.HttpContext.Response.Headers.Location = ReplaceLocation(locationUri, settings);
                        if (!IsSamePath(requestUri, requestUri, locationUri)
                            && !IsPurchaseInit(requestUri, locationUri)
                            && !IsSiteLogout(requestUri, requestUri)
                            )
                        {
                            if (new[] { 302, 303, 307 }.Contains((int)transformContext.ProxyResponse!.StatusCode))
                            {
                                transformContext.HttpContext.Response.Headers.CacheControl = "no-store";
                            }
                            transformContext.HttpContext.Response.StatusCode = 202;
                        }
                    }

                    return ValueTask.CompletedTask;
                });
            }
        });
    }

    static string ReplaceLocation(Uri location, ProxySettings settings)
    {
        var baseUri = new Uri(settings.Endpoint);
        if (location.IsAbsoluteUri)
        {
            return $"{settings.Prefix}{baseUri.MakeRelativeUri(location)}";
        }

        var strLocation = location.ToString();
        if (strLocation.StartsWith(baseUri.AbsolutePath, StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{settings.Prefix}{strLocation.Substring(baseUri.AbsolutePath.Length)}";
        }

        return location.ToString();
    }

    static bool IsSamePath(Uri baseUri, Uri a, Uri b)
    {
        if (a.IsAbsoluteUri && b.IsAbsoluteUri)
            return a.AbsolutePath.Equals(b.AbsolutePath, StringComparison.InvariantCultureIgnoreCase);

        if (a.IsAbsoluteUri && !b.IsAbsoluteUri)
            return a.AbsolutePath.Equals(new Uri(baseUri, b.ToString()).AbsolutePath, StringComparison.InvariantCultureIgnoreCase);

        if (!a.IsAbsoluteUri && b.IsAbsoluteUri)
            return new Uri(baseUri, a.ToString()).AbsolutePath.Equals(b.AbsolutePath, StringComparison.InvariantCultureIgnoreCase);

        return new Uri(baseUri, a.ToString()).AbsolutePath.Equals(new Uri(baseUri, b.ToString()).AbsolutePath, StringComparison.InvariantCultureIgnoreCase);
    }

    static Regex _regexPurchaseInit = new Regex(@"/purchase/\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static bool IsPurchaseInit(Uri baseUri, Uri uri)
    {
        var path = uri.IsAbsoluteUri ? uri.AbsolutePath : new Uri(baseUri, uri.ToString()).AbsolutePath;

        return _regexPurchaseInit.IsMatch(path);
    }

    static Regex _regexSiteLogout = new Regex(@"/site/logout$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    static bool IsSiteLogout(Uri baseUri, Uri uri)
    {
        var path = uri.IsAbsoluteUri ? uri.AbsolutePath : new Uri(baseUri, uri.ToString()).AbsolutePath;

        return _regexSiteLogout.IsMatch(path);
    }

    static (IReadOnlyList<RouteConfig>, IReadOnlyList<ClusterConfig>) GetConfigs(ProxySettings settings)
    {
        var commerble = new Uri(settings.Endpoint);

        var clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = CLUSTER_ID,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    [DESTINATION_ID] = new DestinationConfig
                    {
                        Address = $"{commerble.Scheme}://{commerble.Authority}/",
                    }
                }
            }
        };

        var transforms = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                ["PathPattern"] = $"{commerble.AbsolutePath}{{**catch-all}}"
            }
        };

        if (settings.Headers != null)
        {
            transforms.AddRange(settings.Headers.Select(kv => new Dictionary<string, string>
            {
                ["RequestHeader"] = kv.Key,
                ["Append"] = kv.Value
            }));
        }

        var routes = new[]
        {
            new RouteConfig
            {
                RouteId = ROUTE_ID,
                ClusterId = CLUSTER_ID,
                Match = new RouteMatch
                {
                    Path = $"{settings.Prefix}{{**catch-all}}"
                },
                Transforms = transforms
            }
        };

        return (routes, clusters);
    }
}

public class ProxySettings
{
    public required string Endpoint { get; set; }
    public IReadOnlyDictionary<string, string>? Headers { get; set; }
    public required string Prefix { get; set; }
}


