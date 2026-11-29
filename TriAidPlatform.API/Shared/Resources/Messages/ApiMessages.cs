namespace TriAidPlatform.API.Shared.Resources.Messages;

/// <summary>
/// Marker type for the API message catalog (<c>ApiMessages.resx</c>, <c>ApiMessages.es.resx</c>).
/// Controllers resolve the message of an error by key so the answer follows the
/// client's language (Accept-Language) while the domain keeps its own messages.
/// </summary>
public sealed class ApiMessages;
