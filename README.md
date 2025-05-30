# Todo Application

A modern full-stack Todo application built with:
- **Backend**: .NET Core Web API
- **Frontend**: React 19 + TypeScript + Vite
- **Styling**: Tailwind CSS
- **State Management**: React Query

## Screenshots

<div align="center">
  <img src="https://github.com/user-attachments/assets/97d6f4ac-a8eb-47db-ba66-d55a5ded388e" alt="Application Overview" width="671">
  <img src="https://github.com/user-attachments/assets/f1d92365-89c4-496e-8afe-792052d76ad7" alt="Todo List" width="675">
  <img src="https://github.com/user-attachments/assets/99a6b5f7-038c-4521-b920-8b71e3911a68" alt="Add Todo" width="680">
  <img src="https://github.com/user-attachments/assets/84c6e243-5a60-4542-a185-7482a7867eee" alt="API Documentation" width="943">
</div>

## Features

- Create, read, update, and delete todos
- Mark todos as complete/incomplete
- Responsive design
- Client-side validation
- API documentation via Swagger
- Modern UI with Tailwind CSS

## Tech Stack

### Backend
- .NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI documentation

### Frontend
- React 19
- TypeScript
- Vite
- Tailwind CSS
- React Query
- Axios

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+ recommended)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/FrankDupree/todoriko
cd todoriko
```

### 2. Backend Setup

```bash
cd Todo.Server
dotnet restore
dotnet build
```

### 3. Frontend Setup

```bash
cd todo.client
npm install
```

### 4. Running the Application

#### Option A: Run both backend and frontend together (recommended for development)

From the project root directory:

```bash
dotnet run
```

This will:
- Start the .NET Core API on http://localhost:6159
- Launch the React app on https://localhost:2425/ via Vite
- Automatically proxy API requests

#### Option B: Run separately

##### Backend

Open project using Visual Studio and run.

##### Frontend Development

Development server: https://localhost:2425/
Hot reloading is enabled

Available Scripts

```bash
# Run frontend in development mode
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

