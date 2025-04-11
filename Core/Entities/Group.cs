using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Entities;

public class Group
{
    [Key]
    public required int Id { get; set; }
    public required int Number { get; set; }
    
    [Display(AutoGenerateField = false)]
    public int? CourseId { get; set; }
    
    [JsonIgnore]
    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    public override string ToString()
    {
        return Number.ToString();
    }
}