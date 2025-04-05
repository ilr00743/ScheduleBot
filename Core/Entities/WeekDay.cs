namespace Core.Entities;

public class WeekDay
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CodeAlias  { get; set; }

    public override string ToString()
    {
        return Name;
    }
}