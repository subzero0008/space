using SpaceMission.Models;

namespace SpaceMission.Services;

/// <summary>
/// Generates a random cosmic navigation map with guaranteed solvability.
/// </summary>
public static class MapGenerator
{
    private static readonly Random Rng = new();

    public static CosmicMap Generate(int rows, int cols, int asteroidCount,
                                     int debrisCount = 0, int astronautCount = 1)
    {
        if (rows < 2 || rows > 100) throw new ArgumentOutOfRangeException(nameof(rows));
        if (cols < 2 || cols > 100) throw new ArgumentOutOfRangeException(nameof(cols));
        if (astronautCount < 1 || astronautCount > 3)
            throw new ArgumentOutOfRangeException(nameof(astronautCount));

        int totalCells = rows * cols;
        int reservedCells = astronautCount + 1; // astronauts + station
        int maxObstacles = totalCells - reservedCells - 1; // keep at least 1 open cell
        asteroidCount = Math.Min(asteroidCount, maxObstacles);
        debrisCount = Math.Min(debrisCount, maxObstacles - asteroidCount);

        Cell[,] grid;
        CosmicMap map;
        int attempts = 0;

        do
        {
            if (++attempts > 1000)
                throw new InvalidOperationException(
                    "Could not generate a solvable map after 1000 attempts. " +
                    "Try fewer asteroids.");

            grid = BuildGrid(rows, cols, asteroidCount, debrisCount, astronautCount);
            map = new CosmicMap(grid);
        }
        while (!IsSolvable(map));

        return map;
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static Cell[,] BuildGrid(int rows, int cols,
                                     int asteroidCount, int debrisCount,
                                     int astronautCount)
    {
        var grid = new Cell[rows, cols];

        // Fill with open space
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                grid[r, c] = new Cell(r, c, "O");

        var positions = AllPositions(rows, cols).OrderBy(_ => Rng.Next()).ToList();
        int idx = 0;

        // Place station
        var (sr, sc) = positions[idx++];
        grid[sr, sc] = new Cell(sr, sc, "F");

        // Place astronauts
        string[] ids = ["S1", "S2", "S3"];
        for (int i = 0; i < astronautCount; i++)
        {
            var (ar, ac) = positions[idx++];
            grid[ar, ac] = new Cell(ar, ac, ids[i]);
        }

        // Place asteroids
        for (int i = 0; i < asteroidCount && idx < positions.Count; i++, idx++)
        {
            var (xr, xc) = positions[idx];
            grid[xr, xc] = new Cell(xr, xc, "X");
        }

        // Place debris
        for (int i = 0; i < debrisCount && idx < positions.Count; i++, idx++)
        {
            var (dr, dc) = positions[idx];
            grid[dr, dc] = new Cell(dr, dc, "D");
        }

        return grid;
    }

    private static IEnumerable<(int, int)> AllPositions(int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                yield return (r, c);
    }

    private static bool IsSolvable(CosmicMap map)
    {
        var pathfinder = new Pathfinding.DijkstraPathfinder();
        foreach (var astronaut in map.GetAstronauts())
            if (!pathfinder.FindPath(map, astronaut).Success)
                return false;
        return true;
    }
}