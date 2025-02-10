# Project Setup Guide

## 1️⃣ Clone the Repository
```sh
git clone <your-repo-url>
cd <your-project-folder>
```

---

## 2️⃣ Install Dependencies
Ensure you have .NET installed. If not, download it from [Microsoft .NET](https://dotnet.microsoft.com/).


---

## 3️⃣ Setup `appsettings.json`

🚨 **The `appsettings.json` file is not included in the repository for security reasons.**
You need to create it manually.

### **Steps:**
1. Copy `appsettings.example.json` and rename it to `appsettings.json`.
2. Open `appsettings.json` and update the database connection string.

Example:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

---

## 4️⃣ Apply Migrations & Setup Database

If the database doesn’t exist or needs to be updated, run:

```sh
Update-Database
```

This will apply all migrations and create/update the database.

If you get migration issues, you might need to reset the database:
```sh
dotnet ef database drop --force
Update-Database
```

---

## 5️⃣ Run the Application
Now, start the application:
```sh
dotnet run
```

Your API or application should now be running! 🚀

---

## Troubleshooting
✅ **Migration Issues?** Try running:
```sh
dotnet ef migrations remove
Add-Migration InitialMigration
Update-Database
```

✅ **Database Connection Issues?**
- Double-check your `appsettings.json` connection string.
- Ensure SQL Server is running.
- Check firewall or authentication settings.

---

## 📌 Notes
- **Always run `git pull` before working to get the latest migrations.**
- If any changes to the database schema occur, ensure you run `Update-Database`.

📫 **Need help? Contact the repo owner.**

