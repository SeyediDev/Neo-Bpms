using Neo.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Neo.Bpms.Infrastructure.Data.Configurations;
internal class SubjectSettingConfiguration : IEntityTypeConfiguration<SubjectSetting>
{
    public void Configure(EntityTypeBuilder<SubjectSetting> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CreateDate).HasColumnType("datetime");
        entity.Property(e => e.ExpireDate).HasColumnType("datetime");
    }
}
