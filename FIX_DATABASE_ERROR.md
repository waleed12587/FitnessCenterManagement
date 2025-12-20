# Fix Database Error - Quick Guide

## The Problem
You're getting this error when trying to save an appointment:
```
SqlException: Invalid column name 'PaymentDate'. Invalid column name 'PaymentMethod'. Invalid column name 'PaymentStatus'.
```

This happens because the database migration hasn't been applied yet.

## Quick Fix (Choose ONE method)

### Method 1: PowerShell Script (Easiest)
1. Open PowerShell in the project root directory
2. Run:
   ```powershell
   .\apply-migration.ps1
   ```

### Method 2: Visual Studio Package Manager Console
1. Open Visual Studio
2. Go to: **Tools → NuGet Package Manager → Package Manager Console**
3. Make sure the default project is set to `FitnessCenter.Web`
4. Run:
   ```powershell
   Update-Database
   ```

### Method 3: Command Line (if dotnet-ef is installed)
```powershell
cd FitnessCenter.Web
dotnet ef database update --context ApplicationDbContext
```

### Method 4: Install dotnet-ef first (if needed)
```powershell
dotnet tool install --global dotnet-ef
cd FitnessCenter.Web
dotnet ef database update --context ApplicationDbContext
```

## After Running the Migration

The error should be fixed! You can now:
- ✅ Create appointments
- ✅ Process payments
- ✅ View payment status

---

**Note:** The migration file already exists at:
`FitnessCenter.Web/Data/Migrations/20251220120000_AddPaymentStatusToAppointment.cs`

You just need to apply it to your database.

