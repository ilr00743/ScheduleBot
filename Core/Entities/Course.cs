using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Course
{
    [Key]
    public int Id { get; set; }
    public required int Number { get; set; }

    public List<Group> Groups { get; set; } = new();

    public override string ToString()
    {
        return Number.ToString();
    }
}