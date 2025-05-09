# ☁️ Cloud Connected Flock – Account & Login Microservices

## Overview

Cloud Connected Flock is a microservice-based backend system built with **.NET 8**, designed for **user account registration**, **login**, and **online presence tracking** using **gRPC communication** and **Redis-backed heartbeats**. It serves as the foundational authentication and status layer of a distributed application and is suitable for cloud deployment.

The system includes:

- `AccountService`: gRPC service for account creation and credential validation.
- `LoginServices`: REST API for user login and registration, connecting to `AccountService` via gRPC.
- `HeartBeatService`: gRPC and Redis-powered microservice that stores and tracks online user presence.
- `HubService`: Lightweight hub exposing the list of online users via real-time protocols like MQTT/WebSockets.
- `NGINX`: Acts as a reverse proxy to expose services securely.

---

## 🚀 Features

- 🔐 Account creation with secure password storage (HMACSHA512).
- ✅ Login verification using gRPC and protobuf models.
- 🧠 Heartbeat tracking for active/online users using Redis.
- 📋 Online user visibility via HubService.
- 🌍 Centralized HTTP access through NGINX reverse proxy.
- 🐳 Docker Compose environment for development and deployment.

---

## 📁 Project Structure

```plaintext
.
├── AccountService/
├── LoginServices/
├── HeartBeatService/
├── HubService/
├── nginx/
│   └── conf.d/
├── docker-compose.yml
├── appsettings.json
└── CloudConnectedFlockSolution.sln
```

---

## 🛠️ Tech Stack

- **.NET 8**, **ASP.NET Core**
- **gRPC** for microservice communication
- **Redis** for status tracking
- **PostgreSQL** via Entity Framework Core
- **Docker & Docker Compose**
- **NGINX** for reverse proxy and SSL termination

---

## 🧪 Running the Project

### 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### ▶️ Start with Docker Compose

```bash
docker-compose up --build
```

This will launch:

- PostgreSQL on `5432`
- AccountService on `9000` (gRPC)
- LoginService on `5000` (HTTP) and `5001` (HTTPS)
- HeartBeatService with Redis integration
- HubService for displaying online users
- NGINX on port `80` or `443` as configured

---

## 🎯 REST API Endpoints

### `POST /login`

Authenticates user credentials via gRPC.

```json
{
  "username": "exampleUser",
  "password": "securePass123"
}
```

### `POST /register`

Creates a new account.

```json
{
  "username": "newuser",
  "password": "newpass",
  "email": "user@example.com",
  "birthday": "2000-01-01"
}
```

---

## 🔐 Security

- Passwords hashed using HMACSHA512 and salted with per-user keys.
- Sensitive operations use gRPC over internal ports.
- NGINX enforces HTTPS externally.

---

## 📄 License

Licensed under the [Apache 2.0 License](LICENSE).

---

## 👨‍💻 Author

**Arihant Singh**  
M.S. Computer Science, Franklin University  
Passionate about secure, cloud-native, distributed applications.

---

## 🛣️ Roadmap

- 🪪 Implement JWT-based session auth.
- 📬 Integrate Mailjet for 2FA and notifications.
- 📊 Web UI with real-time online/offline updates.
- ☸️ Full Kubernetes manifests for production deployment.

