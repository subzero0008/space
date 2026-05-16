namespace SpaceMission.Models;

/// <summary>
/// Represents an astronaut with a starting position on the cosmic map.
/// </summary>
public class Astronaut
{
    public string Id { get; }
    public int Row { get; }
    public int Col { get; }

    public Astronaut(string id, int row, int col)
    {
        Id = id;
        Row = row;
        Col = col;
    }

    public override string ToString() => $"Astronaut {Id} at ({Row}, {Col})";
}