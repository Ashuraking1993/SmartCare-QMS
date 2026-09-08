# SmartCare Hospital Queue Management System

## Requirements

Install:

- Git
- .NET SDK
- Node.js + npm

---

## Clone Project

git clone YOUR_REPOSITORY_URL

cd SmartCare

---

## Backend

Open a terminal:

cd backend/QSmart.API

dotnet restore

dotnet build

dotnet run --urls "http://0.0.0.0:5025"

Backend:

http://localhost:5025

---

## Frontend

Open another terminal:

cd frontend

npm install

npm run dev

Frontend:

http://localhost:5173

---

## Database

The project uses Neon PostgreSQL.

Configure the required database connection string
before starting the backend.

---

## Development

Keep both terminals running:

Terminal 1:
dotnet run --urls "http://0.0.0.0:5025"

Terminal 2:
npm run dev
