namespace Core.Entities;

public class Course
{
    public int Id { get; set; }
    public required int Number { get; set; }

    public List<Group> Groups { get; set; } = new();

    public override string ToString()
    {
        return Number.ToString();
    }
}