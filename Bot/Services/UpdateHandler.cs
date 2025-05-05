using System.Text;
using Bot.ApiClients;
using Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Core.Entities.User;

namespace Bot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly MarkupDrawer _markupDrawer;
    private readonly SessionService _sessionService;
    private readonly UserApiClient _userApiClient;
    private readonly CourseApiClient _courseApiClient;
    private readonly GroupApiClient _groupApiClient;
    private readonly DepartmentApiClient _departmentApiClient;
    private readonly TeacherApiClient _teacherApiClient;
    private readonly DayApiClient _dayApiClient;
    private readonly LessonApiClient _lessonApiClient;
    private readonly LessonChangesApiClient _lessonChangesApiClient;
    
    public UpdateHandler(MarkupDrawer markupDrawer,
        SessionService sessionService,
        UserApiClient userApiClient,
        CourseApiClient courseApiClient,
        GroupApiClient groupApiClient,
        DepartmentApiClient departmentApiClient,
        TeacherApiClient teacherApiClient, 
        DayApiClient dayApiClient, 
        LessonApiClient lessonApiClient,
        LessonChangesApiClient lessonChangesApiClient)
    {
        _markupDrawer = markupDrawer;
        _sessionService = sessionService;
        _userApiClient = userApiClient;
        _courseApiClient = courseApiClient;
        _groupApiClient = groupApiClient;
        _departmentApiClient = departmentApiClient;
        _teacherApiClient = teacherApiClient;
        _dayApiClient = dayApiClient;
        _lessonApiClient = lessonApiClient;
        _lessonChangesApiClient = lessonChangesApiClient;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (update.Type != UpdateType.Message || update.Message!.Text == null)
        {
            return;
        }
        
        var userId = update.Message.From.Id;
        var session = _sessionService.GetSession(userId.ToString());
        
        switch (session.State)
        {
            case UserSessionState.None:
                await HandleMainMenuState(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingStatusForRegistration:
                await HandleChoosingUserStatus(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingCourse:
                await HandleChoosingCourseForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingGroupForRegistration:
                await HandleChoosingGroupForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingDepartmentForRegistration:
                await HandleChoosingDepartmentForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingTeacherForRegistration:
                await HandleChoosingTeacherForRegistration(botClient, update, session, cancellationToken);
                break;

            case UserSessionState.ChoosingDay:
                await HandleChoosingDayForSchedule(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingDepartmentForSchedule:
                await HandleChoosingDepartmentForSchedule(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingTeacherForSchedule:
                await HandleChoosingTeacherForSchedule(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingCourseForSchedule:
                await HandleChoosingCourseForSchedule(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingGroupForSchedule:
                await HandleChoosingGroupForSchedule(botClient, update, session, cancellationToken);
                break;
            
            default:
                await botClient.SendMessage(update.Message.Chat.Id, text: "Невідома команда.", cancellationToken: cancellationToken);
                break;
        }
    }

    public async Task HandleMainMenuState(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        var user = await _userApiClient.GetUserByTelegramIdAsync($"{update.Message.From.Id}");

        switch (update.Message.Text)
        {
            case "/start":
            {
                session.State = UserSessionState.None;

                _sessionService.RemoveSession(update.Message.From.Id.ToString());
                
                session = _sessionService.GetSession(update.Message.From.Id.ToString());

                if (user != null && user.Status != UserStatus.None)
                {
                    await botClient.SendMessage(update.Message.Chat.Id,
                        text: "Ви є в базі даних, можете продовжувати користуватися ботом",
                        replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
                
                    return;
                }

                if (user == null)
                {
                    await _userApiClient.CreateUserAsync(new User
                    {
                        TelegramId = update.Message.From.Id.ToString(),
                        UserName = update.Message.From.Username ??
                                   update.Message.From.FirstName + " " + update.Message.From.LastName,
                        Status = UserStatus.None
                    });
                }
            
                session.State = UserSessionState.ChoosingStatusForRegistration;
                await SendStatusSettings(botClient, update);
                break;
            }
            
            case "\ud83d\udcc5 Пошук за днем тижня":
                session.State = UserSessionState.ChoosingDay;
                await SendAllDays(botClient, update, cancellationToken);
                break;
            
            case "\ud83e\uddd1\u200d\ud83c\udfeb Пошук за викладачем":
                session.State = UserSessionState.ChoosingDepartmentForSchedule;
                await SendDepartmentList(botClient, update, hasMainMenuButton: true);
                break;
            
            case "#\ufe0f\u20e3 Пошук за групою":
                session.State = UserSessionState.ChoosingCourseForSchedule;
                await SendCourseList(botClient, update, hasMainMenuButton: true);
                break;
            
            case "\ud83d\udccb Розклад на сьогодні":
                var currentDay = await GetCurrentDay();
                
                await SendSchedule(botClient, currentDay.Item1, update, cancellationToken, session, user.Group?.Id, user.Teacher?.Id);;
                break;
            
            case "\ud83d\udccb Розклад на наступний день":
                var nextDay = await GetNextDay();
                
                await SendSchedule(botClient, nextDay.Item1, update, cancellationToken, session, user.Group?.Id, user.Teacher?.Id);
                break;            
            
            case "\u26a0\ufe0f Зміни на наступний день":
                var currentDayForChanges = await GetNextDay();
                
                await SendScheduleChangesForDay(botClient, update, currentDayForChanges.Item2, currentDayForChanges.Item1,cancellationToken);
                break;
            
            case "\u26a0\ufe0f Зміни на сьогодні":
                var nextDayForChanges = await GetCurrentDay();
                
                await SendScheduleChangesForDay(botClient, update, nextDayForChanges.Item2, nextDayForChanges.Item1, cancellationToken);
                break;
        }
    }

    public async Task HandleChoosingUserStatus(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        switch (update.Message.Text)
        {
            case "Студент":
                await _userApiClient.UpdateUserStatusAsync(update.Message.From.Id.ToString(), UserStatus.Student);
                session.State = UserSessionState.ChoosingCourse;
                await SendCourseList(botClient, update);
                break;
            
            case "Викладач":
                await _userApiClient.UpdateUserStatusAsync(update.Message.From.Id.ToString(), UserStatus.Teacher);
                session.State = UserSessionState.ChoosingDepartmentForRegistration;
                await SendDepartmentList(botClient, update);
                break;
            
            default:
                await botClient.SendMessage(update.Message.Chat.Id, "Оберіть статус: Студент або Викладач.", cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task HandleChoosingCourseForRegistration(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        if (int.TryParse(update.Message.Text, out int courseNumber))
        {
            var course = await _courseApiClient.GetCourseByNumberAsync(courseNumber);

            if (course == null)
            {
                await botClient.SendMessage(update.Message.Chat.Id, "Курс не знайдено.", cancellationToken: cancellationToken);
                return;
            }
            
            session.State = UserSessionState.ChoosingGroupForRegistration;
            
            await SendGroupsByCourse(botClient, update, course.Id, false);
        }
        else
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть курс зі списку.", cancellationToken: cancellationToken);
        }
    }

    private async Task HandleChoosingGroupForRegistration(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        if (int.TryParse(update.Message.Text, out int groupNumber))
        {
            var group = await _groupApiClient.GetGroupByNumber(groupNumber);

            if (group == null)
            {
                await botClient.SendMessage(update.Message.Chat.Id, "Групу не знайдено.", cancellationToken: cancellationToken);
                return;
            }
            
            session.SelectedGroup = group;

            await SendSuccessfulRegistration(botClient, session, update);
        }
        else
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть групу зі списку.", cancellationToken: cancellationToken);
        }
    }

    public async Task HandleChoosingDepartmentForRegistration(ITelegramBotClient botClient, Update update,
        UserSession session, CancellationToken cancellationToken)
    {
        var department = await _departmentApiClient.GetDepartmentByNameAsync(update.Message.Text);

        Console.WriteLine($"[HandleChoosingDepartmentForRegistration] Department by name: {department}");
        
        if (department == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть відділення зі списку.", cancellationToken: cancellationToken);
            return;
        }
        
        session.State = UserSessionState.ChoosingTeacherForRegistration;

        await SendTeachersByDepartment(botClient, update, department.Id);
    }

    private async Task HandleChoosingTeacherForRegistration(ITelegramBotClient botClient, Update update,
        UserSession session, CancellationToken cancellationToken)
    {

        Console.WriteLine("[HandleChoosingTeacher] Handle choosing teacher");
        
        var teacher = await _teacherApiClient.GetTeacherByFullName(update.Message.Text);

        if (teacher == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть викладача зі списку.", cancellationToken: cancellationToken);
            return;
        }
        
        session.SelectedTeacher = teacher;
        
        await SendSuccessfulRegistration(botClient, session, update);
    }
    
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Помилка: {exception.Message}");
        return Task.CompletedTask;
    }

    private async Task SendSchedule(ITelegramBotClient botClient, WeekDay day, Update update, CancellationToken cancellationToken, UserSession session, int? groupId = null, int? teacherId = null)
    {
        var user = await _userApiClient.GetUserByTelegramIdAsync(update.Message.From.Id.ToString());
        var lessons = await _lessonApiClient.GetLessons(groupId: groupId, teacherId: teacherId, dayId: day.Id);

        if (lessons == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text:$"Занять на {day.Name} не знайдено.", cancellationToken: cancellationToken);
            return;
        }
        
        lessons.Sort((lesson1, lesson2) => lesson1.Number.CompareTo(lesson2.Number));

        var constructedSchedule = new StringBuilder();
        
        constructedSchedule.Append($"<b>{day.Name}</b>\n");
        
        var sortedLessons = lessons.OrderBy(l => l.Number);

        switch (session.State)
        {
            case UserSessionState.None:
                SendDefaultSchedule(sortedLessons, user, constructedSchedule);
                break;
            
            case UserSessionState.ChoosingTeacherForSchedule:
                SendScheduleForSelectedTeacher(sortedLessons, user, constructedSchedule);
                break;
            
            case UserSessionState.ChoosingGroupForSchedule:
                SendScheduleForSelectedGroup(sortedLessons, user, constructedSchedule);
                break;
            
            case UserSessionState.ChoosingDay:
                SendDefaultSchedule(sortedLessons, user, constructedSchedule);
                break;
        }

        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: constructedSchedule.ToString(), cancellationToken: cancellationToken, parseMode: ParseMode.Html);
    }

    private void SendDefaultSchedule(IOrderedEnumerable<Lesson> sortedLessons, User? user, StringBuilder constructedSchedule)
    {
        foreach (var lesson in sortedLessons)
        {
            if (user.Status == UserStatus.Student)
            {
                constructedSchedule.Append($"{lesson.Number}. {lesson.Discipline.Name} \u27a1\ufe0f {lesson.Teacher.FullName} \u27a1\ufe0f ауд. {lesson.Auditorium.Number}\n");
            }
            else
            {
                constructedSchedule.Append($"{lesson.Number}. {lesson.Discipline.Name} \u27a1\ufe0f {lesson.Group.Number} група \u27a1\ufe0f ауд. {lesson.Auditorium.Number}\n");
            }
        }
    }

    private void SendScheduleForSelectedTeacher(IOrderedEnumerable<Lesson> sortedLessons, User? user, StringBuilder constructedSchedule)
    {
        foreach (var lesson in sortedLessons)
        {
            constructedSchedule.Append($"{lesson.Number}. {lesson.Discipline.Name} \u27a1\ufe0f {lesson.Group.Number} група \u27a1\ufe0f ауд. {lesson.Auditorium.Number}\n");
        }
    }
    
    private void SendScheduleForSelectedGroup(IOrderedEnumerable<Lesson> sortedLessons, User? user, StringBuilder constructedSchedule)
    {
        foreach (var lesson in sortedLessons)
        {
            constructedSchedule.Append($"{lesson.Number}. {lesson.Discipline.Name} \u27a1\ufe0f {lesson.Teacher.FullName} \u27a1\ufe0f ауд. {lesson.Auditorium.Number}\n");
        }
    }
    
    private async Task SendStatusSettings(ITelegramBotClient botClient, Update update)
    {
        await botClient.SendMessage(chatId:update.Message.Chat.Id, text: "Оберіть, хто ви:", replyMarkup: _markupDrawer.DrawStatusSettings());
    }

    private async Task SendCourseList(ITelegramBotClient botClient, Update update, bool hasMainMenuButton = false)
    {
        var courses = await _courseApiClient.GetCoursesAsync();
        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть курс:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 2, courses, hasMainMenuButton: hasMainMenuButton));
    }

    private async Task SendTeachersByDepartment(ITelegramBotClient botClient, Update update, int departmentId, bool hasMainMenuButton = false)
    {
        Console.WriteLine("[UpdateHandler] Sending teachers by department");
        
        var teachers = await _teacherApiClient.GetTeachersByDepartment(departmentId);
        
        Console.WriteLine($"[UpdateHandler] teachers by department count: {teachers.Count}");

        await botClient.SendMessage(update.Message.Chat.Id, text:"Оберіть викладача:", replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 2, teachers, hasMainMenuButton: hasMainMenuButton));
    }

    private async Task SendDepartmentList(ITelegramBotClient botClient, Update update, bool hasMainMenuButton = false)
    {
        var departments = await _departmentApiClient.GetDepartmentsAsync();
        
        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть відділення:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 3, departments, hasMainMenuButton: hasMainMenuButton));
    }

    private async Task SendSuccessfulRegistration(ITelegramBotClient botClient, UserSession session, Update update)
    {
        switch (session.State)
        {
            case UserSessionState.ChoosingGroupForRegistration:
                await _userApiClient.UpdateUserGroupAsync(update.Message.From.Id.ToString(), session.SelectedGroup.Id);
                break;
            case UserSessionState.ChoosingTeacherForRegistration:
                await _userApiClient.UpdateUserTeacherAsync(update.Message.From.Id.ToString(), session.SelectedTeacher.Id);
                break;
        }

        session.State = UserSessionState.None;
        
        _sessionService.RemoveSession(update.Message.From.Id.ToString());

        await botClient.SendMessage(update.Message.Chat.Id, text: "Дякуємо за реєстрацію!", replyMarkup: _markupDrawer.DrawMainMenu());
    }

    private async Task SendGroupsByCourse(ITelegramBotClient botClient, Update update, int courseId, bool hasMainMenuButton)
    {
        var groups = await _groupApiClient.GetGroupsByCourse(courseId);
        
        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть групу:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 5, groups, hasMainMenuButton));
    }

    private async Task HandleChoosingDepartmentForSchedule(ITelegramBotClient botClient, Update update, UserSession session,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text == "Головне меню")
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
            session.State = UserSessionState.None;
            return;
        }

        var department = await _departmentApiClient.GetDepartmentByNameAsync(update.Message.Text);
        
        if (department == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Оберіть відділення зі списку.", cancellationToken: cancellationToken);
            return;
        }
        
        session.State = UserSessionState.ChoosingTeacherForSchedule;
        
        await SendTeachersByDepartment(botClient, update, department.Id, true);
    }
    
    private async Task HandleChoosingTeacherForSchedule(ITelegramBotClient botClient, Update update, UserSession session,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text == "Головне меню")
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
            session.State = UserSessionState.None;
            return;
        }   
        
        var teacher = await _teacherApiClient.GetTeacherByFullName(update.Message.Text);

        if (teacher == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Оберіть викладача зі списку.", cancellationToken: cancellationToken);
            return;
        }

        var days = await _dayApiClient.GetDays();

        foreach (var day in days.Take(5))
        {
            await SendSchedule(botClient, day, update, cancellationToken, session, teacherId: teacher.Id);;
        }
        
        await Task.Delay(TimeSpan.FromSeconds(0.3), cancellationToken);
        
        await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
            replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
        
        session.State = UserSessionState.None;
    }

    private async Task HandleChoosingCourseForSchedule(ITelegramBotClient botClient, Update update, UserSession session,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text == "Головне меню")
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
            
            session.State = UserSessionState.None;
            return;
        }

        if (!int.TryParse(update.Message.Text, out var courseNumber))
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Оберіть курс зі списку.",
                cancellationToken: cancellationToken);
            return;
        }

            
        var course = await _courseApiClient.GetCourseByNumberAsync(courseNumber);

        if (course == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Курс не знайдено.", cancellationToken: cancellationToken);
            return;
        }
        
        session.State = UserSessionState.ChoosingGroupForSchedule;
        
        await SendGroupsByCourse(botClient, update, course.Id, true);
    }

    private async Task HandleChoosingGroupForSchedule(ITelegramBotClient botClient, Update update, UserSession session,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text == "Головне меню")
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
            session.State = UserSessionState.None;
            return;
        }

        if (!int.TryParse(update.Message.Text, out var groupNumber))
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Оберіть групу зі списку.",
                cancellationToken: cancellationToken);
            return;
        }

        var group = await _groupApiClient.GetGroupByNumber(groupNumber);

        if (group == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Групу не знайдено.", cancellationToken: cancellationToken);
            return;
        }

        var days = await _dayApiClient.GetDays();

        foreach (var day in days.Take(5))
        {
            await SendSchedule(botClient, day, update, cancellationToken, session, groupId: group.Id);;
        }
        
        await Task.Delay(TimeSpan.FromSeconds(0.3), cancellationToken);
        
        await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
            replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
        
        session.State = UserSessionState.None;
        
    }
    
    private async Task HandleChoosingDayForSchedule(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        if (update.Message?.Text == "Головне меню")
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
            session.State = UserSessionState.None;
            return;
        }
        
        var user = await _userApiClient.GetUserByTelegramIdAsync(update.Message.From.Id.ToString());
        
        var day = await _dayApiClient.GetDayByName(update.Message.Text);
        
        await SendSchedule(botClient, day, update, cancellationToken,session);
        
        await Task.Delay(TimeSpan.FromSeconds(0.3), cancellationToken);
        
        await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
            replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
        
        session.State = UserSessionState.None;
    }

    private async Task SendAllDays(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var days = await _dayApiClient.GetDays();

        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть день тижня:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 3, days?.Take(5).ToList()), cancellationToken: cancellationToken);
    }

    private async Task SendScheduleChangesForDay(ITelegramBotClient botClient, Update update, DateOnly date, WeekDay dayName,CancellationToken cancellationToken)
    {
        var user = await _userApiClient.GetUserByTelegramIdAsync(update.Message.From.Id.ToString());
        var lessonChanges = await _lessonChangesApiClient.GetChanges(groupId: user.GroupId, teacherId: user.TeacherId, date: date);

        if (lessonChanges == null)
        {
            await botClient.SendMessage(update.Message.Chat.Id, text: $"Змін на {dayName} {date.ToString("dd.MM.yyyy")} не знайдено.", cancellationToken: cancellationToken);
            return;
        }
        
        lessonChanges.Sort((change1, change2) => change1.Number.CompareTo(change2.Number));

        var constructedSchedule = new StringBuilder();
        
        constructedSchedule.Append($"Зміни на <b>{dayName} {date.ToString("dd.MM.yyyy")}</b>\n");
        
        var sortedLessons = lessonChanges.OrderBy(l => l.Number);
        
        foreach (var lesson in sortedLessons)
        {
            if (lesson.ChangeType == ChangeType.Cancelled)
            {
                constructedSchedule.Append($"{lesson.Number} пара \u27a1\ufe0f скасована\n");
                continue;
            }
            constructedSchedule.Append($"{lesson.Number} пара \u27a1\ufe0f {lesson.Discipline.Name} \u27a1\ufe0f {lesson.Teacher.FullName} \u27a1\ufe0f {lesson.Group.Number} група \u27a1\ufe0f ауд. {lesson.Auditorium.Number}\n");
        }

        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: constructedSchedule.ToString(), cancellationToken: cancellationToken, parseMode: ParseMode.Html);
    }
    
    private async Task<(WeekDay,DateOnly)> GetCurrentDay()
    {
        var days = await _dayApiClient.GetDays();
        
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        
        var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        var currentDayId = (int)time.DayOfWeek;
        
        var date = DateOnly.FromDateTime(time);

        return (days.FirstOrDefault(d => d.CodeAlias == currentDayId), date);
    }

    private async Task<(WeekDay,DateOnly)> GetNextDay()
    {
        var days = await _dayApiClient.GetDays();
        
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        
        var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(1), timeZone);

        var nextDayId = (int)time.DayOfWeek;
        
        var date = DateOnly.FromDateTime(time);

        return (days.FirstOrDefault(d => d.CodeAlias == nextDayId), date);
    }
}