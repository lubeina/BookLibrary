# Book Manager

A small app for managing books and authors. Built with a .NET Web API backend and a Blazor Server frontend, connected to a PostgreSQL database.

## What you need

- .NET 10 SDK
- PostgreSQL running locally on port 5432
- EF Core CLI tools — install with `dotnet tool install --global dotnet-ef`

## Getting started

### 1. Start the API

```bash
cd api
dotnet ef migrations add InitialCreate
dotnet run --urls "http://localhost:5001"
```

The database gets created automatically when the API starts up for the first time. You can check it's working by visiting http://localhost:5001/api/health — you should see `{"status":"Connected!"}`.

### 2. Start the frontend

Open a separate terminal:

```bash
cd client
dotnet run --urls "http://localhost:5002"
```

Then open http://localhost:5002 in your browser. You should see the Authors and Books sections.

## How it works

The API handles all the database operations  these endpoints:

**Authors** — `/api/authors` (GET, POST, PUT, DELETE)

**Books** — `/api/books` (GET, POST, PUT, DELETE)

When you create a book, the API checks that the selected author actually exists. If you delete an author, all their books get deleted too (cascade delete).

## A few things to know

- The Postgres connection string lives in `api/appsettings.json`. You'll probably need to change the username and password to match your local setup.
- The API runs on port 5001 and the frontend on 5002. Both need to be running at the same time.
