namespace Core.Entities;

public class Discipline
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<TeacherDiscipline> TeacherDisciplines { get; set; } = new();
}