using System.Text.Json.Serialization;

namespace Core.Entities;

public class Group
{
    public required int Id { get; set; }
    public required int Number { get; set; }
    
    public int? CourseId { get; set; }
    
    [JsonIgnore]
    public Course? Course { get; set; }

    public override string ToString()
    {
        return Number.ToString();
    }
}