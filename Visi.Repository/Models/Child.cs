using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Visi.Repository.Models;

[Table("CHILD", Schema = "RICAP")]
public class Child
{
    [Column("CHILD_ID")] public ulong? ChildId { get; init; }

    [Column("FIRST_NAME")] [MaxLength(50)] public string FirstName { get; init; }

    [Column("LAST_NAME")] [MaxLength(50)] public string LastName { get; init; }

    [Column("BIRTH_DATE_TIME")] public DateTime BirthDateTime { get; init; }

    [Column("SEX")] public char Sex { get; init; }
}