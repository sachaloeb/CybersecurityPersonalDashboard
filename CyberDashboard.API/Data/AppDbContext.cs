using Microsoft.EntityFrameworkCore;
//using CyberDashboard.API.Models;
public class AppDbContext : DbContext
{
    public DbSet<SshAttemptLog> SshAttemptLogs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}