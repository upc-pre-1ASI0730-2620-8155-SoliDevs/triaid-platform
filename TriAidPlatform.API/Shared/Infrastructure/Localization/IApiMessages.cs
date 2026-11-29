using Microsoft.Extensions.Localization;
using TriAidPlatform.API.Shared.Domain.Model.Errors;
using TriAidPlatform.API.Shared.Resources.Messages;

namespace TriAidPlatform.API.Shared.Infrastructure.Localization;

/// <summary>Localized API messages keyed by error name, following the request culture.</summary>
public interface IApiMessages
{
    /// <summary>Message for a key with format arguments; the key itself when the catalog has no entry.</summary>
    string this[string key, params object[] args] { get; }

    /// <summary>Message for a key; null when the catalog has no entry.</summary>
    string? Find(string key);

    /// <summary>Message for a typed error of a bounded context (key = "<paramref name="prefix"/>.<paramref name="errorType"/>").</summary>
    string? Find(string prefix, object errorType);

    /// <summary>Message for an <see cref="Error"/>: its code, or "<prefix>.<type>" when given; falls back to the domain message.</summary>
    string For(Error error, string? prefix = null);
}

public class ApiMessagesAccessor(IStringLocalizer<ApiMessages> localizer) : IApiMessages
{
    public string this[string key, params object[] args]
    {
        get
        {
            var entry = localizer[key, args];
            return entry.ResourceNotFound ? key : entry.Value;
        }
    }

    public string? Find(string key)
    {
        var entry = localizer[key];
        return entry.ResourceNotFound ? null : entry.Value;
    }

    public string? Find(string prefix, object errorType) => Find($"{prefix}.{errorType}");

    public string For(Error error, string? prefix = null)
    {
        var typed = prefix is null ? null : Find(prefix, error.Code);
        return typed ?? Find(error.Code) ?? error.Message;
    }
}
