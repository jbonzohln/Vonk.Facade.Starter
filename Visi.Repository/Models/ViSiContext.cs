using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Visi.Repository.Models;

public partial class ViSiContext : DbContext
{
    private readonly IOptions<DbOptions> _dbOptionsAccessor;

    public ViSiContext(IOptions<DbOptions> dbOptionsAccessor)
    {
        _dbOptionsAccessor = dbOptionsAccessor;
    }

    public virtual DbSet<Child> Child { get; set; }

    // public virtual DbSet<ViSiPatient> Patient { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // optionsBuilder.UseSqlServer(_dbOptionsAccessor.Value.ConnectionString);
            optionsBuilder.UseOracle(_dbOptionsAccessor.Value.ConnectionString);
        }
    }
}

[Table("CHILD", Schema = "RICAP")]
public partial class Child
{
    [Column("CHILD_ID")] public int? ChildId { get; set; }

    [Column("FIRST_NAME")] public string FirstName { get; set; }

    [Column("LAST_NAME")] public string LastName { get; set; }

    [Column("BIRTH_DATE_TIME")] public DateTime BirthDateTime { get; set; }

    [Column("SEX")] public char Sex { get; set; }
}