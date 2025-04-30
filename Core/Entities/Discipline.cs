using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Discipline
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public required int Id { get; set; }
    public required string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}