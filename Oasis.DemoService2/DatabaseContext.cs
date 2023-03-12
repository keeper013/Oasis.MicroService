namespace Oasis.DemoService2;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public sealed class Service
{
	public string? Name { get; set; }
}

public sealed class DatabaseContext : DbContext
{
	public DatabaseContext(DbContextOptions options)
		: base(options)
	{
	}

	[NotMapped]
	public DbSet<Service> Services { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Service>().HasNoKey();
	}
}