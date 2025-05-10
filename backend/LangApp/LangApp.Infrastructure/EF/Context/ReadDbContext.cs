using LangApp.Infrastructure.EF.Config;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Identity;
using LangApp.Infrastructure.EF.Models.Lexicons;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Context;

internal sealed class ReadDbContext : DbContext
{
    public DbSet<UserReadModel> Users { get; set; }
    public DbSet<StudyGroupReadModel> StudyGroups { get; set; }
    public DbSet<PostReadModel> Posts { get; set; }
    public DbSet<LexiconReadModel> Lexicons { get; set; }
    public DbSet<AssignmentReadModel> Assignments { get; set; }
    public DbSet<SubmissionReadModel> Submissions { get; set; }

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("application");

        var configuration = new ReadConfiguration();

        modelBuilder.ApplyConfiguration<UserReadModel>(configuration);
        modelBuilder.ApplyConfiguration<StudyGroupReadModel>(configuration);
        modelBuilder.ApplyConfiguration<MemberReadModel>(configuration);
        modelBuilder.ApplyConfiguration<PostReadModel>(configuration);
        modelBuilder.ApplyConfiguration<PostCommentReadModel>(configuration);
        modelBuilder.ApplyConfiguration<LexiconReadModel>(configuration);
        modelBuilder.ApplyConfiguration<LexiconEntryReadModel>(configuration);
        modelBuilder.ApplyConfiguration<LexiconEntryDefinitionReadModel>(configuration);
        modelBuilder.ApplyConfiguration<AssignmentReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SubmissionReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SubmissionGradeReadModel>(configuration);
        modelBuilder.ApplyConfiguration<IdentityRoleReadModel>(configuration);
        modelBuilder.ApplyConfiguration<IdentityUserClaimReadModel>(configuration);

        base.OnModelCreating(modelBuilder);
    }
}
