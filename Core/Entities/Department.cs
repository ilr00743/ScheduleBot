using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Department
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public string? Name { get; set; }
    
    [Display(AutoGenerateField = false)]
    public List<Teacher>? Teachers { get; set; }

    public override string? ToString()
    {
        return Name;
    }
}