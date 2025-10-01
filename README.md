# 🚀 Hacker News Best Stories API

This project exposes a RESTful API using **ASP.NET Core (.NET 9)** that retrieves the **best N stories from the Hacker News API**, ordered by score.

---

## 📌 Purpose
The API must:
- Retrieve the **best stories** (`amount` defined by the caller).
- Return them sorted by **score** in descending order.
- Be **resilient and efficient**, avoiding overload of both our API and the external Hacker News API.

---

## 🛠️ Features Implemented

### ✅ Caching
- Implemented **MemoryCache** to avoid calling Hacker News API on every request.
- Prepared the architecture to easily switch to **Redis** (for distributed cache in production).
- Cache reduces latency and improves API responsiveness.

### ✅ Resilience with Polly
- **Retry with exponential backoff** → 200ms → 400ms → 800ms, when Hacker News is unstable.
- **Circuit Breaker** → prevents overwhelming an external service already failing, by opening the circuit after repeated errors.

### ✅ Rate Limiting
- **Native .NET Rate Limiting Middleware**: protects our API by limiting inbound requests (per IP, per user, per API key).
- Ensures **fair usage** and prevents abuse.

### ✅ External Rate Limiter
- Custom limiter applied before making calls to Hacker News.
- Ensures our application never exceeds a safe request threshold against the upstream API.
- Prevents our service from being blocked or throttled by Hacker News.

### ✅ Health Checks
- `/health` endpoint exposes application health in JSON format.
- Checks:
  - Our API process.
  - External Hacker News API availability.
  - Memory cache availability.

---

## 🔐 Why this matters

By combining:
- **Cache** (avoid redundant external calls),
- **Polly Retry Policies** (recover from transient failures),
- **Circuit Breaker** (stop hammering when external service is down),
- **Native Rate Limiter** (protect our API from being overloaded),
- **External Rate Limiter** (avoid overloading Hacker News),

👉 we guarantee that:
- **Our API will not be overloaded by high traffic.**
- **The Hacker News API will not be overloaded by us.**
- **Users experience consistent performance and reliability.**

---

## 🏗️ Architecture (Clean Architecture Style)

The solution follows a layered architecture:

```bash
/src
├── Api
│ └── Controllers -> Expose REST endpoints
├── Application
│ └── Services -> Business logic (cache, ordering, orchestration)
├── Infrastructure
│ └── HttpClients -> HackerNews HttpClient with Polly policies
│ └── RateLimiting -> Internal & external rate limiters
│ └── Cache -> MemoryCache (ready for Redis)
├── Domain
│ └── Entities -> Core domain models
└── Contracts (Shared)
└── DTOs -> Data transfer objects
```


- **Controllers** → entrypoints (REST API).  
- **Services** → orchestrate business logic (get IDs, fetch stories, apply cache, sort).  
- **Clients** → isolated typed HttpClient for Hacker News.  
- **DTOs** → input/output models.  
- **Domain** → core entity models.  

---

## ▶️ How to Run

```bash
# clone repository
git clone https://github.com/brunoverc/hackernews-beststories.git

cd hackernews-beststories

# run the API
dotnet run --project src/HackerNews.BestStories.Api
```

Then navigate to:

Swagger UI → https://localhost:5001/swagger

Health check → https://localhost:5001/health

Example call → https://localhost:5001/api/v1/stories/bests/{amount}

## ✅ Example Response

```bash
[
  {
    "title": "A uBlock Origin update was rejected from the Chrome Web Store",
    "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
    "postedBy": "ismaildonmez",
    "time": "2019-10-12T13:43:01+00:00",
    "score": 1716,
    "commentCount": 572
  }
]
```

## 🔮 Future Improvements

- Switch from MemoryCache → Redis for distributed environments.

- Add OpenTelemetry for logs, metrics and traces (Prometheus + Grafana).

- Add Dockerfile & docker-compose for local development and observability stack.

- Improve integration tests for rate limiting and circuit breaker behavior.

- Implement background cache refresh to keep stories updated proactively.

## 👨‍💻 Author

Developed by Bruno Verçosa as part of a technical coding challenge.
Focus: Resilience, Performance, and Clean Architecture in .NET.

