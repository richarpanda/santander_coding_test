# Santander Coding Test

This is a RESTful API built with ASP.NET Core 8.0 that retrieves the best stories from Hacker News API, with intelligent caching and rate limiting to handle large volumes of requests efficiently.

## Features

- Retrieve top N best stories from Hacker News
- High-performance with intelligent caching strategy
- Built-in rate limiting to prevent API overload
- Concurrent request handling with semaphore throttling
- Swagger/OpenAPI documentation

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, Visual Studio Code, or Rider)

## Project Structure

```
SANTANDER_CODING_TEST/
├── CodingTest/
    ├── Controllers/
    │   └── StoriesController.cs      # API endpoints and request handling
    ├── Services/
    │   ├── IHackerNewsService.cs     # Service interface
    │   └── HackerNewsService.cs      # Service implementation with caching
    ├── Models/
    │   └── ErrorResponse.cs          # Error model
    │   └── HackerNewsStory.cs        # Response HackerNews model
    │   └── StoryModels.cs            # StoryModelResponse model
    ├── Program.cs                     # Application configuration and DI
    ├── CodingTest.csproj          # Project file with dependencies
    ├── appsettings.json              # Configuration settings
├── .gitignore                    # Git ignore file
│   CodingTest.sln                # Project Solution
└── README.md                     # This file
    
  
```

## How to Run

### Option 1: Using .NET CLI (Recommended)

1. **Clone or download the repository**

2. **Navigate to the project directory**
   ```bash
   cd CodingTest
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - API: `https://localhost:5001/api/beststories/{n}`
   - Swagger UI: `https://localhost:5001/swagger`

### Option 2: Using Visual Studio

1. Open the `.csproj` file in Visual Studio
2. Press `F5` or click "Run" to start the application
3. The browser will automatically open with Swagger UI


## API Usage

### Get Best N Stories

**Endpoint:** `GET /api/beststories/{n}`

**Parameters:**
- `n` (path parameter): Number of stories to retrieve (1-500)

**Example Request:**
```bash
curl https://localhost:5001/api/beststories/10
```

**Example Response:**
```json
[
  {
    "title": "A uBlock Origin update was rejected from the Chrome Web Store",
    "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
    "postedBy": "ismaildonmez",
    "time": "2019-10-12T13:43:01+00:00",
    "score": 1716,
    "commentCount": 572
  },
  {
    "title": "Example Story",
    "uri": "https://example.com",
    "postedBy": "user123",
    "time": "2024-10-20T10:30:00+00:00",
    "score": 1500,
    "commentCount": 234
  }
]
```

**Error Response (500 Internal Server Error):**
```json
{
  "error": "An error occurred while retrieving stories",
  "statusCode": 500
}
```

## Enhancements & Future Improvements

**Unit and Integration Tests**
  - xUnit tests for services and controllers
  - Mock HttpClient responses
  - Test cache behavior and edge cases

**Configuration Management**
  - Externalize cache durations to appsettings.json
  - Environment-specific settings

**Swagger/OpenAPI Enhancements**
  - Add XML documentation comments
  - Include example requests/responses
  - Better API documentation

**Enhanced Security**
  - API key authentication
  - OAuth 2.0 support
  - Request signature validation
  - Rate limiting per API key

**Database Layer**
  - Store historical story data
  - Enable trend analysis over time
  - Support filtering by date ranges
  - Archive old stories

**Elasticsearch Integration**
  - Full-text search capabilities
  - Search stories by keywords
  - Advanced filtering options
  - Autocomplete suggestions