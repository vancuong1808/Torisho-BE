namespace Torisho.Application.DTOs.Dictionary;

public sealed record ExampleDto{
    public string Japanese { get; init; } = string.Empty;
    public string English { get; init; } = string.Empty;
}