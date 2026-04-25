namespace EmployeeCrud.Web.Logging;

public sealed class GrafanaLokiOptions
{
    public const string SectionName = "GrafanaLoki";

    public bool Enabled { get; set; }

    public string Endpoint { get; set; } = "http://localhost:3100";

    public string Application { get; set; } = "EmployeeCrud";

    public string? Username { get; set; }

    public string? Password { get; set; }

    public Dictionary<string, string> Labels { get; set; } = [];
}
