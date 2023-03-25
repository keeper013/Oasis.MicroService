namespace Oasis.MicroService;

using System.Collections.Generic;

public interface IAssemblyConfiguration
{
	string Directory { get; }

	string SearchPattern { get; }
}

public sealed class IncludedAssemblyConfiguration : IAssemblyConfiguration
{
	public string Directory { get; set; } = null!;

	public string SearchPattern { get; set; } = null!;

	public string? Environment { get; set; }
}

public sealed class ExcludedAssemblyConfiguration : IAssemblyConfiguration
{
	public string Directory { get; set; } = null!;

	public string SearchPattern { get; set; } = null!;
}

public sealed class MicroServiceConfiguration
{
	public IList<IncludedAssemblyConfiguration> Included { get; set; } = null!;

	public IList<ExcludedAssemblyConfiguration>? Excluded { get; set; }

	public bool IgnoreAssemblyLoadingErrors { get; set; }
}

public static class AssemblyConfigurationExtension
{
	public static void ThrowIfInvalid(this IAssemblyConfiguration config)
	{
		if (string.IsNullOrEmpty(config.Directory) || string.IsNullOrEmpty(config.SearchPattern))
		{
			throw new ArgumentNullException(nameof(MicroServiceConfiguration), "Both Diretory and SearchPattern are madatory for searching for assemblies.");
		}
	}
}