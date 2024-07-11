using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Visi.Repository.Models;

[Table("CHILD", Schema = "RICAP")]
public class Child
{
    [Column("CHILD_ID")] public int? ChildId { get; set; }

    [Column("FIRST_NAME")] public string FirstName { get; set; }

    [Column("LAST_NAME")] public string LastName { get; set; }

    [Column("BIRTH_DATE_TIME")] public DateTime BirthDateTime { get; set; }

    [Column("SEX")] public char Sex { get; set; }
}