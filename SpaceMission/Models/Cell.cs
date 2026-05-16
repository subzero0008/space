namespace SpaceMission.Models;

/// <summary>
/// Represents a single cell in the cosmic navigation map.
/// </summary>
public class Cell
{
	public int Row { get; }
	public int Col { get; }
	public string Symbol { get; set; }

	public bool IsAsteroid => Symbol == "X";
	public bool IsDebris => Symbol == "D";
	public bool IsOpenSpace => Symbol == "O";
	public bool IsStation => Symbol == "F";
	public bool IsAstronaut => Symbol is "S1" or "S2" or "S3";

	/// <summary>Movement cost: debris costs 2, everything else costs 1.</summary>
	public int MovementCost => IsDebris ? 2 : 1;

	public Cell(int row, int col, string symbol)
	{
		Row = row;
		Col = col;
		Symbol = symbol;
	}

	public override string ToString() => Symbol;
}