namespace Core.Entities;

public class Department
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<Teacher>? Teachers { get; set; }
}