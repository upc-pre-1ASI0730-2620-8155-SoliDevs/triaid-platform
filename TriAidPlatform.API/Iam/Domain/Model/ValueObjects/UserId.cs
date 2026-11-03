using TriAidPlatform.API.Shared.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Represents the strongly-typed identifier for a user in the IAM bounded context.
/// </summary>
/// <param name="Value">The underlying identifier value.</param>
public readonly record struct UserId(int Value) : IEntityId<int>
{
    public static implicit operator int(UserId id) => id.Value;
    public static implicit operator UserId(int value) => new(value);

    public override string ToString() => Value.ToString();
}
