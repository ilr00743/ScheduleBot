namespace Core.Entities;

public class Teacher
{
    public required int Id { get; set; }
    public required string FullName { get; set; }
    
    public required int DepartmentId { get; set; }
    public required Department Department { get; set; }

    public List<TeacherDiscipline> TeacherDisciplines { get; set; } = new();
}