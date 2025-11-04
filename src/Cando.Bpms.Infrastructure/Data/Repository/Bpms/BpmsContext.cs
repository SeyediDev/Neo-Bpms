using Neo.Domain.Entities.Common;
using Neo.Domain.Repository;
using Neo.Infrastructure.Data.Repository.Ef;
using Microsoft.EntityFrameworkCore;

namespace Neo.Bpms.Infrastructure.Data.Repository.Bpms;

public abstract partial class BpmsContext<TContext>(DbContextOptions<TContext> options)
    : EfDbContext<TContext>(options), IUnitOfWork
    where TContext : DbContext
{
    public virtual DbSet<Setting> Settings { get; set; }
    public virtual DbSet<SubjectSetting> SubjectSettings { get; set; }

//    public virtual DbSet<CultureTerm> CultureTerms { get; set; }
//    public virtual DbSet<Language> Language { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
