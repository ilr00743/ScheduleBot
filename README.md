# Schedule Bot

A Telegram bot application for managing and providing access to academic schedules. The project consists of a Telegram bot client and a REST API backend with database management capabilities.

## Project Structure

The solution is organized into three main projects:

- **Bot** - Telegram bot application that handles user interactions
- **DataBaseApi** - REST API backend with database management and admin interface
- **Core** - Shared entities and DTOs used across the application

## Features

### Telegram Bot
- User session management
- Interactive schedule browsing
- Course, group, and teacher information
- Lesson changes notifications
- Department-based navigation

### API Backend
- RESTful API for all academic entities
- PostgreSQL database integration
- CoreAdmin interface for data management
- Authentication and authorization
- Swagger documentation

### Core Entities
- Users and UserSessions
- Departments, Courses, Groups
- Teachers, Disciplines, Auditoriums
- Lessons, LessonChanges, WeekDays

## Technology Stack

- **.NET 8** - Main framework
- **Telegram.Bot** - Telegram bot API client
- **Entity Framework Core** - ORM with PostgreSQL
- **ASP.NET Core** - Web API framework
- **CoreAdmin** - Database administration interface
- **Fly.io** - Cloud deployment platform

## Prerequisites

- .NET 8 SDK
- PostgreSQL database
- Telegram Bot Token (from @BotFather)
- Fly.io account (for deployment)

## Environment Variables

### Bot Application
```bash
BOT_TOKEN=your_telegram_bot_token
API_BASE_URL=https://your-api-domain.com
```

### DataBaseApi Application
```bash
CONNECTION_STRING=Host=your-db-host;Database=your-db-name;Username=your-username;Password=your-password
ADMIN_USERNAME=admin_username
ADMIN_PASSWORD=admin_password
```

## Getting Started

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd PIBScheduleBot
   ```

2. **Set up the database**
   - Create a PostgreSQL database
   - Update the connection string in environment variables

3. **Configure environment variables**
   - Set up the required environment variables for both projects
   - Ensure your Telegram bot token is configured

4. **Run the applications**
   ```bash
   # Run the API backend
   cd DataBaseApi
   dotnet run
   
   # Run the bot (in another terminal)
   cd Bot
   dotnet run
   ```

### Docker Deployment

The project includes Docker support for containerized deployment:

```bash
# Build and run with Docker Compose
docker-compose up --build
```

## API Endpoints

The DataBaseApi provides REST endpoints for:

- **Users** - `/api/users`
- **Groups** - `/api/groups`
- **Courses** - `/api/courses`
- **Teachers** - `/api/teachers`
- **Departments** - `/api/departments`
- **Lessons** - `/api/lessons`
- **Lesson Changes** - `/api/lesson-changes`
- **Days** - `/api/days`
- **Auditoriums** - `/api/auditoriums`
- **Disciplines** - `/api/disciplines`

### Swagger Documentation
Access the API documentation at `/swagger` when running in development mode.

## Admin Interface

The application includes a CoreAdmin interface for database management:

- Access via `/coreadmin` endpoint
- Protected by authentication
- Manage all entities through a web interface
- Configure admin credentials via environment variables

## Deployment

### Fly.io Deployment

The project is configured for deployment on Fly.io:

1. **Install Fly CLI**
   ```bash
   curl -L https://fly.io/install.sh | sh
   ```

2. **Deploy the API**
   ```bash
   cd DataBaseApi
   fly deploy
   ```

3. **Deploy the Bot**
   ```bash
   cd Bot
   fly deploy
   ```

4. **Set environment variables**
   ```bash
   fly secrets set CONNECTION_STRING="your-connection-string"
   fly secrets set ADMIN_USERNAME="your-admin-username"
   fly secrets set ADMIN_PASSWORD="your-admin-password"
   fly secrets set BOT_TOKEN="your-bot-token"
   fly secrets set API_BASE_URL="https://your-api-domain.com"
   ```

## Project Structure Details

```
PIBScheduleBot/
├── Bot/                    # Telegram bot application
│   ├── ApiClients/        # HTTP clients for API communication
│   ├── Services/          # Bot services and handlers
│   ├── Program.cs         # Bot entry point
│   └── Dockerfile         # Bot container configuration
├── DataBaseApi/           # REST API backend
│   ├── Controllers/       # API controllers
│   ├── Persistence/       # Database context and migrations
│   ├── Program.cs         # API entry point
│   └── Dockerfile         # API container configuration
├── Core/                  # Shared domain models
│   ├── Entities/          # Database entities
│   └── DTO/              # Data transfer objects
├── fly.toml              # Fly.io deployment configuration
└── PIBScheduleBot.sln    # Solution file
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation at `/swagger`
