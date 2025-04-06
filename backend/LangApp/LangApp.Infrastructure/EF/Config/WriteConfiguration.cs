using System.Text.Json;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Infrastructure.EF.Config.Exceptions;
using LangApp.Infrastructure.EF.Config.JsonConfig;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LangApp.Infrastructure.EF.Config;

internal sealed class WriteConfiguration :
    IEntityTypeConfiguration<IdentityApplicationUser>,
    IEntityTypeConfiguration<StudyGroup>,
    IEntityTypeConfiguration<Member>,
    IEntityTypeConfiguration<Post>,
    IEntityTypeConfiguration<Lexicon>,
    IEntityTypeConfiguration<LexiconEntry>,
    IEntityTypeConfiguration<Assignment>,
    IEntityTypeConfiguration<Submission>,
    IEntityTypeConfiguration<IdentityRole<Guid>>,
    IEntityTypeConfiguration<IdentityUserClaim<Guid>>,
    IEntityTypeConfiguration<IdentityUserRole<Guid>>,
    IEntityTypeConfiguration<IdentityUserLogin<Guid>>,
    IEntityTypeConfiguration<IdentityUserToken<Guid>>,
    IEntityTypeConfiguration<IdentityRoleClaim<Guid>>

{
    public void Configure(EntityTypeBuilder<IdentityApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .HasConversion(fn => fn.ToString(), s => new UserFullName(s));

        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }

    public void Configure(EntityTypeBuilder<StudyGroup> builder)
    {
        builder.ToTable("StudyGroups");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Language)
            .HasConversion(g => g.ToString(), s => new Language(s));

        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GroupId);

        builder.HasOne<IdentityApplicationUser>()
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => new { m.UserId, m.GroupId });

        builder.HasOne<IdentityApplicationUser>()
            .WithMany()
            .HasForeignKey(m => m.UserId);

        builder.HasOne<StudyGroup>()
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
            .HasConversion(p => p.ToString(), s => new PostContent(s));

        builder.Property("_media")
            .HasColumnName("Media")
            .HasDefaultValueSql("'{}'::text[]");

        builder.HasOne<IdentityApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<StudyGroup>()
            .WithMany()
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<Lexicon> builder)
    {
        builder.ToTable("Lexicons");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Language)
            .HasConversion(g => g.ToString(), s => new Language(s));
        builder.Property(l => l.Title)
            .HasConversion(g => g.ToString(), s => new LexiconTitle(s));

        builder.HasOne<IdentityApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<LexiconEntry> builder)
    {
        builder.ToTable("LexiconEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Term)
            .HasConversion(term => term.ToString(), value => new Term(value))
            .HasColumnName("Term")
            .IsRequired();

        builder.Navigation(e => e.Definitions).HasField("_definitions");

        builder.HasOne<Lexicon>()
            .WithMany(l => l.Entries)
            .HasForeignKey(e => e.LexiconId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(e => e.Definitions, definitionsBuilder =>
        {
            definitionsBuilder.ToTable("LexiconEntryDefinitions");
            definitionsBuilder.WithOwner().HasForeignKey("LexiconEntryId");

            definitionsBuilder.Property<Guid>("Id").HasColumnType("uuid");
            definitionsBuilder.HasKey("Id");

            definitionsBuilder.Property(d => d.Value).IsRequired();
        });
    }

    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasOne<IdentityApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<StudyGroup>()
            .WithMany()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.Details).HasConversion(entry =>
                JsonSerializer.Serialize(entry,
                    new JsonSerializerOptions { TypeInfoResolver = new PolymorphicTypeResolver<AssignmentDetails>() }),
            value => DeserializeAssignmentDetails(value));
    }

    private static AssignmentDetails DeserializeAssignmentDetails(string json)
    {
        return JsonSerializer.Deserialize<AssignmentDetails>(json,
                   new JsonSerializerOptions { TypeInfoResolver = new PolymorphicTypeResolver<AssignmentDetails>() }) ??
               throw new DeserializationException(typeof(AssignmentDetails), json);
    }

    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        // builder.ToTable("Submissions");
        // builder.HasKey(e => e.Id);
        //
        // builder.Property(e => e.Id).ValueGeneratedNever();
        //
        // builder.HasOne<IdentityApplicationUser>()
        //     .WithMany()
        //     .HasForeignKey(a => a.StudentId)
        //     .OnDelete(DeleteBehavior.Cascade);
        //
        // builder.HasOne<Assignment>()
        //     .WithMany()
        //     .HasForeignKey(a => a.AssignmentId)
        //     .OnDelete(DeleteBehavior.Cascade);
        //
        // builder.Property(a => a.Details).HasConversion(entry =>
        //         JsonSerializer.Serialize(entry,
        //             new JsonSerializerOptions { TypeInfoResolver = new AssignmentDetailsTypeResolver() }),
        //     value => DeserializeAssignmentDetails(value));
    }

    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("Roles");
    }

    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("UserClaims");
    }

    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("UserRoles");
    }

    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("UserLogins");
    }

    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("UserTokens");
    }

    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("RoleClaims");
    }
}