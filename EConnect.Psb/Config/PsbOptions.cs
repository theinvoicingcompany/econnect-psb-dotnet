namespace EConnect.Psb.Config;

public class PsbOptions
{
    public string? PsbUrl { get; set; }

    /// <summary>
    /// Optional subscription key. When empty, no Subscription-Key header is sent.
    /// </summary>
    public string? SubscriptionKey { get; set; }

    public string? IdentityUrl { get; set; }

    /// <summary>
    /// Required for all authentication flows.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Required for all authentication flows.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Required for the Resource Owner Password flow. Used together with <see cref="Password"/>.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// When set, the Resource Owner Password flow is used; otherwise the Client Credentials flow.
    /// </summary>
    public string? Password { get; set; }
}
