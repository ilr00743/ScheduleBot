namespace Core.Entities;

public class Lesson
{
    public int Id { get; set; }
    public int Number { get; set; }
    
    public int DisciplineId { get; set; }
    public Discipline? Discipline { get; set; }
    
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    
    public int AuditoriumId { get; set; }
    public Auditorium? Auditorium { get; set; }
    
    public required DayOfWeek Day { get; set; }
}