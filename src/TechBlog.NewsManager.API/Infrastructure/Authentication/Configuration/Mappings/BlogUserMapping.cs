using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.ValueObjects;

namespace TechBlog.NewsManager.API.Infrastructure.Authentication.Configuration.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class BlogUserMapping : IEntityTypeConfiguration<BlogUser>
    {
        public void Configure(EntityTypeBuilder<BlogUser> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).IsRequired();
            builder.Ignore(b => b.InternalId);

            builder.Property(b => b.BlogUserType)
                              .IsRequired()
                              .HasMaxLength(50)
                              .HasConversion(benum => Enum.GetName(benum),
                                             bname => Enum.Parse<BlogUserType>(bname));

            builder.Property(b => b.EmailConfirmed).HasDefaultValue(true);

            builder.Property(b => b.Name).HasMaxLength(300).IsRequired();

            builder.Property(b => b.Email).IsRequired();

            builder.Property(b => b.UserName).IsRequired();

            builder.Property(b => b.Enabled).HasDefaultValue(true).IsRequired();

            builder.HasMany(b => b.WrittenNews);

            builder.Property(b => b.CreatedAt).HasDefaultValue(DateTime.Now).IsRequired();

            builder.Property(b => b.LastUpdateAt).HasDefaultValue(DateTime.Now).IsRequired();

            builder.ToTable("AspNetUsers");
        }
    }
}
