using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Course
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public int Number { get; set; }

    [Display(AutoGenerateField = false)]
    public List<Group> Groups { get; set; } = new();

    public override string ToString()
    {
        return Number.ToString();
    }
}