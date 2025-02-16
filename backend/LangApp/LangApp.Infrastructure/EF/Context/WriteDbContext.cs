using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.EF.Config;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Context;

internal sealed class WriteDbContext : IdentityDbContext<IdentityApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<StudyGroup> StudyGroups { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Lexicon> Lexicons { get; set; }

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("application");
        var configuration = new WriteConfiguration();

        modelBuilder.ApplyConfiguration<IdentityApplicationUser>(configuration);
        modelBuilder.ApplyConfiguration<StudyGroup>(configuration);
        modelBuilder.ApplyConfiguration<Member>(configuration);
        modelBuilder.ApplyConfiguration<Post>(configuration);
        modelBuilder.ApplyConfiguration<Lexicon>(configuration);
        modelBuilder.ApplyConfiguration<LexiconEntry>(configuration);

        base.OnModelCreating(modelBuilder);
    }
}