namespace Core.Entities;

public class UserSession
{
    public UserSessionState State { get; set; }
    public Course? SelectedCourse { get; set; }
    public Group? SelectedGroup { get; set; }
    public Department? SelectedDepartment { get; set; }
    public Teacher? SelectedTeacher { get; set; }
}

public enum UserSessionState
{
    None = 0,
    ChoosingCourse = 1,
    ChoosingGroup = 2,
    ChoosingDepartment = 3,
    ChoosingTeacher = 4,
    ChoosingStatus = 5
}