dotnet version 8.0.204

You may have to update the Visual Studio in the Visual Studio Installer

First time:
  git clone https://github.com/jezuscame/Sankabinis.git;
  In the project: Tools -> NuGet Package Manager -> Package Manager Console: Update-Database;
  crtl + f5

After changing database:
  Package Manager Console: Add-Migration <MigrationNameWithoutSpaces>;
  Package Manager Console: Update-Database

Do not forget to push changes
