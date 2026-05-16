using SpaceMission.Models;

namespace SpaceMission.Pathfinding;

/// <summary>
/// Abstraction for pathfinding algorithms — swap implementations without touching the rest of the app.
/// </summary>
public interface IPathfinder
{
    /// <summary>
    /// Finds the shortest (lowest-cost) path from the astronaut's position to the Space Station.
    /// </summary>
    PathResult FindPath(CosmicMap map, Astronaut astronaut);
}