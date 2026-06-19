# 📦 ParcelTrack - Parcel Tracking and Delivery Management System
### Group 8 | Information Management Final Activity | June 2026

A Windows Forms desktop application for managing parcels, deliveries, couriers, tracking updates, and recipient information.

---

## 🛠️ Requirements
- Windows OS
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (.NET Framework)
- NuGet Package: `MySql.Data`

---

## 🗄️ Database Setup

> Open **MySQL Workbench** and connect to your local instance. Run the SQL files **in this exact order** using the ⚡ button.

**1.** Run `sql/01_schema.sql` — creates all tables

**2.** Run `sql/00_psgc.sql` — loads Philippine provinces and municipalities

**3.** Run `sql/02_stored_procedures.sql` — creates all stored procedures

**4.** Run `sql/03_triggers.sql` — creates all triggers

**5.** Run `sql/04_sample_data.sql` — loads sample data

---

## 💻 Application Setup
**1.** Open `ParcelNiPrixy.sln` in Visual Studio 2022

**2.** Install the MySQL connector via **Package Manager Console**:
Install-Package MySql.Data

**3.** Open `app/DatabaseHelper.cs` and update line 8 with your MySQL password:
```csharp
private const string Password = ""; // ← put your password here, leave blank if none
```

**4.** Click **Build → Rebuild Solution**

**5.** Press **F5** or click **Start** to run

---

## 🔐 Login Credentials
| Username | Password |
|----------|----------|
| admin    | admin123 |

---

## 📋 How to Use

**1. Add a Rider** → go to Riders tab → ➕ Add

**2. Add a Parcel** → go to Parcels tab → ➕ Add Parcel
   - Type sender name → select existing or fill in new details
   - Type recipient name → select existing or fill in new details
   - Select origin and destination (province → municipality)
   - Select rider and ship date (estimated delivery auto-fills +3 days)
   - System auto-assigns to an existing shipment or creates a new one

**3. Track a Parcel** → go to Tracking tab → select parcel → Load → ➕ Add Update

**4. Update Payment** → go to Payments tab → select → ✏️ Edit Status

**5. View Audit Log** → go to Audit Log tab → all database activity is logged automatically by triggers

---

## 👥 Group 8 Members
- *(ibet paul marquin prixy mat)*

**1.** Open `ParcelNiPrixy.sln` in Visual Studio 2022

**2.** Install the MySQL connector via **Package Manager Console**:
