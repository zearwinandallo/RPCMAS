# RPCMAS
KCCC Take Home Exam
# Add MSSQL Docker

## Steps

1. Pull the MSSQL Docker image:
   ```bash
   docker pull mcr.microsoft.com/mssql/server:2022-latest
   ```

2. Run the SQL Server container:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=123qweASD!" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
   ```

3. After adding MSSQL Docker, update your connection strings based on your local setup:

   - In `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=YourDb;User Id=sa;Password=123qweASD!;TrustServerCertificate=True;"
     }
     ```

   - In `docker-compose.yaml`:
     ```yaml
     ConnectionStrings__DefaultConnection: "Server=host.docker.internal,1433;Database=YourDb;User Id=sa;Password=123qweASD!;TrustServerCertificate=True;"
     ```

## Notes

- Use `localhost,1433` for local machine connections.
- Use `host.docker.internal,1433` for container-to-container connections.
- Ensure credentials and database name match your configuration.


4. In your solution, open **Terminal** and run:
   ```bash
   docker compose up -d
   ```

---

# Item Catalog Access & Role Permissions

## Overview
All roles have access to the **Item Catalog**, but their permissions differ based on their assigned role.

---

## Role Permissions

### 1. Department Supervisor
- ✅ Access Item Catalog  
- ✅ Submit Requests  

---

### 2. Merchant Manager
- ✅ Access Item Catalog  
- ✅ Submit Requests  
- ✅ View Approval Request List  
- ✅ View Approval History  

- ❌ Cannot Approve Requests  
- ❌ Cannot Reject Requests  
- ❌ Cannot Apply Requests  

---

### 3. Store Manager
- ✅ View Approval Request List  
- ✅ View Approval History  

- ✅ Can Approve Requests  
- ✅ Can Reject Requests  
- ✅ Can Apply Requests  

---

## Summary

| Role                  | Submit Request | View Approval List | View History | Approve/Reject/Apply |
|----------------------|--------------|--------------------|--------------|----------------------|
| Department Supervisor| ✅ Yes        | ❌ No              | ❌ No        | ❌ No                |
| Merchant Manager     | ✅ Yes        | ✅ Yes             | ✅ Yes       | ❌ No                |
| Store Manager        | ❌ No         | ✅ Yes             | ✅ Yes       | ✅ Yes               |


