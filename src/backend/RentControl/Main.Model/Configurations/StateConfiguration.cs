using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.HasIndex(s => s.Code).IsUnique();
        builder.Property(s => s.Code).HasMaxLength(50);
        builder.Property(s => s.Name).HasMaxLength(50);
        builder.Property(s => s.Id).HasConversion<int>();

        builder.HasData([
                new State { Id = StateEnum.BROKEN, Code = "BROKEN", Name = "Broken" },
                new State { Id = StateEnum.EXCELLENT, Code = "EXCELLENT", Name = "Excellent" },
                new State { Id = StateEnum.GOOD, Code = "GOOD", Name = "Good" },
                new State { Id = StateEnum.NEEDS_MAINTENANCE, Code = "NEEDS_MAINTENANCE", Name = "Needs maintenance" },
                new State { Id = StateEnum.NEEDS_REPAIR, Code = "NEEDS_REPAIR", Name = "Needs repair" },
                new State { Id = StateEnum.NEW, Code = "NEW", Name = "New" },
                new State { Id = StateEnum.USED, Code = "USED", Name = "Used" },
                new State { Id = StateEnum.WRITE_OFF, Code = "WRITE_OFF", Name = "Write off" }
        ]);
    }
}
