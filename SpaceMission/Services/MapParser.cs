using SpaceMission.Models;

namespace SpaceMission.Services;

/// <summary>
/// Parses raw console input into a <see cref="CosmicMap"/>.
/// </summary>
public static class MapParser
{
    private static readonly HashSet<string> ValidSymbols =
        new(StringComparer.OrdinalIgnoreCase) { "O", "X", "F", "D", "S1", "S2", "S3" };

    public static CosmicMap Parse(int rows, int cols, string[] lines)
    {
        if (lines.Length != rows)
            throw new ArgumentException($"Expected {rows} map lines, got {lines.Length}.");

        var grid = new Cell[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            var tokens = lines[r].Trim()
                                 .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != cols)
                throw new ArgumentException(
                    $"Row {r + 1}: expected {cols} symbols, got {tokens.Length}.");

            for (int c = 0; c < cols; c++)
            {
                string sym = tokens[c].ToUpper();
                if (!ValidSymbols.Contains(sym))
                    throw new ArgumentException($"Unknown symbol '{tokens[c]}' at row {r + 1}, col {c + 1}.");

                grid[r, c] = new Cell(r, c, sym);
            }
        }

        Validate(grid, rows, cols);
        return new CosmicMap(grid);
    }

    private static void Validate(Cell[,] grid, int rows, int cols)
    {
        bool hasStation = false;
        bool hasS1 = false;
        var astronautIds = new HashSet<string>();

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                var sym = grid[r, c].Symbol;
                if (sym == "F") { hasStation = true; }
                if (sym == "S1") { hasS1 = true; }
                if (sym is "S1" or "S2" or "S3")
                {
                    if (!astronautIds.Add(sym))
                        throw new InvalidOperationException($"Duplicate astronaut '{sym}' on the map.");
                }
            }

        if (!hasStation)
            throw new InvalidOperationException("Map must contain exactly one Space Station 'F'.");
        if (!hasS1)
            throw new InvalidOperationException("Map must contain at least astronaut 'S1'.");
    }
}