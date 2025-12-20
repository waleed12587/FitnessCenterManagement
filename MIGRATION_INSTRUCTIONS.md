# Database Migration Instructions

## Problem
The database is missing the new payment columns: `PaymentStatus`, `PaymentDate`, and `PaymentMethod`.

## Solution
You need to run the migration in Visual Studio using Package Manager Console.

### Steps:

1. **Open Visual Studio**
2. **Open Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
3. **Make sure the default project is set to `FitnessCenter.Web`**
4. **Run these commands:**

```powershell
Add-Migration AddPaymentStatusToAppointment
Update-Database
```

### Alternative (if you have dotnet-ef installed):

Open PowerShell in the project root and run:

```powershell
cd FitnessCenter.Web
dotnet ef migrations add AddPaymentStatusToAppointment --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

### If dotnet-ef is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

Then run the migration commands above.

---

**Note:** The migration file has already been created. You just need to apply it to the database using `Update-Database`.

