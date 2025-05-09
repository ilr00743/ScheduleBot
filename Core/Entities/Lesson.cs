using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Lesson
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public int Number { get; set; }
    
    public int? DisciplineId { get; set; }
    
    [ForeignKey(nameof(DisciplineId))]
    public Discipline? Discipline { get; set; }
    
    public int? TeacherId { get; set; }
    
    [ForeignKey(nameof(TeacherId))]
    public Teacher? Teacher { get; set; }
    
    public int? GroupId { get; set; }
    
    [ForeignKey(nameof(GroupId))]
    public Group? Group { get; set; }
    
    public int? AuditoriumId { get; set; }
    
    [ForeignKey(nameof(AuditoriumId))]
    public Auditorium? Auditorium { get; set; }
    
    public int? DayId { get; set; }
    
    [ForeignKey(nameof(DayId))]
    public WeekDay? Day { get; set; }
}