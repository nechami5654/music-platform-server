# Music Platform - Backend API

A powerful and scalable Web API backend for a music platform, designed to support a YouTube-like experience for both singers and listeners. Built with ASP.NET Core and structured with clean architecture and best practices.

## Project Overview

This backend project serves as the server-side engine for a full-stack music platform. It supports:

* Singers uploading and managing songs
* Users streaming songs, reacting with likes and comments
* Advanced search by text (Elasticsearch) and voice (speech-to-text)
* Automated email notifications based on user feedback

> Note: This repository includes **only the backend**. A separate repository will be created for the React frontend client.

---

## Technologies Used

### Backend

* **ASP.NET Core** (C#) - Web API
* **Entity Framework Core** - ORM
* **SQL Server** - Relational database
* **JWT Authentication** - User authentication and authorization

### Architecture

* 4-layer architecture:

  * **Mock**
  * **Repository**
  * **Service**
  * **Web API**
* Principles:

  * **OOP** (Object-Oriented Programming)
  * **SOLID** principles for maintainable, scalable code

### External Integrations

* **Elasticsearch** - Full-text search engine
* **Azure Speech API** - Speech-to-text processing
* **MailJet** - Automated email notifications

### Development Tools & Best Practices

* **AutoMapper** – for mapping between entities and DTOs  
* **Swagger (OpenAPI)** – for API documentation and testing  
* **Dependency Injection** – built-in DI container of ASP.NET Core  
* **Code First Migrations** – database schema generation via EF Core  
* **Postman** – for manual API testing  
* **DTOs and ViewModels** – for separation of layers and clean data handling  
* **Clean Code & SoC (Separation of Concerns)** – maintainable and readable architecture  

---

## User Roles & Permissions

| Role                  | Permissions                                      |
| --------------------- | ------------------------------------------------ |
| **Singer**            | Upload, update, and manage songs                 |
| **Registered User**   | Listen to songs, view history, like, and comment |
| **Guest (Anonymous)** | Listen to songs only                             |

---

## Features

* **Secure login and registration with JWT**
* **Song management system for singers**
* **User history tracking**
* **Like/comment system with 1 negative feedback per song per user**
* **Negative feedback aggregation → email alert to singer → song deletion**

* **Search capabilities:**
  * **Basic filters** – by song name, artist name, category, and target audience
  * **Advanced search:**
    * Keyword-based (Elasticsearch)
    * Voice-based (Speech-to-text search)

---

## Getting Started

To run this backend project:

1. Clone the repo
2. Add your own `appsettings.json` (credentials and API keys)
3. Run the project in Visual Studio

---

## Project Structure

```
MusicPlatform.Server
│
├── Controllers/             # Web API Controllers
├── Repository/              # Data access layer
├── Service/                 # Business logic layer
├── Mock/                    # Data models & interfaces
├── appsettings.json         # Configuration file (excluded from Git for security)
└── MusicPlatform.sln        # Solution file
```

---

## Contact

For questions or feedback, feel free to reach out:
**Nechami Schwartz** – Full Stack Developer  
[nechami5654@gmail.com](mailto:nechami5654@gmail.com)

---

> Final project grade: 100
> Built with passion, code quality, and real-world architecture in mind
