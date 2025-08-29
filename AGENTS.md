# AGENTS Instructions

- This solution contains three projects:
    - **Server** – core executable that runs the ServUO shard.
    - **Scripts** – gameplay scripts referenced by the server.
    - **Ultima** – library for reading Ultima Online data files.
- Run `dotnet build` from the repository root after modifying code.
- Follow project-specific instructions in nested AGENTS.md files.

## Style

- Run `dotnet format` before committing to enforce project conventions.
- Use 4 spaces for indentation and ensure files end with a newline.
- Keep lines under 120 characters.
- Place opening braces on the same line as declarations.
- Use PascalCase for public members and types.
- Use camelCase with a leading underscore for private fields.

## Testing

- Run `dotnet test` from the repository root if tests are available.
- Add or update tests for any code changes when feasible.
