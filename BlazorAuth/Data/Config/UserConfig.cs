using BlazorAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorAuth.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Firstname)
                .HasMaxLength(50);

            builder.Property(u => u.Lastname)
                .HasMaxLength(50);
        }
    }
}
