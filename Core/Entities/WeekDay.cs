using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class WeekDay
{
    [Key, Editable(false)]
    public int Id { get; set; }
    public string Name { get; set; }

    [Display(AutoGenerateField = false)]
    public int CodeAlias  { get; set; }

    public override string ToString()
    {
        return Name;
    }
}