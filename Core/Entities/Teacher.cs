namespace Core.Entities;

public class Teacher
{
    public required int Id { get; set; }
    public required string FullName { get; set; }
    
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
}