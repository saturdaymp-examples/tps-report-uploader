using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SaturdayMP.Examples.TpsReportUploader.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ReportProcessRunItem> ReportProcessRunItems { get; set; }
    public DbSet<ReportProcessRun> ReportProcessRuns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ReportProcessRunItem>().ToTable("ReportProcessRunItems");
        modelBuilder.Entity<ReportProcessRun>().ToTable("ReportProcessRuns");
    }
}
