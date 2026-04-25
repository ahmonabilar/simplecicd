using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;

namespace EmployeeCrud.Web.Logging;

public sealed class GrafanaLokiSink : ILogEventSink, IDisposable
{
    private readonly HttpClient _httpClient = new();
    private readonly Uri _pushEndpoint;
    private readonly string _application;
    private readonly IReadOnlyDictionary<string, string> _labels;

    public GrafanaLokiSink(GrafanaLokiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _application = string.IsNullOrWhiteSpace(options.Application) ? "EmployeeCrud" : options.Application.Trim();
        _pushEndpoint = BuildPushEndpoint(options.Endpoint);
        _labels = options.Labels;

        if (!string.IsNullOrWhiteSpace(options.Username) && options.Password is not null)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }
    }

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        try
        {
            var payload = CreatePayload(logEvent);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            using var response = _httpClient.PostAsync(_pushEndpoint, content).GetAwaiter().GetResult();
        }
        catch
        {
            // Logging must not break the request pipeline if Grafana Loki is unavailable.
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private string CreatePayload(LogEvent logEvent)
    {
        var labels = new Dictionary<string, string>(_labels, StringComparer.Ordinal)
        {
            ["app"] = _application,
            ["level"] = logEvent.Level.ToString()
        };

        var logLine = JsonSerializer.Serialize(new
        {
            message = logEvent.RenderMessage(CultureInfo.InvariantCulture),
            exception = logEvent.Exception?.ToString(),
            properties = logEvent.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ToString("l", CultureInfo.InvariantCulture))
        });

        var timestampNanoseconds = (logEvent.Timestamp.ToUnixTimeMilliseconds() * 1_000_000).ToString(CultureInfo.InvariantCulture);

        return JsonSerializer.Serialize(new
        {
            streams = new[]
            {
                new
                {
                    stream = labels,
                    values = new[]
                    {
                        new[] { timestampNanoseconds, logLine }
                    }
                }
            }
        });
    }

    private static Uri BuildPushEndpoint(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("GrafanaLoki:Endpoint must be configured when GrafanaLoki is enabled.");
        }

        var trimmedEndpoint = endpoint.TrimEnd('/');
        if (!trimmedEndpoint.EndsWith("/loki/api/v1/push", StringComparison.OrdinalIgnoreCase))
        {
            trimmedEndpoint += "/loki/api/v1/push";
        }

        return new Uri(trimmedEndpoint, UriKind.Absolute);
    }
}
