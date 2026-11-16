namespace Pos.Domain.Services.Totals;

/// <summary>Strategy: сменное правило, по которому строки итога превращаются в балл по шкале 0–10.</summary>
public interface ITotalStrategy
{
    decimal Calculate(IReadOnlyList<TotalLine> lines);
}
