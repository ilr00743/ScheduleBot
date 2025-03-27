namespace Core.Entities;

public class Course
{
    public required int Id { get; set; }
    public required int Number { get; set; }

    public List<Group> Groups { get; set; } = new();
}