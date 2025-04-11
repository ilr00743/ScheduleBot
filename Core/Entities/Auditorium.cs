using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Auditorium
{
    [Key]
    public int Id { get; set; }
    public int Number { get; set; }

    public override string ToString()
    {
        return Number.ToString();
    }
}