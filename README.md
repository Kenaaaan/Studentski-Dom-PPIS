# Studentski Dom (Student Dormitory Management System)

A comprehensive management system for student dormitories, encompassing access rights, requests/complaints handling, and facility administration.

## 🌟 Features
- **User Authentication**: Role-based access control (Student, Admin, Staff).
- **Access Management**: Control physical access to rooms, networks, and building facilities.
- **Request Management**: Submit and track status of maintenance, inventory replacement, or document requests.
- **Reporting**: Full visibility for administration staff over students, rooms, and access logs.

## 💻 Tech Stack
- **Backend**: C# .NET 10, ASP.NET Core Web API, Entity Framework Core 
- **Database**: PostgreSQL (deployed via Docker Compose)
- **Frontend**: Next.js, React, Tailwind CSS

## 📊 Entity Relationship Diagram

```mermaid
erDiagram
    USER ||--o{ REQUEST : "Submits"
    USER ||--o{ REQUEST : "Assigned To"
    USER ||--o{ ACCESS_RIGHT : "Receives"
    USER ||--o{ ACCESS_RIGHT : "Grants"
    USER ||--o{ ACCESS_LOG : "Logs"
    USER ||--o{ COMMENT : "Writes"
    
    REQUEST ||--o{ COMMENT : "Contains"
    REQUEST }|--|o ROOM : "Related To"
    
    ACCESS_RIGHT }|--|o ROOM : "Grants access to"
    ACCESS_RIGHT }|--|o RESOURCE : "Grants access to"
    ACCESS_RIGHT ||--o{ ACCESS_LOG : "Tracks"
    
    USER {
        uuid Id PK
        string FirstName
        string LastName
        string Email
        enum Role
        enum StudentStatus
    }
    
    REQUEST {
        uuid Id PK
        string Title
        string Description
        enum Status
        enum RequestType
        enum Priority
        uuid RequestedByUserId FK
        uuid AssignedToUserId FK
        uuid RoomId FK
    }
    
    ACCESS_RIGHT {
        uuid Id PK
        uuid UserId FK
        uuid RoomId FK
        uuid ResourceId FK
        enum AccessType
        boolean IsActive
        DateTime ExpiresAt
    }
    
    ROOM {
        uuid Id PK
        string RoomNumber
        int Capacity
    }
    
    RESOURCE {
        uuid Id PK
        string Name
        string Type
    }
    
    COMMENT {
        uuid Id PK
        string Content
        uuid RequestId FK
        uuid UserId FK
    }
    
    ACCESS_LOG {
        uuid Id PK
        uuid UserId FK
        uuid AccessRightId FK
        boolean IsSuccess
        DateTime Timestamp
    }
```

## 🚀 Getting Started

### Infrastructure (Database)
Requires Docker Desktop installed and running.
```bash
docker-compose up -d
```

### Backend
Navigate to `backend/src/StudentskiDom.API` and start the server:
```bash
dotnet run
```
Provides the Swagger UI at `http://localhost:5104/swagger`.

### Frontend
Navigate to the `frontend` folder, install dependencies, and run:
```bash
npm install
npm run dev
```
Navigate to `http://localhost:3000`.

Email: admin@studentskidom.ba (ili staff@studentskidom.ba za tehničara)
Lozinka: Admin123! (respektivno Staff123!)