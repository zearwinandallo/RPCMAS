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


