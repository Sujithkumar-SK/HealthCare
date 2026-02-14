# .NET Backend Implementation Guide
## Patient Health Intake System

---

## 📋 Current Status

✅ **Completed:**
- Layered architecture created (Domain, Data, Business, API, Common)
- Project references configured
- Redis installed and running
- Angular frontend ready

❌ **To Do:**
- Install NuGet packages
- Create domain entities
- Create DTOs
- Implement repositories
- Implement services
- Create API controllers
- Configure PostgreSQL
- Configure Redis
- Setup dependency injection

---

## 🎯 Implementation Order

We'll build from bottom to top:
```
1. Domain Layer (Entities)
2. Common Layer (DTOs)
3. Data Layer (DbContext, Repositories, Redis)
4. Business Layer (Services)
5. API Layer (Controllers, Configuration)
```

---

## 📦 Step 1: Install NuGet Packages

### Navigate to backend folder
```bash
cd D:\Learning\PV_Health\pv-health-intake-backend
```

### Data Layer Packages
```bash
cd PVHealth.Data
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package StackExchange.Redis --version 2.7.0
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis --version 8.0.0
cd ..
```

### Business Layer Packages
```bash
cd PVHealth.Business
dotnet add package AutoMapper --version 13.0.1
dotnet add package FluentValidation --version 11.9.0
cd ..
```

### API Layer Packages
```bash
cd PVHealth.API
dotnet add package Microsoft.AspNetCore.Cors --version 2.2.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
cd ..
```

---

## 📁 Step 2: Create Folder Structure

### Domain Layer
```
PVHealth.Domain/
├── Entities/
│   ├── BaseEntity.cs
│   ├── Patient.cs
│   └── Survey.cs
└── Interfaces/
    └── IRepository.cs
```

### Common Layer
```
PVHealth.Common/
└── DTOs/
    ├── PatientDTO.cs
    ├── SurveyDTO.cs
    └── ResponseDTO.cs
```

### Data Layer
```
PVHealth.Data/
├── Context/
│   └── ApplicationDbContext.cs
├── Repositories/
│   ├── PatientRepository.cs
│   └── SurveyRepository.cs
└── Cache/
    └── RedisCacheService.cs
```

### Business Layer
```
PVHealth.Business/
└── Services/
    ├── IPatientService.cs
    ├── PatientService.cs
    ├── ISurveyService.cs
    └── SurveyService.cs
```

### API Layer
```
PVHealth.API/
├── Controllers/
│   ├── PatientController.cs
│   └── SurveyController.cs
├── Program.cs
└── appsettings.json
```

---

## 🗄️ Step 3: Database Schema

### PostgreSQL Tables

#### Patients Table
```sql
CREATE TABLE patients (
    id UUID PRIMARY KEY,
    full_name VARCHAR(200) NOT NULL,
    date_of_birth TIMESTAMP NOT NULL,
    email VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    country VARCHAR(100) NOT NULL,
    state VARCHAR(100) NOT NULL,
    city VARCHAR(100) NOT NULL,
    appointment_date TIMESTAMP NOT NULL,
    reason_for_visit TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP
);

CREATE INDEX idx_patients_email ON patients(email);
```

#### Surveys Table
```sql
CREATE TABLE surveys (
    id UUID PRIMARY KEY,
    patient_id UUID NOT NULL,
    survey_data JSONB NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP,
    FOREIGN KEY (patient_id) REFERENCES patients(id) ON DELETE CASCADE
);

CREATE INDEX idx_surveys_patient_id ON surveys(patient_id);
```

---

## 🔧 Step 4: Configuration Files

### appsettings.json
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=pv_health_db;Username=postgres;Password=your_password",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  }
}
```

---

## 🚀 Step 5: API Endpoints

### Patient Endpoints
```
POST   /api/patients              - Create patient
GET    /api/patients/{id}         - Get patient by ID
GET    /api/patients              - Get all patients
PUT    /api/patients/{id}         - Update patient
DELETE /api/patients/{id}         - Delete patient
```

### Survey Endpoints
```
POST   /api/surveys               - Create survey
GET    /api/surveys/{id}          - Get survey by ID
GET    /api/surveys/patient/{id}  - Get survey by patient ID
PUT    /api/surveys/{id}          - Update survey
DELETE /api/surveys/{id}          - Delete survey
```

---

## 🔄 Data Flow

### Create Patient Flow
```
Angular Form
    ↓ HTTP POST
API Controller (PatientController)
    ↓
Business Service (PatientService)
    ↓
Repository (PatientRepository)
    ↓
PostgreSQL Database
    ↓
Cache in Redis (1 hour TTL)
    ↓
Return Response
```

### Get Patient Flow
```
Angular Request
    ↓ HTTP GET
API Controller
    ↓
Business Service
    ↓
Check Redis Cache
    ↓
If Found → Return from Redis (FAST)
    ↓
If Not Found → Query PostgreSQL
    ↓
Store in Redis
    ↓
Return Response
```

---

## 📝 Implementation Checklist

### Phase 1: Domain & Common (30 min)
- [ ] Create BaseEntity.cs
- [ ] Create Patient.cs
- [ ] Create Survey.cs
- [ ] Create IRepository.cs
- [ ] Create PatientDTO.cs
- [ ] Create SurveyDTO.cs
- [ ] Create ResponseDTO.cs

### Phase 2: Data Layer (45 min)
- [ ] Create ApplicationDbContext.cs
- [ ] Create PatientRepository.cs
- [ ] Create SurveyRepository.cs
- [ ] Create RedisCacheService.cs
- [ ] Run database migrations

### Phase 3: Business Layer (30 min)
- [ ] Create IPatientService.cs
- [ ] Create PatientService.cs
- [ ] Create ISurveyService.cs
- [ ] Create SurveyService.cs

### Phase 4: API Layer (45 min)
- [ ] Create PatientController.cs
- [ ] Create SurveyController.cs
- [ ] Configure Program.cs
- [ ] Setup CORS
- [ ] Setup Swagger

### Phase 5: Testing (30 min)
- [ ] Test with Swagger
- [ ] Test with Postman
- [ ] Connect Angular frontend
- [ ] End-to-end testing

**Total Time: ~3 hours**

---

## 🎯 Next Steps

1. **Install PostgreSQL** (if not installed)
   - Download: https://www.postgresql.org/download/
   - Create database: `pv_health_db`

2. **Install NuGet Packages** (run commands above)

3. **Start Implementation** (I'll provide code for each layer)

4. **Test Each Layer** (as we build)

5. **Connect Angular** (update services to call API)

---

## 🔍 Quick Commands Reference

### Build Solution
```bash
dotnet build
```

### Run API
```bash
cd PVHealth.API
dotnet run
```

### Create Migration
```bash
cd PVHealth.Data
dotnet ef migrations add InitialCreate --startup-project ../PVHealth.API
```

### Update Database
```bash
dotnet ef database update --startup-project ../PVHealth.API
```

### Test Redis Connection
```bash
cd "C:\Program Files\Redis"
.\redis-cli.exe ping
```

---

**Ready to start implementation? Let me know and I'll provide the code for each layer!**
