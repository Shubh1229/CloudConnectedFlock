# ☁️ Cloud Connected Flock – Account & Login Microservices

## Overview

This project is a microservice-based backend system built with **.NET 7**, designed to handle **user account registration** and **login** using **gRPC communication** between services. It's suitable for cloud deployment and serves as the foundational authentication layer of a larger distributed system.

The solution includes:

- `AccountService` — gRPC service for creating and retrieving user accounts.
- `LoginServices` — HTTP REST API for login and registration, acting as a gRPC client to `AccountService`.

---

## 🚀 Features

- 🔐 **Account Creation** with username, password, email, and birthday.
- 🔑 **Secure Password Hashing** using HMAC-SHA512.
- ✅ **Login Validation** via gRPC calls to the Account service.
- 📦 **PostgreSQL** database for account persistence.
- 🐳 **Docker Compose** setup for easy development and deployment.
- 🌐 **gRPC Communication** for fast, efficient service-to-service interaction.

---

## 📁 Project Structure

```plaintext
.
├── AccountService/
│   ├── Controllers/
│   ├── GrpcServices/
│   ├── Security/
│   ├── Data/
│   ├── Protos/               # gRPC definitions
│   └── Dockerfile
├── LoginServices/
│   ├── Controllers/
│   ├── DTOs/
│   └── Dockerfile
├── docker-compose.yml
└── CloudConnectedFlockSolution.sln
```

---

## 🛠️ Technologies Used

- **.NET 7**
- **gRPC**
- **ASP.NET Core**
- **PostgreSQL**
- **Docker & Docker Compose**
- **Entity Framework Core**
- **HMAC-SHA512** for password hashing

---

## 🧪 Running the Project

### 🔧 Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 📦 Run with Docker Compose

```bash
docker-compose up --build
```

This will:
- Spin up a PostgreSQL container
- Build and run the AccountService (on port `9000`)
- Build and run the LoginServices (on port `5000` HTTP, `5001` HTTPS)

---

## 🎯 API Endpoints

### `POST /login`
Authenticates a user via gRPC and returns an appropriate response.

```json
{
  "username": "user123",
  "password": "mypassword"
}
```

### `POST /register`
Registers a new user.

```json
{
  "username": "user123",
  "password": "mypassword",
  "email": "user@example.com",
  "birthday": "2000-01-01"
}
```

---

## 🔐 Security

- Passwords are never stored in plain text.
- Each password is hashed using a unique key (HMACSHA512).
- gRPC is used for secure, efficient communication between internal services.

---

## 📄 License

This project is licensed under the [Apache 2.0 License](License).

---

## 👨‍💻 Author

Arihant Singh  
M.S. Computer Science, Franklin University  
Passionate about cloud-native development, distributed systems, and secure backend services.

---

## 🏗️ Future Improvements

- Add **2FA** via email (Mailjet integration).
- Implement **JWT tokens** for login sessions.
- Introduce **RabbitMQ** or **MassTransit** for messaging between microservices.
- Add support for **Kubernetes** deployment.
