# MongoDB WebAPI Solution

A production-ready Visual Studio 2022 WebAPI solution based on Clean Architecture and SOLID principles, featuring MongoDB integration, SignalR real-time notifications, and Change Streams.

## Solution Structure

```
MongoDB.sln
├── MongoDB.API/                 # Web API layer (Controllers, SignalR Hub, Swagger)
├── MongoDB.Application/         # Application layer (DTOs, Services, Validators)
├── MongoDB.Domain/             # Domain layer (Entities, Interfaces)
├── MongoDB.Infrastructure/     # Infrastructure layer (MongoDB access, Change Streams)
└── MongoDB.Tests/             # Test layer (Unit and Integration tests)
```

## Features

- ✅ **Clean Architecture** with proper separation of concerns
- ✅ **SOLID Principles** implementation
- ✅ **MongoDB Integration** with generic repository pattern
- ✅ **SignalR Real-time Notifications** for client registration and updates
- ✅ **MongoDB Change Streams** for background monitoring
- ✅ **RESTful API** with comprehensive endpoints
- ✅ **Swagger Documentation** with detailed API docs
- ✅ **FluentValidation** for input validation
- ✅ **AutoMapper** for object mapping
- ✅ **Comprehensive Testing** with xUnit
- ✅ **Generic Collection Access** for any document type

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022
- MongoDB (local or cloud instance)

## Getting Started

### 1. Database Setup

Ensure MongoDB is running on your system:
```bash
# Using Docker
docker run -d -p 27017:27017 --name mongodb mongo:latest

# Or install MongoDB locally
# Follow instructions at https://docs.mongodb.com/manual/installation/
```

### 2. Configuration

Update the connection string in `appsettings.json`:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MongoDBSamples",
    "CollectionName": "samples"
  }
}
```

### 3. Running the Application

1. Open `MongoDB.sln` in Visual Studio 2022
2. Set `MongoDB.API` as the startup project
3. Press F5 or Ctrl+F5 to run
4. Navigate to `https://localhost:7000` for Swagger UI

### 4. Testing

Run the test suite:
```bash
dotnet test
```

## API Endpoints

### Samples Controller (`/api/samples`)

- `GET /api/samples` - Get all samples
- `GET /api/samples/{id}` - Get sample by ID
- `GET /api/samples/by-name/{name}` - Get samples by name
- `GET /api/samples/by-date-range?startDate={date}&endDate={date}` - Get samples by date range
- `GET /api/samples/paged?page={page}&pageSize={size}` - Get paged samples
- `GET /api/samples/count` - Get total sample count
- `POST /api/samples` - Create new sample
- `PUT /api/samples/{id}` - Update sample
- `DELETE /api/samples/{id}` - Delete sample

### Notifications Controller (`/api/notifications`)

- `GET /api/notifications/clients` - Get registered clients
- `POST /api/notifications/broadcast` - Broadcast to all clients
- `POST /api/notifications/notify-client` - Notify specific client

### SignalR Hub (`/notificationHub`)

- `RegisterClient(clientName)` - Register for notifications
- `UnregisterClient()` - Unregister from notifications

## Sample Document Schema

```json
{
  "id": "ObjectId",
  "date": "DateTime",
  "Name": "string",
  "Hash": "double",
  "LC_FC": "int",
  "sample": [
    {
      "Data": "float",
      "Vahicle Date": "DateTime",
      "recieved Date": "DateTime"
    }
  ]
}
```

## Real-time Features

### Client Registration
Clients can register for real-time notifications by connecting to the SignalR hub and calling the `RegisterClient` method with their name.

### Change Stream Monitoring
The application automatically monitors MongoDB change streams and pushes updates to registered clients when:
- New samples are added
- Existing samples are updated
- Samples are deleted

## Architecture Benefits

- **Testability**: Each layer can be tested independently
- **Maintainability**: Clear separation of concerns
- **Scalability**: Easy to add new features or modify existing ones
- **Flexibility**: Generic repository pattern supports any document type
- **Real-time**: SignalR provides instant updates to connected clients

## Technologies Used

- .NET 8.0
- ASP.NET Core Web API
- MongoDB Driver
- SignalR
- AutoMapper
- FluentValidation
- xUnit
- Swagger/OpenAPI

## License

This project is provided as-is for educational and development purposes.