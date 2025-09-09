# MongoDB Generic API

A production-ready Visual Studio 2022 WebAPI solution built with Clean Architecture and SOLID principles, featuring MongoDB integration, SignalR real-time notifications, and Change Streams support.

## 🏗️ Architecture

This solution follows Clean Architecture principles with the following project structure:

- **MongoDB.API** - Web API layer with controllers, SignalR hub, and Swagger documentation
- **MongoDB.Application** - Business logic, DTOs, services, and use cases
- **MongoDB.Domain** - Entities, interfaces, and domain models
- **MongoDB.Infrastructure** - Data access, repositories, and external service implementations
- **MongoDB.Tests** - Comprehensive unit and integration tests

## 🚀 Features

### Core Features
- **Clean Architecture** - Separation of concerns with dependency inversion
- **SOLID Principles** - Maintainable and extensible codebase
- **Generic Repository Pattern** - Flexible data access for any collection
- **RESTful API** - Full CRUD operations with proper HTTP status codes
- **Comprehensive Error Handling** - Global exception middleware
- **Input Validation** - FluentValidation for request validation
- **AutoMapper** - Object-to-object mapping
- **Structured Logging** - Serilog with file and console outputs

### MongoDB Features
- **Generic Collection Access** - Work with any document type in any collection
- **Change Streams** - Real-time monitoring of database changes
- **Connection Management** - Configurable timeouts and connection settings
- **BSON Serialization** - Proper MongoDB document handling

### SignalR Features
- **Real-time Notifications** - Instant updates when new samples are added
- **Client Registration** - Register clients by name for targeted notifications
- **Connection Management** - Automatic client registration/deregistration
- **Activity Tracking** - Monitor client activity and last connection times

### API Documentation
- **Swagger UI** - Interactive API documentation
- **OpenAPI Specification** - Complete API documentation
- **XML Comments** - Detailed endpoint descriptions
- **Response Examples** - Clear API response formats

## 📋 Sample Document Schema

The API works with the following sample document structure:

```json
{
  "_id": "ObjectId",
  "date": "DateTime",
  "name": "string",
  "hash": "double",
  "lc_fc": "int",
  "sample": [
    {
      "data": "float",
      "vahicle_date": "DateTime",
      "recieved_date": "DateTime"
    }
  ],
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

## 🛠️ Prerequisites

- **.NET 8.0 SDK** or later
- **MongoDB** - Local instance or MongoDB Atlas
- **Visual Studio 2022** (recommended) or VS Code
- **MongoDB Compass** (optional, for database management)

## ⚙️ Configuration

### MongoDB Settings

Update `appsettings.json` with your MongoDB configuration:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MongoDbGenericAPI",
    "ConnectionTimeoutSeconds": 30,
    "SocketTimeoutSeconds": 30,
    "EnableChangeStreams": true
  }
}
```

### Environment-Specific Settings

- **Development**: Uses `appsettings.Development.json`
- **Production**: Uses `appsettings.json`

## 🚀 Getting Started

### 1. Setup MongoDB

**Option A: Local MongoDB**
```bash
# Install MongoDB Community Edition
# Start MongoDB service
mongod --dbpath /path/to/your/db
```

**Option B: MongoDB Atlas**
```bash
# Create a free cluster at https://www.mongodb.com/atlas
# Get connection string and update appsettings.json
```

### 2. Clone and Build

```bash
# Navigate to the solution directory
cd /path/to/MongoDB.sln

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build
```

### 3. Run the API

```bash
# Run the API project
cd src/MongoDB.API
dotnet run

# Or use Visual Studio 2022
# Open MongoDB.sln and press F5
```

### 4. Access the API

- **Swagger UI**: http://localhost:5000 (or https://localhost:5001)
- **API Base URL**: http://localhost:5000/api
- **SignalR Hub**: http://localhost:5000/notificationHub
- **Health Check**: http://localhost:5000/health

## 📡 API Endpoints

### Samples API
- `GET /api/samples` - Get all samples (paginated)
- `GET /api/samples/{id}` - Get sample by ID
- `GET /api/samples/by-name/{name}` - Get samples by name
- `GET /api/samples/by-date-range` - Get samples by date range
- `GET /api/samples/latest` - Get latest sample
- `POST /api/samples` - Create new sample
- `PUT /api/samples/{id}` - Update sample
- `DELETE /api/samples/{id}` - Delete sample

### Generic Documents API
- `GET /api/collections/{collectionName}/documents` - Get all documents
- `GET /api/collections/{collectionName}/documents/{id}` - Get document by ID
- `POST /api/collections/{collectionName}/documents` - Create document
- `PUT /api/collections/{collectionName}/documents/{id}` - Update document
- `DELETE /api/collections/{collectionName}/documents/{id}` - Delete document
- `GET /api/collections/{collectionName}/documents/count` - Get document count

### SignalR Clients API
- `GET /api/signalrclients/active` - Get active clients
- `GET /api/signalrclients/by-name/{name}` - Get client by name
- `POST /api/signalrclients/register` - Register client (testing only)
- `DELETE /api/signalrclients/unregister/{connectionId}` - Unregister client
- `PUT /api/signalrclients/activity/{connectionId}` - Update client activity

## 🔌 SignalR Integration

### JavaScript Client Example

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

// Start connection
connection.start().then(function () {
    console.log("Connected to SignalR hub");
    
    // Register client
    connection.invoke("RegisterClient", "MyClientName");
}).catch(function (err) {
    console.error(err.toString());
});

// Listen for new samples
connection.on("NewSample", function (sample) {
    console.log("New sample received:", sample);
    // Handle new sample notification
});

// Listen for general notifications
connection.on("Notification", function (message, data) {
    console.log("Notification:", message, data);
});

// Listen for registration confirmation
connection.on("ClientRegistered", function (message) {
    console.log("Registration:", message);
});
```

### .NET Client Example

```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/notificationHub")
    .Build();

// Handle new samples
connection.On<object>("NewSample", (sample) =>
{
    Console.WriteLine($"New sample received: {sample}");
});

// Start connection and register
await connection.StartAsync();
await connection.InvokeAsync("RegisterClient", "MyClientName");
```

## 🧪 Testing

### Run Unit Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/MongoDB.Tests/MongoDB.Tests.csproj
```

### Test Categories

- **Unit Tests** - Service layer, validators, and business logic
- **Integration Tests** - Repository and database operations
- **Entity Tests** - Domain model validation

### Test Coverage

The test suite includes:
- ✅ Domain entity tests
- ✅ Service layer tests with mocking
- ✅ Validation tests
- ✅ Repository integration tests
- ✅ Mapping tests

## 📝 Logging

The application uses Serilog for structured logging:

- **Console Output** - Development and debugging
- **File Output** - `logs/mongodb-api-{date}.txt`
- **Log Levels** - Information, Warning, Error, Debug
- **Structured Data** - JSON-formatted log entries

## 🔧 Development

### Adding New Entities

1. Create entity in `MongoDB.Domain/Entities`
2. Add repository interface in `MongoDB.Domain/Interfaces`
3. Implement repository in `MongoDB.Infrastructure/Repositories`
4. Create DTOs in `MongoDB.Application/DTOs`
5. Add service interface and implementation
6. Create controller in `MongoDB.API/Controllers`
7. Add validation rules
8. Write comprehensive tests

### Custom Collections

The generic document API allows working with any collection:

```bash
# Create document in custom collection
POST /api/collections/mycollection/documents
{
  "customField": "value",
  "anotherField": 123
}
```

## 🚀 Deployment

### Docker Support

```dockerfile
# Example Dockerfile for containerization
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MongoDB.API/MongoDB.API.csproj", "src/MongoDB.API/"]
# ... copy other projects
RUN dotnet restore "src/MongoDB.API/MongoDB.API.csproj"
COPY . .
RUN dotnet build "src/MongoDB.API/MongoDB.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/MongoDB.API/MongoDB.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MongoDB.API.dll"]
```

### Production Considerations

- **Connection Strings** - Use secure, environment-specific configurations
- **SSL/TLS** - Enable HTTPS in production
- **Authentication** - Add JWT or other authentication mechanisms
- **Rate Limiting** - Implement API rate limiting
- **Monitoring** - Add application performance monitoring
- **Caching** - Consider Redis for caching frequently accessed data

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Write tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the Swagger documentation at `/swagger`
- Review the test cases for usage examples

## 🎯 Roadmap

- [ ] Authentication and authorization
- [ ] Rate limiting and throttling
- [ ] Redis caching integration
- [ ] GraphQL support
- [ ] Kubernetes deployment manifests
- [ ] Performance monitoring and metrics
- [ ] Advanced MongoDB features (aggregation pipelines)
- [ ] Multi-tenant support

---

**Built with ❤️ using Clean Architecture and SOLID principles**