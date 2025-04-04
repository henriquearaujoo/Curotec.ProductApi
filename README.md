# 🛠️ Product API

A clean, modular ASP.NET Core 7 Web API demonstrating advanced backend architecture patterns including the Repository + Specification pattern, custom middleware, caching, error handling, and more.

---

## 📌 Features

- ✅ Clean Architecture (Domain / Application / Infrastructure / API)
- ✅ Repository Pattern with Specification Objects
- ✅ EF Core 7 with SQL Server 2022 support
- ✅ Entity-level Caching with configurable expiration
- ✅ Pagination, Sorting & Filtering
- ✅ Custom Middleware:
  - Request Logging
  - Correlation ID Propagation
  - Global Exception Handling (Structured JSON)
- ✅ Response Compression (Gzip/Brotli)
- ✅ FluentValidation for DTO validation
- ✅ Dockerized with SQL Server (via Docker Compose)
- ✅ Unit Tests using xUnit + Moq

---

## 🏗️ Technologies

| Layer         | Tech Stack                                 |
|---------------|---------------------------------------------|
| Language      | C# 11                                       |
| Framework     | ASP.NET Core 7                              |
| ORM           | Entity Framework Core 7                     |
| Validation    | FluentValidation 11.3.0                     |
| DB            | SQL Server 2022 (Docker)                    |
| Testing       | xUnit, Moq                                  |
| Caching       | IMemoryCache                                |
| Container     | Docker, Docker Compose                      |

---

## 🚀 Getting Started

### 🔧 Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Docker + Docker Compose](https://www.docker.com/)
- Optional: Postman / Swagger / HTTPie

---

### 🐳 Run with Docker

```bash
docker-compose up -d sqlserver 
```

- Product API: [https://localhost:7102](https://localhost:7102)
- SQL Server: `localhost,1433` (`sa` / `Your_strong_password123`)

---

### 🧪 Run Tests

```bash
dotnet test Curotec.ProductApi.Tests
```

---

## 🔗 API Endpoints

> Base URL: `https://localhost:7102/api/products`

| Method | Endpoint               | Description             |
|--------|------------------------|-------------------------|
| GET    | `/products`            | List products (supports `search`, `sort`, `pageIndex`, `pageSize`) |
| GET    | `/products/{id}`       | Get product by ID       |
| POST   | `/products`            | Create product          |
| PUT    | `/products/{id}`       | Update product          |
| DELETE | `/products/{id}`       | Delete product          |

---

## 🧠 Architectural Decisions

- **Repository + Specification Pattern** for clean query separation
- **Caching** only at read layer to decouple from EF tracking
- **Manual validation** using FluentValidation to enable centralized error handling
- **Middleware-first** for logging, error handling, compression
- **DTOs** to protect domain from input binding & validation concerns

---

## 🛠️ Configuration

### 📁 `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=ProductApiDb;User=sa;Password=Your_strong_password123;TrustServerCertificate=True"
  },
  "RequestLogging": {
    "Enabled": true
  }
}
```

---

## 🧪 Example Requests

```bash
curl "https://localhost:7102/api/products?search=phone&pageIndex=1&pageSize=5&sort=priceDesc"
```

```http
POST /api/products
Content-Type: application/json

{
  "name": "Wireless Mouse",
  "price": 29.99,
  "description": "Compact and portable"
}
```

---

## 🧱 Project Structure

```
/ProductApi.Api           - API layer (Controllers, Middlewares)
/ProductApi.Application   - DTOs, Interfaces, Validators
/ProductApi.Domain        - Entities, Specifications
/ProductApi.Infrastructure- EF Core, Repositories, Caching
/ProductApi.Tests         - xUnit test project
```

---

## 📦 Migrations (EF Core)

```bash
dotnet ef database update --project Curotec.ProductApi.Infrastructure --startup-project Curotec.ProductApi.Api
```

---

## 📌 Submission Notes

- ✅ Migrations included
- ✅ Custom Middleware implemented and tested
- ✅ All acceptance criteria met
- ✅ Ready for Docker run
- ✅ Correlation ID + Logging + Compression in place
- ✅ Unit tests for repository, caching, controller

---

## 👨‍💻 Author

Made by Henrique Araujo