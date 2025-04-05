using System.Data.Common;
using Core.Entities;
using PIBScheduleBot.ApiClients;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Core.Entities.User;

namespace PIBScheduleBot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly MarkupDrawer _markupDrawer;
    private readonly SessionService _sessionService;
    private readonly UserApiClient _userApiClient;
    private readonly CourseApiClient _courseApiClient;
    private readonly GroupApiClient _groupApiClient;
    private readonly DepartmentApiClient _departmentApiClient;
    private readonly TeacherApiClient _teacherApiClient;
    
    public UpdateHandler(MarkupDrawer markupDrawer,
        SessionService sessionService,
        UserApiClient userApiClient,
        CourseApiClient courseApiClient,
        GroupApiClient groupApiClient,
        DepartmentApiClient departmentApiClient,
        TeacherApiClient teacherApiClient)
    {
        _markupDrawer = markupDrawer;
        _sessionService = sessionService;
        _userApiClient = userApiClient;
        _courseApiClient = courseApiClient;
        _groupApiClient = groupApiClient;
        _departmentApiClient = departmentApiClient;
        _teacherApiClient = teacherApiClient;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (update.Type != UpdateType.Message || update.Message!.Text == null)
        {
            return;
        }

        var message = update.Message.Text;
        var userId = update.Message.From.Id;
        var session = _sessionService.GetSession(userId.ToString());
        
        switch (session.State)
        {
            case UserSessionState.None:
                await HandleDefaultState(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingStatus:
                await HandleChoosingUserStatus(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingCourse:
                await HandleChoosingCourseForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingGroup:
                await HandleChoosingGroupForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingDepartment:
                await HandleChoosingDepartmentForRegistration(botClient, update, session, cancellationToken);
                break;
            
            case UserSessionState.ChoosingTeacher:
                await HandleChoosingTeacherForRegistration(botClient, update, session, cancellationToken);
                break;
            
            default:
                await botClient.SendMessage(update.Message.Chat.Id, text: "Невідома команда.");
                break;
        }
        
        switch (update.Message.Text)
        {
            case "\ud83d\udccb Розклад на сьогодні":
                await SendScheduleForToday(botClient, update);
                break;

            case "\ud83d\udccb Розклад на наступний день":
                Console.WriteLine("Розклад на наступний день");
                break;

            case "\u26a0\ufe0f Зміни на сьогодні":
                Console.WriteLine("Зміни на сьогодні");
                break;

            case "\u26a0\ufe0f Зміни на наступний день":
                Console.WriteLine("Зміни на наступний день");
                break;

            case "#\ufe0f\u20e3 Пошук за групою":
                //await SendFindByGroup(botClient, update);
                break;

            case "\ud83e\uddd1\u200d\ud83c\udfeb Пошук за викладачем":
                break;

            case "\ud83d\udcc5 Пошук за днем тижня":
                break;
            case "Головне меню":
                await botClient.SendMessage(update.Message.Chat.Id, text: "Повертаємось до головного меню",
                    replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
                break;
        }
    }

    public async Task HandleDefaultState(ITelegramBotClient botClient, Update update, UserSession session, CancellationToken cancellationToken)
    {
        if (update.Message.Text == "/start")
        {
            session.State = UserSessionState.None;

            _sessionService.RemoveSession(update.Message.From.Id.ToString());
            
            var user = await _userApiClient.GetUserByTelegramIdAsync($"{update.Message.From.Id}");
            
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
            
            session.State = UserSessionState.ChoosingStatus;
            await SendStatusSettings(botClient, update);
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
                session.State = UserSessionState.ChoosingDepartment;
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
                await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть курс зі списку.", cancellationToken: cancellationToken);
                return;
            }
            
            session.SelectedCourse = course;
            session.State = UserSessionState.ChoosingGroup;
            
            await SendGroupsByCourse(botClient, update, session.SelectedCourse.Id, false);
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
                await botClient.SendMessage(update.Message.Chat.Id, "Будь ласка, оберіть групу зі списку.", cancellationToken: cancellationToken);
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
        
        session.SelectedDepartment = department;
        session.State = UserSessionState.ChoosingTeacher;

        await SendTeachersByDepartment(botClient, update, session.SelectedDepartment.Id);
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

    private async Task SendScheduleForToday(ITelegramBotClient botClient, Update update)
    {
        await botClient.SendMessage(chatId:update.Message.Chat.Id, text: "Розклад на сьогодні");

        await botClient.SendMessage(chatId: update.Message.Chat.Id,
            text: $"Username: {update.Message.From.Username}, TelegramId: {update.Message.From.Id}");
    }

    private async Task SendStatusSettings(ITelegramBotClient botClient, Update update)
    {
        await botClient.SendMessage(chatId:update.Message.Chat.Id, text: "Оберіть, хто ви:", replyMarkup: _markupDrawer.DrawStatusSettings());
    }

    private async Task SendCourseList(ITelegramBotClient botClient, Update update)
    {
        var courses = await _courseApiClient.GetCoursesAsync();
        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть курс:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 2, courses));
    }

    private async Task SendTeachersByDepartment(ITelegramBotClient botClient, Update update, int departmentId)
    {
        Console.WriteLine("[UpdateHandler] Sending teachers by department");
        
        var teachers = await _teacherApiClient.GetTeachersByDepartment(departmentId);
        
        Console.WriteLine($"[UpdateHandler] teachers by department count: {teachers.Count}");

        await botClient.SendMessage(update.Message.Chat.Id, text:"Оберіть викладача:", replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 3, teachers, hasMainMenuButton: false));
    }

    private async Task SendDepartmentList(ITelegramBotClient botClient, Update update)
    {
        var departments = await _departmentApiClient.GetDepartmentsAsync();
        
        await botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть відділення:",
            replyMarkup: _markupDrawer.DrawCustomMarkup(buttonsPerRow: 3, departments, hasMainMenuButton: false));
    }

    private async Task SendSuccessfulRegistration(ITelegramBotClient botClient, UserSession session, Update update)
    {
        switch (session.State)
        {
            case UserSessionState.ChoosingGroup:
                await _userApiClient.UpdateUserGroupAsync(update.Message.From.Id.ToString(), session.SelectedGroup.Id);
                break;
            case UserSessionState.ChoosingTeacher:
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
}