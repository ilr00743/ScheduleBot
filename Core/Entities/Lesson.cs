namespace Core.Entities;

public class Lesson
{
    public int Id { get; set; }
    public int Number { get; set; }
    
    public int DisciplineId { get; set; }
    public required Discipline Discipline { get; set; }
    
    public int TeacherId { get; set; }
    public required Teacher Teacher { get; set; }
    
    public int AuditoriumId { get; set; }
    public required Auditorium Auditorium { get; set; }
}