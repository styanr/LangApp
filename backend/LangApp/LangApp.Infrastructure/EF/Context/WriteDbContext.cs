using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Submissions;
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
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Activity> Activities { get; set; }

    public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    public DbSet<ActivitySubmission> ActivitySubmissions { get; set; }


    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.HasDefaultSchema("application");
        var configuration = new WriteConfiguration();

        modelBuilder.ApplyConfiguration<IdentityApplicationUser>(configuration);
        modelBuilder.ApplyConfiguration<StudyGroup>(configuration);
        modelBuilder.ApplyConfiguration<Member>(configuration);
        modelBuilder.ApplyConfiguration<Post>(configuration);
        modelBuilder.ApplyConfiguration<PostComment>(configuration);
        modelBuilder.ApplyConfiguration<Activity>(configuration);
        modelBuilder.ApplyConfiguration<Assignment>(configuration);
        modelBuilder.ApplyConfiguration<AssignmentSubmission>(configuration);
        modelBuilder.ApplyConfiguration<ActivitySubmission>(configuration);
        modelBuilder.ApplyConfiguration<IdentityRole<Guid>>(configuration);
        modelBuilder.ApplyConfiguration<IdentityUserClaim<Guid>>(configuration);
        modelBuilder.ApplyConfiguration<IdentityUserRole<Guid>>(configuration);
        modelBuilder.ApplyConfiguration<IdentityUserLogin<Guid>>(configuration);
        modelBuilder.ApplyConfiguration<IdentityUserToken<Guid>>(configuration);
        modelBuilder.ApplyConfiguration<IdentityRoleClaim<Guid>>(configuration);
        var model = modelBuilder.Model;
        Console.WriteLine(model.ToDebugString());
    }
}
