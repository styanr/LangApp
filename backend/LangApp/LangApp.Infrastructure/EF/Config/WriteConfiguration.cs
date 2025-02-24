using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.EF.Identity;
using LangApp.Infrastructure.EF.Models.Lexicons;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LangApp.Infrastructure.EF.Config;

internal sealed class WriteConfiguration :
    IEntityTypeConfiguration<IdentityApplicationUser>,
    IEntityTypeConfiguration<StudyGroup>,
    IEntityTypeConfiguration<Member>,
    IEntityTypeConfiguration<Post>,
    IEntityTypeConfiguration<Lexicon>,
    IEntityTypeConfiguration<LexiconEntry>
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

        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GroupId);
        builder.Property(g => g.Language).HasConversion(g => g.ToString(), s => new Language(s));
    }

    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => new { m.UserId, m.GroupId });

        builder.HasOne<IdentityApplicationUser>().WithMany().HasForeignKey(m => m.UserId);
    }

    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Content)
            .HasConversion(p => p.ToString(), s => new PostContent(s));
    }

    public void Configure(EntityTypeBuilder<Lexicon> builder)
    {
        builder.ToTable("Lexicons");
        builder.HasKey(l => l.Id);

        builder.Property(g => g.Language).HasConversion(g => g.ToString(), s => new Language(s));
        builder.Property(g => g.Title).HasConversion(g => g.ToString(), s => new LexiconTitle(s));
    }

    public void Configure(EntityTypeBuilder<LexiconEntry> builder)
    {
        builder.ToTable("LexiconEntries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // Needed for proper change tracking

        builder.Property(e => e.Term)
            .HasConversion(term => term.ToString(), value => new Term(value))
            .HasColumnName("Term")
            .IsRequired();

        builder.Navigation(e => e.Definitions).HasField("_definitions");

        builder.OwnsMany(e => e.Definitions, definitionsBuilder =>
        {
            definitionsBuilder.ToTable("LexiconEntryDefinitions");

            definitionsBuilder.WithOwner()
                .HasForeignKey("LexiconEntryId");

            definitionsBuilder.Property<Guid>("Id").HasColumnType("uuid");
            definitionsBuilder.HasKey("Id");

            definitionsBuilder.Property(d => d.Value)
                .IsRequired();
        });
    }
}