# Project Setup Guide

## Prerequisites

Ensure you have the following installed:

- Visual Studio 2022
- .NET SDK (compatible version with the project)
- SQL Server (or update connection string for your DB provider)

## Steps to Setup After Cloning Repository

### 1. Restore Dependencies

Run the following command in the project root to restore NuGet packages:

```sh
 dotnet restore
```

### 2. Update `appsettings.json`

- The `appsettings.json` file is not included in the repository for security reasons.
- Create a new `appsettings.json` file in the project’s root.
- Copy the content from `appsettings.example.json` (or ask the team for the correct configuration).
- Ensure the **database connection string** is correct.

### 3. Apply Migrations and Update Database

If you are setting up for the first time:

```sh
 dotnet ef database update
```

If the migrations folder is missing (not committed), generate migrations first:

```sh
 dotnet ef migrations add InitialCreate
 dotnet ef database update
```

### 4. Manually Create Upload Folder

This project requires an `Upload` folder for storing images and post-related media.

#### Steps to create it manually:

1. Navigate to the project's root directory.
2. Create a new folder named `Upload`.
3. Inside `Upload`, create the following subfolders:
   - `images`
   - `Posts`

Alternatively, run the following command in the terminal:

```sh
mkdir Upload && cd Upload && mkdir images Posts
```

### 5. Run the Application

Use the following command to start the application:

```sh
dotnet run
```

Or start it using Visual Studio (F5 or Ctrl+F5).

### Notes

- If you face issues related to missing migrations, ensure the `Migrations` folder is committed to the repository.
- If there are database schema conflicts, consider running:
  ```sh
  dotnet ef migrations remove
  dotnet ef migrations add ResolvedMigration
  dotnet ef database update
  ```
- Contact the team for any additional configurations.

Happy coding! 🚀
