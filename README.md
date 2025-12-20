# Fitness Center Management System

ASP.NET Core MVC project for Web Programming course - Fitness Center Management and Appointment System.

## Project Overview

This project is a comprehensive Fitness Center Management System that allows gyms to manage their services, trainers, and appointments. Members can book appointments with trainers, and administrators can approve or reject appointments.

## Features

### Core Functionality
- ✅ **Gym Management**: Create, read, update, and delete gym information
- ✅ **Trainer Management**: Manage trainers with their specialties and services
- ✅ **Service Management**: Define gym services with duration and pricing
- ✅ **Appointment System**: Members can book appointments with trainers
- ✅ **Appointment Approval**: Admin can approve/reject pending appointments
- ✅ **Conflict Detection**: System prevents overlapping appointments

### Technical Features
- ✅ **REST API**: RESTful API endpoints with LINQ filtering
  - `/api/TrainersApi` - Get trainers with filtering (gym, specialty, search)
  - `/api/TrainersApi/available` - Get available trainers for a service
  - `/api/AppointmentsApi/mine` - Get user's appointments with date/status filtering
  - `/api/AppointmentsApi/trainer/{id}` - Get trainer's appointments

- ✅ **AI Integration**: AI-powered workout and diet recommendations
  - Personalized exercise plans based on age, height, weight, goals
  - Nutrition recommendations
  - Progress tracking suggestions
  - OpenAI API integration (with fallback)

- ✅ **User Authentication**: ASP.NET Core Identity
  - User registration and login
  - Role-based authorization (Admin, Member)

- ✅ **Data Validation**: Client and server-side validation
  - Model validation attributes
  - jQuery validation

- ✅ **Modern UI**: Bootstrap 5 with responsive design
  - Card-based layouts
  - Bootstrap Icons
  - Modern navigation

## Technology Stack

- **Backend**: ASP.NET Core MVC 9.0
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript, jQuery
- **AI**: OpenAI API (with fallback implementation)
- **ORM**: Entity Framework Core with LINQ

## Database Models

- `Gym`: Spor salonu bilgileri
- `GymService`: Salon hizmetleri (fitness, yoga, pilates vb.)
- `Trainer`: Antrenör bilgileri
- `TrainerService`: Antrenör-hizmet ilişkisi
- `TrainerAvailability`: Antrenör müsaitlik saatleri
- `Appointment`: Randevu kayıtları
- `ApplicationUser`: Kullanıcı bilgileri (Identity)

## Default Admin Account

- **Email**: ogrencinumarasi@sakarya.edu.tr
- **Password**: sau
- **Role**: Admin

## API Endpoints

### Trainers API
- `GET /api/TrainersApi` - List all trainers (with optional filters: gymId, specialty, search)
- `GET /api/TrainersApi/available?serviceId={id}&date={date}` - Get available trainers for a service
- `GET /api/TrainersApi/{id}` - Get trainer details

### Appointments API
- `GET /api/AppointmentsApi/mine` - Get current user's appointments (with optional filters: status, fromDate, toDate)
- `GET /api/AppointmentsApi/trainer/{trainerId}` - Get trainer's appointments (with optional date filter)

## AI Feature

Access the AI workout recommendation feature at `/AIWorkout`. Users can input:
- Age, height, weight
- Gender
- Fitness goal (weight loss, muscle gain, etc.)
- Activity level
- Health conditions (optional)

The system generates personalized workout and nutrition plans.

## Setup Instructions

1. Clone the repository
2. Update `appsettings.json` with your SQL Server connection string
3. Run migrations: `Update-Database`
4. (Optional) Add OpenAI API key to `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-api-key-here"
     }
   }
   ```
5. Run the application

## Project Structure

```
FitnessCenter.Web/
├── Controllers/
│   ├── Api/              # REST API controllers
│   ├── Admin/             # Admin area controllers
│   └── ...                # MVC controllers
├── Models/                # Entity models
├── ViewModels/            # View models
├── Views/                 # Razor views
├── Data/                  # DbContext and migrations
└── wwwroot/               # Static files
```

## Commit History

The project has been developed with regular commits from 15/12/2025 onwards, ensuring proper version control and project tracking.

## License

This project is developed for educational purposes as part of the Web Programming course.
