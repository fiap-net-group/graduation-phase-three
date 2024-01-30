using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.API.Infrastructure.Database.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class BlogNewsMapping : IEntityTypeConfiguration<BlogNew>
    {
        public void Configure(EntityTypeBuilder<BlogNew> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).IsRequired();

            builder.Property(b => b.Title).HasMaxLength(100).IsRequired();

            builder.Property(b => b.Description).HasMaxLength(300).IsRequired();

            builder.Property(b => b.Body).IsRequired();

            builder.Property(b => b.Enabled).HasDefaultValue(true).IsRequired();

            builder.Ignore(b => b.Tags);

            builder.Property(b => b.InternalTags).HasColumnName("Tags").IsRequired(false);

            builder.Property(b => b.AuthorId).HasColumnName("BlogUserId").IsRequired();

            builder.HasOne(b => b.Author)
                   .WithMany(a => a.WrittenNews)
                   .HasForeignKey(c => c.AuthorId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Property(b => b.CreatedAt).HasDefaultValue(DateTime.Now).IsRequired();

            builder.Property(b => b.LastUpdateAt).HasDefaultValue(DateTime.Now).IsRequired();
        }
    }
}