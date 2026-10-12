using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(e =>
        {
            e.ToTable("tarefas");
            e.HasKey(t => t.Id);
            e.Property(t => t.Titulo).IsRequired().HasMaxLength(150);
            e.Property(t => t.Descricao).HasMaxLength(1000);
            e.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        });
    }
}
