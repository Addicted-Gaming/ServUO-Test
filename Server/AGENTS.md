# AGENTS Instructions for Server

- Builds the main ServUO executable and references the Ultima library.
- Run `dotnet build Server/Server.csproj` after modifying code.

## Style

- Run `dotnet format Server/Server.csproj` before committing.
- Use 4 spaces for indentation, keep lines under 120 characters, and place braces on the same line.
- Use PascalCase for public types and methods.
- Use camelCase with a leading underscore for private fields.

## Testing

- No automated tests currently exist; manually launch the server to verify changes.
- When tests are added, run `dotnet test Server` before committing.
