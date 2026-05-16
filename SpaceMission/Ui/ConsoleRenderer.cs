using System.Text;
using SpaceMission.Models;
using SpaceMission.Pathfinding;

namespace SpaceMission.UI;

/// <summary>
/// Handles all console output for the Space Mission application.
/// </summary>
public static class ConsoleRenderer
{
	// ── Public API ────────────────────────────────────────────────────────────

	public static void PrintBanner()
	{
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine("""
            ╔══════════════════════════════════════════════╗
            ║         🚀  SPACE 2026 — MISSION HQ  🚀      ║
            ║      Hitachi Solutions — Navigation AI       ║
            ╚══════════════════════════════════════════════╝
            """);
		Console.ResetColor();
	}

	public static void PrintSectionHeader(string title)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"\n══ {title} ══");
		Console.ResetColor();
	}

	public static void PrintSuccess(string message)
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	public static void PrintError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	public static void PrintInfo(string message)
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		Console.WriteLine(message);
		Console.ResetColor();
	}

	public static void PrintPrompt(string prompt)
	{
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write(prompt);
		Console.ResetColor();
	}

	/// <summary>Displays all mission results to the console.</summary>
	public static void PrintMissionResults(CosmicMap map, IReadOnlyList<PathResult> results)
	{
		PrintSectionHeader("MISSION RESULTS");
		Console.WriteLine();

		foreach (var result in results)
		{
			if (!result.Success)
			{
				PrintError($"Mission failed — Astronaut {result.AstronautId} lost in space!");
				Console.WriteLine();
				continue;
			}

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"Astronaut {result.AstronautId} - Shortest path: {result.TotalCost} steps");
			Console.ResetColor();

			PrintMap(map, result);
			Console.WriteLine();
		}
	}

	/// <summary>Renders the map with the path marked as '*'.</summary>
	public static void PrintMap(CosmicMap map, PathResult result)
	{
		var display = map.ToDisplayGrid();

		var pathSet = new HashSet<(int, int)>(result.PathCells);

		// Find astronaut start position to exclude from '*' marking
		(int Row, int Col)? startPos = null;
		for (int r = 0; r < map.Rows; r++)
			for (int c = 0; c < map.Cols; c++)
				if (map[r, c].Symbol == result.AstronautId)
					startPos = (r, c);

		// Calculate column widths for alignment
		int[] colWidths = new int[map.Cols];
		for (int c = 0; c < map.Cols; c++)
		{
			colWidths[c] = 1;
			for (int r = 0; r < map.Rows; r++)
				colWidths[c] = Math.Max(colWidths[c], display[r, c].Length);
			colWidths[c] = Math.Max(colWidths[c], 1);
		}

		for (int r = 0; r < map.Rows; r++)
		{
			Console.Write("  ");
			for (int c = 0; c < map.Cols; c++)
			{
				bool isPath = pathSet.Contains((r, c)) &&
							  !(startPos.HasValue && startPos.Value == (r, c)) &&
							  !map[r, c].IsStation;

				string symbol = isPath ? "*" : display[r, c];

				if (isPath)
					Console.ForegroundColor = ConsoleColor.Magenta;
				else if (map[r, c].IsStation)
					Console.ForegroundColor = ConsoleColor.Green;
				else if (map[r, c].IsAsteroid)
					Console.ForegroundColor = ConsoleColor.Red;
				else if (map[r, c].IsDebris)
					Console.ForegroundColor = ConsoleColor.DarkYellow;
				else if (map[r, c].Symbol == result.AstronautId)
					Console.ForegroundColor = ConsoleColor.Cyan;
				else
					Console.ForegroundColor = ConsoleColor.DarkGray;

				Console.Write(symbol.PadRight(colWidths[c]));
				Console.ResetColor();

				if (c < map.Cols - 1) Console.Write(" ");
			}
			Console.WriteLine();
		}
	}

	/// <summary>Builds a plain-text string of the map (used for email).</summary>
	public static string BuildMapString(CosmicMap map, PathResult result)
	{
		var display = map.ToDisplayGrid();
		var pathSet = new HashSet<(int, int)>(result.PathCells);

		var sb = new StringBuilder();
		for (int r = 0; r < map.Rows; r++)
		{
			var rowTokens = new List<string>();
			for (int c = 0; c < map.Cols; c++)
			{
				bool isPath = pathSet.Contains((r, c)) && !map[r, c].IsStation;
				rowTokens.Add(isPath ? "*" : display[r, c]);
			}
			sb.AppendLine(string.Join(" ", rowTokens));
		}
		return sb.ToString();
	}
}