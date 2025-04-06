using System.Text.Json;
using LangApp.Infrastructure.EF.Config.Exceptions;
using LangApp.Infrastructure.EF.Config.JsonConfig.ReadContext;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Identity;
using LangApp.Infrastructure.EF.Models.Lexicons;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LangApp.Infrastructure.EF.Config;

internal sealed class ReadConfiguration :
    IEntityTypeConfiguration<UserReadModel>,
    IEntityTypeConfiguration<StudyGroupReadModel>,
    IEntityTypeConfiguration<MemberReadModel>,
    IEntityTypeConfiguration<PostReadModel>,
    IEntityTypeConfiguration<LexiconReadModel>,
    IEntityTypeConfiguration<LexiconEntryReadModel>,
    IEntityTypeConfiguration<LexiconEntryDefinitionReadModel>,
    IEntityTypeConfiguration<AssignmentReadModel>,
    IEntityTypeConfiguration<IdentityRoleReadModel>,
    IEntityTypeConfiguration<IdentityUserClaimReadModel>,
    IEntityTypeConfiguration<IdentityUserRoleReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName)
            .HasConversion(fn => fn.ToString(), s => new FullNameReadModel(s));

        builder.Property(u => u.Username).HasColumnName("UserName");

        builder.HasMany(u => u.StudyGroups)
            .WithMany(g => g.Members).UsingEntity<MemberReadModel>();
        builder.HasMany(u => u.ManagedGroups)
            .WithOne(g => g.Owner).HasForeignKey(g => g.OwnerId);
        builder.HasMany(u => u.Lexicons)
            .WithOne(l => l.Owner).HasForeignKey(l => l.UserId);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }

    public void Configure(EntityTypeBuilder<StudyGroupReadModel> builder)
    {
        builder.ToTable("StudyGroups");
        builder.HasKey(g => g.Id);

        builder.HasOne(g => g.Owner)
            .WithMany(u => u.ManagedGroups);

        builder.HasMany(g => g.Members)
            .WithMany(u => u.StudyGroups)
            .UsingEntity<MemberReadModel>(
                m => m.HasOne<UserReadModel>()
                    .WithMany()
                    .HasForeignKey(mb => mb.UserId),
                m => m.HasOne<StudyGroupReadModel>()
                    .WithMany()
                    .HasForeignKey(mb => mb.GroupId));

        builder.HasMany(g => g.Posts)
            .WithOne(p => p.Group).HasForeignKey(p => p.GroupId);
    }

    public void Configure(EntityTypeBuilder<MemberReadModel> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => new { m.UserId, m.GroupId });
        builder.HasOne<UserReadModel>().WithMany().HasForeignKey(m => m.UserId);
        builder.HasOne<StudyGroupReadModel>().WithMany().HasForeignKey(m => m.GroupId);
    }


    public void Configure(EntityTypeBuilder<PostReadModel> builder)
    {
        builder.ToTable("Posts");
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Author)
            .WithMany().HasForeignKey(p => p.AuthorId);

        builder.HasOne(p => p.Group)
            .WithMany(g => g.Posts);
    }

    public void Configure(EntityTypeBuilder<LexiconReadModel> builder)
    {
        builder.ToTable("Lexicons");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Language).IsRequired();
        builder.Property(l => l.Title).IsRequired();

        builder.HasOne(l => l.Owner)
            .WithMany(u => u.Lexicons)
            .HasForeignKey(l => l.UserId)
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<LexiconEntryReadModel> builder)
    {
        builder.ToTable("LexiconEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Term).IsRequired();

        builder.HasOne(e => e.Lexicon)
            .WithMany(l => l.Entries)
            .HasForeignKey(e => e.LexiconId)
            .IsRequired();

        builder.HasMany(e => e.Definitions)
            .WithOne(d => d.Entry)
            .HasForeignKey(d => d.LexiconEntryId)
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<LexiconEntryDefinitionReadModel> builder)
    {
        builder.ToTable("LexiconEntryDefinitions");
        builder.HasKey(d => d.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(d => d.LexiconEntryId).IsRequired();
        builder.Property(d => d.Value).IsRequired();
    }

    public void Configure(EntityTypeBuilder<AssignmentReadModel> builder)
    {
        builder.ToTable("Assignments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.AuthorId);
        builder.Property(a => a.GroupId);
        builder.Property(a => a.DueTime);
        builder.Property(a => a.MaxScore);
        builder.Property(a => a.Type);

        builder.Property(a => a.Details)
            .HasConversion(
                details => JsonSerializer.Serialize(details, new JsonSerializerOptions
                    ()),
                json => DeserializeAssignmentDetails(json)
            );
    }

    private static AssignmentDetailsReadModel DeserializeAssignmentDetails(string json)
    {
        return JsonSerializer.Deserialize<AssignmentDetailsReadModel>(json, new JsonSerializerOptions
               {
                   TypeInfoResolver = new AssignmentDetailsReadModelTypeResolver()
               })
               ?? throw new DeserializationException(typeof(AssignmentDetailsReadModel), json);
    }


    public void Configure(EntityTypeBuilder<IdentityRoleReadModel> builder)
    {
        builder.ToTable("Roles");
    }

    public void Configure(EntityTypeBuilder<IdentityUserClaimReadModel> builder)
    {
        builder.ToTable("Claims");
    }

    public void Configure(EntityTypeBuilder<IdentityUserRoleReadModel> builder)
    {
        builder.ToTable("UserRoles");
    }
}