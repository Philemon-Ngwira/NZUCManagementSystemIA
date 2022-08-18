using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NZUCManagementSystemIA.Server.Data
{
    public class RolesConfigurator : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = "Reviewer",
                    NormalizedName = "REVIEWER"
                },
                new IdentityRole
                {
                    Name = "Authorizer",
                    NormalizedName = "AUTHORIZER"
                }

                );
        }
    }
}
