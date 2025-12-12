namespace NETworkManager.Models.Network;

/// <summary>
/// Defines the classification for a network connection based on its environment or security level.
/// Used to specify the type and trustworthiness of the network for configuration and policy application.
/// </summary>
public enum NetworkCategory
{
    /// <summary>
    /// Represents a network category that has not been configured.
    /// This value indicates that no specific classification has been assigned to the network.
    /// It is commonly used as a default or placeholder state before any configuration is applied.
    /// </summary>
    NotConfigured,

    /// <summary>
    /// Represents a private network category, generally marked as trusted.
    /// This category is ideal for environments such as home or small office networks,
    /// where devices can communicate freely, and sharing or discovery features may be enabled with fewer restrictions.
    /// </summary>
    Private,

    /// <summary>
    /// Represents a network that is categorized as public.
    /// This category is commonly used for networks in public spaces such as libraries,
    /// hotels, or airports, where the network is considered untrusted.
    /// Connections in this category typically enforce stricter security policies to reduce potential risks.
    /// </summary>
    Public,

    /// <summary>
    /// Indicates that the network is authenticated with a domain controller.
    /// This category typically applies to networks managed within enterprise or organizational environments where
    /// centralized authentication and policies are enforced.
    /// Domain-authenticated networks commonly allow access to internal resources and services with predefined security configurations.
    /// </summary>
    DomainAuthenticated
}