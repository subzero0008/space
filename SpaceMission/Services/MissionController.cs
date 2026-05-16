using SpaceMission.Models;
using SpaceMission.Pathfinding;

namespace SpaceMission.Services;

/// <summary>
/// Orchestrates pathfinding for all astronauts and returns sorted results.
/// </summary>
public sealed class MissionController
{
    private readonly IPathfinder _pathfinder;

    public MissionController(IPathfinder pathfinder)
    {
        _pathfinder = pathfinder;
    }

    /// <summary>
    /// Runs pathfinding for all astronauts.
    /// Returns failures first (as required), then successes sorted by cost ascending.
    /// </summary>
    public IReadOnlyList<PathResult> ExecuteMission(CosmicMap map)
    {
        var astronauts = map.GetAstronauts();
        var results = astronauts.Select(a => _pathfinder.FindPath(map, a)).ToList();

        var failures = results.Where(r => !r.Success)
                              .OrderBy(r => r.AstronautId)
                              .ToList();

        var successes = results.Where(r => r.Success)
                               .OrderBy(r => r.TotalCost)
                               .ThenBy(r => r.AstronautId)
                               .ToList();

        return [.. failures, .. successes];
    }
}