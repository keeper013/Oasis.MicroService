namespace Oasis.MicroService;

public sealed class MicroServiceConfiguration
{
	public string Path { get; set; } = null!;

	public string? Environment { get; set; }
}