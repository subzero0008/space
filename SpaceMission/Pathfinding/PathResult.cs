namespace SpaceMission.Pathfinding;

/// <summary>
/// Holds the result of a pathfinding operation for one astronaut.
/// </summary>
public class PathResult
{
    public string AstronautId { get; }
    public bool Success { get; }
    public int TotalCost { get; }

    /// <summary>Ordered list of (row, col) cells on the shortest path, excluding start.</summary>
    public IReadOnlyList<(int Row, int Col)> PathCells { get; }

    // Success constructor
    public PathResult(string astronautId, int totalCost, IReadOnlyList<(int, int)> pathCells)
    {
        AstronautId = astronautId;
        Success = true;
        TotalCost = totalCost;
        PathCells = pathCells;
    }

    // Failure constructor
    public PathResult(string astronautId)
    {
        AstronautId = astronautId;
        Success = false;
        TotalCost = int.MaxValue;
        PathCells = Array.Empty<(int, int)>();
    }
}