namespace Core.Entities;

public class Discipline
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}