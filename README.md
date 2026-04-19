# RPCMAS
KCCC Take Home Exam

# Add MSSQL Docker

## Docker

**Terminal:**

1. `docker pull mcr.microsoft.com/mssql/server:2022-latest`
2. `docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=123qweASD!' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest`
3. In solution, go to **Terminal** and run:

```bash
docker compose up -d
```

A `docker-compose.yaml` is present in the solution root that defines the services you want to start.

> **Security note:** The `SA_PASSWORD` above is an example. Use a strong, unique password in production.

---


