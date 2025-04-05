using Core.Entities;

namespace Core.DTO;

public class UpdateCourseRequest
{
    public int? Number { get; set; }
    public List<Group>? Groups { get; set; }
}