namespace Core.Entities;

public class UserSession
{
    public UserSessionState State { get; set; }
    public Group? SelectedGroup { get; set; }
    public Teacher? SelectedTeacher { get; set; }
}

public enum UserSessionState
{
    None = 0,
    ChoosingCourse = 1,
    ChoosingGroupForRegistration = 2,
    ChoosingDepartmentForRegistration = 3,
    ChoosingTeacherForRegistration = 4,
    ChoosingStatusForRegistration = 5,
    ChoosingDay = 6,
    ChoosingTeacherForSchedule = 7,
    ChoosingDepartmentForSchedule = 8,
    ChoosingGroupForSchedule = 9,
    ChoosingCourseForSchedule = 10
}