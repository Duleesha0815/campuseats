# CampusEats

CampusEats is a React/Vite frontend and ASP.NET Core menu API.

## Frontend

```powershell
Set-Location "C:\path\to\campuseats\frontend"
npm install
npm run dev
```

The checked-in `frontend/.env.development` points to the local API at `http://localhost:5039`, so no URL needs to be typed when starting the frontend. The frontend requests `GET ${VITE_API_URL}/api/menu`.

## API

The API targets .NET 10 and uses PostgreSQL through Entity Framework Core.

```powershell
Set-Location "C:\path\to\campuseats\CampusEats.Api"
dotnet run
```

Before the first run, paste your Supabase PostgreSQL connection details into `CampusEats.Api/.env` as `ConnectionStrings__Default`. Keep the `FrontendOrigin` value at `http://localhost:5173` for local development. The API loads this ignored local file automatically. Its launch settings start the API at `http://localhost:5039`.

## Supabase schema

In the Supabase SQL Editor, run the contents of [`supabase/schema.sql`](./supabase/schema.sql) once to create the categories/menu tables, primary and foreign keys, constraints, indexes, and sample menu rows. The API expects those tables before `GET /api/menu` is called.

## Production configuration

| Tier | Variable | Purpose |
| --- | --- | --- |
| Frontend | `VITE_API_URL` | Public base URL for the deployed API |
| Backend | `FrontendOrigin` | Exact frontend origin allowed by the API's CORS policy |
| Backend | `ConnectionStrings__Default` | Supabase PostgreSQL connection string, including `SSL Mode=Require` |
| Backend (when JWT authentication is implemented) | `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` | JWT signing key and token issuer/audience |

Set production values only in the hosting platform's environment-variable settings. Do not commit `.env` files or credentials. The Dockerfile uses .NET 10 to match the API project and listens on `0.0.0.0:8080`; configure the Railway service's root directory as `CampusEats.Api` for this Dockerfile.
