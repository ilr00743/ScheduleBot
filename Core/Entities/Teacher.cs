using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Teacher
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public required string FullName { get; set; }
    
    [Display(AutoGenerateField = false)]
    public int? DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; }
    
    public override string ToString()
    {
        return FullName;
    }
}