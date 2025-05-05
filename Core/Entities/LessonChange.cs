using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class LessonChange
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public int Number { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly Date { get; set; }
    
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
    
    public ChangeType ChangeType { get; set; }
}

public enum ChangeType
{
    Added = 0,
    Cancelled = 1,
    Replaced = 2
}