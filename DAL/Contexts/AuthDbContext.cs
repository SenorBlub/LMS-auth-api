using Logic.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contexts;

public class AuthDbContext : DbContext
{
	public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

	// DbSet representing the refresh_tokens table
	public DbSet<RefreshToken> RefreshTokens { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Map RefreshToken entity to refresh_tokens table
		modelBuilder.Entity<RefreshToken>(entity =>
		{
			entity.ToTable("refresh_tokens");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Id)
				.HasColumnName("id")
				.HasColumnType("char(36)")  // MySQL GUID type
				.IsRequired();

			entity.Property(e => e.Token)
				.HasColumnName("token")
				.HasMaxLength(512)
				.IsRequired();

			entity.Property(e => e.UserId)
				.HasColumnName("user_id")
				.HasColumnType("char(36)")
				.IsRequired();

			entity.Property(e => e.ExpiresAt)
				.HasColumnName("expires_at")
				.IsRequired();

			entity.Property(e => e.Revoked)
				.HasColumnName("revoked")
				.IsRequired();

			entity.Property(e => e.CreatedAt)
				.HasColumnName("created_at")
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.IsRequired();
		});
	}
}