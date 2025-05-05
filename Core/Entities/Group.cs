using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Entities;

public class Group
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public int Number { get; set; }
    
    public int? CourseId { get; set; }
    
    [JsonIgnore]
    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    public override string ToString()
    {
        return Number.ToString();
    }
}