using System.Text.Json;
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
    IEntityTypeConfiguration<PostReadModel>,
    IEntityTypeConfiguration<LexiconReadModel>,
    IEntityTypeConfiguration<LexiconEntryReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName)
            .HasConversion(fn => fn.ToString(), s => new FullNameReadModel(s));

        builder.HasMany(u => u.StudyGroups)
            .WithMany(g => g.Members);
        builder.HasMany(u => u.ManagedGroups)
            .WithOne(g => g.Owner);
        builder.HasOne(u => u.Lexicon)
            .WithOne(l => l.Owner);
    }

    public void Configure(EntityTypeBuilder<StudyGroupReadModel> builder)
    {
        builder.ToTable("StudyGroups");
        builder.HasKey(g => g.Id);

        builder.HasOne(g => g.Owner)
            .WithMany(u => u.ManagedGroups);

        builder.HasMany(g => g.Members)
            .WithMany(u => u.StudyGroups);

        builder.HasMany(g => g.Posts)
            .WithOne(p => p.Group);
    }

    public void Configure(EntityTypeBuilder<PostReadModel> builder)
    {
        builder.ToTable("Posts");
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Author)
            .WithMany();

        builder.HasOne(p => p.Group)
            .WithMany(g => g.Posts);
    }

    public void Configure(EntityTypeBuilder<LexiconReadModel> builder)
    {
        builder.ToTable("Lexicons");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Language).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Title).IsRequired().HasMaxLength(255);

        builder.HasOne(l => l.Owner)
            .WithOne(u => u.Lexicon)
            .HasForeignKey<LexiconReadModel>(l => l.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Entries)
            .WithOne(e => e.Lexicon)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<LexiconEntryReadModel> builder)
    {
        builder.ToTable("LexiconEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Definitions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)default),
                v => JsonSerializer.Deserialize<ICollection<string>>(v, (JsonSerializerOptions?)default) ??
                     new List<string>()
            );

        builder.HasOne(e => e.Lexicon)
            .WithMany(l => l.Entries);
    }
}