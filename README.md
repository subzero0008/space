# 🚀 SPACE 2026 — Cosmic Navigation System

> **Assessment Task**  
> A C# .NET console application that guides astronauts stranded in space back to the Space Station, navigating through asteroid fields using intelligent pathfinding.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [How to Run](#how-to-run)
- [How to Use](#how-to-use)
- [Cosmic Symbols](#cosmic-symbols)
- [Example](#example)
- [Bonus Objectives Completed](#bonus-objectives-completed)
- [Architecture & OOP Design](#architecture--oop-design)
- [Email Report Feature](#email-report-feature)

---

## Overview

SPACE 2026 is a C# .NET 10 console application that solves shortest-path navigation for up to **3 astronauts** on a cosmic map filled with open space, asteroids, space debris, and a Space Station destination.

For each astronaut, the application:
- Finds the **shortest (lowest-cost) path** to the Space Station using Dijkstra's algorithm
- **Visualizes the route** on the map, marking safe cells with `*`
- **Sorts results** by path length (shortest first)
- Reports astronauts with **no valid path** at the top of the output

---

## Project Structure

```
SpaceMission/
├── SpaceMission.csproj
├── Program.cs                    # Entry point & menu logic
├── Properties/
│   └── launchSettings.json
├── Models/
│   ├── Cell.cs                   # Single map cell with movement cost
│   ├── CosmicMap.cs              # Full grid with helper methods
│   └── Astronaut.cs              # Astronaut identity & position
├── Pathfinding/
│   ├── IPathfinder.cs            # Swappable pathfinding interface
│   ├── PathResult.cs             # Result model (cost + path cells)
│   └── DijkstraPathfinder.cs     # Dijkstra implementation (weighted)
├── Services/
│   ├── MapParser.cs              # Parses & validates console input
│   ├── MapGenerator.cs           # Random solvable map generator
│   ├── MissionController.cs      # Orchestrates all astronaut missions
│   └── EmailReporter.cs          # SMTP email report sender
└── UI/
    └── ConsoleRenderer.cs        # All console output & color rendering
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 8+)
- Windows Terminal / any terminal with UTF-8 support

Verify your installation:
```
dotnet --version
```

---

## How to Run

### Option 1 — From Terminal (Recommended)

```
cd project path
dotnet run --project SpaceMission.csproj
```



## How to Use

On launch, you are presented with a main menu:

```
╔══════════════════════════════════════════════╗
║         🚀  SPACE 2026 — MISSION HQ  🚀      ║
║      Hitachi Solutions — Navigation AI       ║
╚══════════════════════════════════════════════╝

══ MAIN MENU ══
  [1] Enter cosmic map manually
  [2] Generate random cosmic map
  [0] Exit mission
```

### Option 1 — Manual Map Input

Enter the map dimensions and rows one by one:

```
Map rows (2-100): 5
Map columns (2-100): 7

Row 1: S1 O X O O O S2
Row 2: X O O O O X O
Row 3: X X O X O X O
Row 4: O X X O O X O
Row 5: O X X O O O F
```

### Option 2 — Random Map Generation

```
Map rows (2-100): 8
Map columns (2-100): 8
Number of asteroids: 10
Number of debris cells (D): 3
Number of astronauts (1-3): 2
```

The generator guarantees a **solvable map** — every astronaut will have at least one valid path to the Space Station.

---

## Cosmic Symbols

| Symbol | Name | Description | Movement Cost |
|--------|------|-------------|---------------|
| `S1` `S2` `S3` | Astronaut | Starting positions (up to 3) | — |
| `F` | Space Station | Final destination | 1 |
| `O` | Open Space | Safe to travel through | 1 |
| `X` | Asteroid | Impassable — do not enter! | ∞ |
| `D` | Space Debris | Passable but costly | **2** |

---

## Example

**Input:**
```
Map rows: 5  |  Map columns: 7

S1 O  X  O  O  O  S2
X  O  O  O  O  X  O
X  X  O  X  O  X  O
O  X  X  O  O  X  O
O  X  X  O  O  O  F
```

**Output:**
```
Astronaut S2 - Shortest path: 4 steps
  S1 O  X  O  O  O  S2
  X  O  O  O  O  X  *
  X  X  O  X  O  X  *
  O  X  X  O  O  X  *
  O  X  X  O  O  O  F

Astronaut S1 - Shortest path: 10 steps
  S1 *  X  O  O  O  S2
  X  *  *  *  *  X  O
  X  X  O  X  *  X  O
  O  X  X  O  *  X  O
  O  X  X  O  *  *  F
```

---

## Bonus Objectives Completed

### ✅ 1. Space Debris Navigation (`D` symbol)

Space Debris cells are **passable but cost 2 steps** instead of 1. The pathfinding algorithm accounts for this weighted cost — the shortest path reflects **total cost**, not just number of cells traversed.

This is handled in `Cell.cs`:
```csharp
public int MovementCost => IsDebris ? 2 : 1;
```

And in `DijkstraPathfinder.cs` where each neighbor's cost is computed as:
```csharp
int newCost = currentCost + cell.MovementCost;
```

---

### ✅ 2. Advanced OOP — Swappable Pathfinding Algorithm

The pathfinding algorithm is **fully abstracted** behind an interface:

```csharp
public interface IPathfinder
{
    PathResult FindPath(CosmicMap map, Astronaut astronaut);
}
```

`MissionController` depends only on `IPathfinder` — the algorithm can be swapped (e.g., A*, BFS) **without changing any other class**:

```csharp
var controller = new MissionController(new DijkstraPathfinder());
// Could just as easily be: new MissionController(new AStarPathfinder());
```

---

### ✅ 3. Email Report via SMTP

After results are displayed, the application offers to send a full mission report via email:

```
Send mission report via email? (y/n): y
Sender email:    mission@gmail.com
Sender password: ****************
Receiver email:  control@example.com
SMTP host:       smtp.gmail.com
SMTP port:       587
```

The email contains a formatted plain-text report with all astronaut paths and map visualizations.

**Email Screenshot:**

![Mission Report Email](email_screenshot.png)

> **Gmail users:** Use an [App Password](https://myaccount.google.com/apppasswords)

| Provider | SMTP Host | Port |
|----------|-----------|------|
| Gmail | `smtp.gmail.com` | `587` |
| Outlook | `smtp.office365.com` | `587` |
| Yahoo | `smtp.mail.yahoo.com` | `587` |

---

### ✅ 4. Dynamic Random Map Generation

Users can generate a random cosmic navigation map by specifying:
- Grid dimensions (rows × columns)
- Number of asteroids (`X`)
- Number of debris cells (`D`)
- Number of astronauts (1–3)

The generator uses retry logic to guarantee **100% solvability** — it will never produce a map where an astronaut is trapped.

---

## Architecture & OOP Design

The solution follows clean OOP principles throughout:

| Principle | Implementation |
|-----------|----------------|
| **Single Responsibility** | Each class has one clear purpose (`MapParser`, `MapGenerator`, `ConsoleRenderer`, etc.) |
| **Open/Closed** | New pathfinding algorithms can be added without modifying existing code |
| **Dependency Inversion** | `MissionController` depends on `IPathfinder` abstraction, not concrete implementation |
| **Encapsulation** | `CosmicMap` exposes only necessary grid operations; internal state is private |
| **Separation of Concerns** | UI (`ConsoleRenderer`), logic (`MissionController`), and data (`Models`) are fully separated |

### Algorithm: Dijkstra's Shortest Path

- Uses a **min-heap priority queue** (`PriorityQueue<T, TPriority>`) for O((V + E) log V) performance
- Handles **weighted edges** (debris = cost 2, others = cost 1)
- **Early termination** when the Space Station is reached
- Path **reconstruction** via predecessor tracking

---

## Error Handling

The application handles all edge cases gracefully:

- Invalid symbols in map input
- Wrong number of columns per row
- Missing Space Station or S1 astronaut
- Duplicate astronaut symbols
- No valid path for an astronaut → `"Mission failed — Astronaut [S1/S2/S3] lost in space!"`
- SMTP connection failures
- Map generation timeout (> 1000 attempts with impossible constraints)
- All numeric inputs are validated with clear re-prompt messages

---

*© 2026 — Yuluan Yuriev
