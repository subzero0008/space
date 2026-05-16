using SpaceMission.Models;

namespace SpaceMission.Pathfinding;

/// <summary>
/// Dijkstra-based pathfinder.
/// Handles weighted cells (Space Debris = cost 2) correctly.
/// For uniform-cost maps it degenerates to BFS performance.
/// </summary>
public sealed class DijkstraPathfinder : IPathfinder
{
    // 4-directional movement
    private static readonly (int dr, int dc)[] Directions =
        [(-1, 0), (1, 0), (0, -1), (0, 1)];

    public PathResult FindPath(CosmicMap map, Astronaut astronaut)
    {
        var station = map.GetStation();
        if (station is null)
            return new PathResult(astronaut.Id);

        int rows = map.Rows;
        int cols = map.Cols;

        // dist[r,c] = minimum cost to reach (r,c)
        var dist = new int[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                dist[r, c] = int.MaxValue;

        // prev[r,c] = predecessor cell for path reconstruction
        var prev = new (int r, int c)?[rows, cols];

        dist[astronaut.Row, astronaut.Col] = 0;

        // Min-heap: (cost, row, col)
        var pq = new PriorityQueue<(int row, int col), int>();
        pq.Enqueue((astronaut.Row, astronaut.Col), 0);

        while (pq.Count > 0)
        {
            var (cr, cc) = pq.Dequeue();
            int currentCost = dist[cr, cc];

            // Early exit
            if (cr == station.Value.Row && cc == station.Value.Col)
                break;

            foreach (var (dr, dc) in Directions)
            {
                int nr = cr + dr;
                int nc = cc + dc;

                if (!map.InBounds(nr, nc)) continue;

                var cell = map[nr, nc];
                if (cell.IsAsteroid) continue;

                int newCost = currentCost + cell.MovementCost;
                if (newCost < dist[nr, nc])
                {
                    dist[nr, nc] = newCost;
                    prev[nr, nc] = (cr, cc);
                    pq.Enqueue((nr, nc), newCost);
                }
            }
        }

        int finalCost = dist[station.Value.Row, station.Value.Col];
        if (finalCost == int.MaxValue)
            return new PathResult(astronaut.Id);

        // Reconstruct path (exclude start, include destination)
        var path = ReconstructPath(prev, astronaut, station.Value);
        return new PathResult(astronaut.Id, finalCost, path);
    }

    private static List<(int, int)> ReconstructPath(
        (int r, int c)?[,] prev,
        Astronaut start,
        (int Row, int Col) end)
    {
        var path = new List<(int, int)>();
        (int r, int c)? current = (end.Row, end.Col);

        while (current.HasValue)
        {
            var (r, c) = current.Value;
            if (r == start.Row && c == start.Col) break;
            path.Add((r, c));
            current = prev[r, c];
        }

        path.Reverse();
        return path;
    }
}