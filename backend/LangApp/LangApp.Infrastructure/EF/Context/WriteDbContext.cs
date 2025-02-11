using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Context;

internal sealed class WriteDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<StudyGroup> StudyGroups { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Lexicon> Lexicons { get; set; }

    public WriteDbContext(DbContextOptions<ReadDbContext> options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("lang_app");
        base.OnModelCreating(modelBuilder);
    }
}