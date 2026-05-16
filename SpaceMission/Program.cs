using SpaceMission.Models;
using SpaceMission.Pathfinding;
using SpaceMission.Services;
using SpaceMission.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

ConsoleRenderer.PrintBanner();

while (true)
{
    ConsoleRenderer.PrintSectionHeader("MAIN MENU");
    Console.WriteLine("  [1] Enter cosmic map manually");
    Console.WriteLine("  [2] Generate random cosmic map");
    Console.WriteLine("  [0] Exit mission");
    ConsoleRenderer.PrintPrompt("\nSelect option: ");

    string? choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            RunManualMode();
            break;
        case "2":
            RunGeneratorMode();
            break;
        case "0":
            ConsoleRenderer.PrintInfo("\n🚀 Mission HQ signing off. Safe travels!\n");
            return;
        default:
            ConsoleRenderer.PrintError("Invalid option. Please enter 1, 2, or 0.");
            break;
    }
}

// ── Manual input mode ────────────────────────────────────────────────────────

static void RunManualMode()
{
    try
    {
        int rows = ReadPositiveInt("Map rows (2-100): ", 2, 100);
        int cols = ReadPositiveInt("Map columns (2-100): ", 2, 100);

        Console.WriteLine($"\nEnter {rows} map rows. Each row must have {cols} space-separated symbols.");
        Console.WriteLine("Valid symbols: S1 S2 S3  F  O  X  D\n");

        var lines = new string[rows];
        for (int i = 0; i < rows; i++)
        {
            ConsoleRenderer.PrintPrompt($"  Row {i + 1}: ");
            lines[i] = Console.ReadLine() ?? string.Empty;
        }

        var map = MapParser.Parse(rows, cols, lines);
        RunMission(map);
    }
    catch (Exception ex)
    {
        ConsoleRenderer.PrintError($"\n[!] Input error: {ex.Message}");
    }
}

// ── Random generation mode ───────────────────────────────────────────────────

static void RunGeneratorMode()
{
    try
    {
        int rows = ReadPositiveInt("Map rows (2-100): ", 2, 100);
        int cols = ReadPositiveInt("Map columns (2-100): ", 2, 100);
        int asteroids = ReadPositiveInt("Number of asteroids: ", 0, rows * cols - 2);
        int debris = ReadPositiveInt("Number of debris cells (D): ", 0, rows * cols - 2 - asteroids);
        int astCount = ReadPositiveInt("Number of astronauts (1-3): ", 1, 3);

        ConsoleRenderer.PrintInfo("\n[...] Generating solvable map...");
        var map = MapGenerator.Generate(rows, cols, asteroids, debris, astCount);
        ConsoleRenderer.PrintSuccess("[OK] Map generated successfully!\n");

        ConsoleRenderer.PrintSectionHeader("GENERATED MAP");
        PrintRawMap(map);

        RunMission(map);
    }
    catch (Exception ex)
    {
        ConsoleRenderer.PrintError($"\n[!] Generation error: {ex.Message}");
    }
}

// ── Core mission runner ──────────────────────────────────────────────────────

static void RunMission(CosmicMap map)
{
    var controller = new MissionController(new DijkstraPathfinder());
    var results = controller.ExecuteMission(map);

    ConsoleRenderer.PrintMissionResults(map, results);

    ConsoleRenderer.PrintPrompt("Send mission report via email? (y/n): ");
    if (Console.ReadLine()?.Trim().ToLower() == "y")
        TrySendEmail(map, results);
}

// ── Email ────────────────────────────────────────────────────────────────────

static void TrySendEmail(CosmicMap map, IReadOnlyList<SpaceMission.Pathfinding.PathResult> results)
{
    try
    {
        ConsoleRenderer.PrintPrompt("Sender email:    ");
        string sender = Console.ReadLine()?.Trim() ?? string.Empty;

        ConsoleRenderer.PrintPrompt("Sender password: ");
        string password = ReadPassword();

        ConsoleRenderer.PrintPrompt("Receiver email:  ");
        string receiver = Console.ReadLine()?.Trim() ?? string.Empty;

        ConsoleRenderer.PrintPrompt("SMTP host (e.g. smtp.gmail.com): ");
        string host = Console.ReadLine()?.Trim() ?? "smtp.gmail.com";

        int port = ReadPositiveInt("SMTP port (e.g. 587): ", 1, 65535);

        ConsoleRenderer.PrintInfo("\n[...] Sending email...");
        EmailReporter.SendReport(sender, password, receiver, host, port, map, results);
        ConsoleRenderer.PrintSuccess("[OK] Mission report sent successfully!");
    }
    catch (Exception ex)
    {
        ConsoleRenderer.PrintError($"\n[!] Email error: {ex.Message}");
    }
}

// ── Helpers ──────────────────────────────────────────────────────────────────

static int ReadPositiveInt(string prompt, int min, int max)
{
    while (true)
    {
        ConsoleRenderer.PrintPrompt(prompt);
        string? input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out int value) && value >= min && value <= max)
            return value;
        ConsoleRenderer.PrintError($"  Please enter a number between {min} and {max}.");
    }
}

static string ReadPassword()
{
    var sb = new System.Text.StringBuilder();
    ConsoleKeyInfo key;
    do
    {
        key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
            Console.Write("\b \b");
        }
        else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
        {
            sb.Append(key.KeyChar);
            Console.Write('*');
        }
    } while (key.Key != ConsoleKey.Enter);
    Console.WriteLine();
    return sb.ToString();
}

static void PrintRawMap(CosmicMap map)
{
    for (int r = 0; r < map.Rows; r++)
    {
        Console.Write("  ");
        var tokens = new List<string>();
        for (int c = 0; c < map.Cols; c++)
            tokens.Add(map[r, c].Symbol);
        Console.WriteLine(string.Join(" ", tokens));
    }
    Console.WriteLine();
}