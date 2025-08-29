# AGENTS Instructions for Scripts

- Contains gameplay scripts referenced by the server.
- Run `dotnet build Scripts/Scripts.csproj` after modifying code.

## Style

- Run `dotnet format Scripts/Scripts.csproj` before committing.
- Use 4 spaces for indentation, keep lines under 120 characters, and place braces on the same line.
- Use PascalCase for classes and methods.
- Use camelCase with a leading underscore for private fields.

## Testing

- This project has no automated tests; verify changes by running the server and exercising the affected scripts.
- When tests are added, run `dotnet test Scripts` before committing.
