namespace Pos.Domain.Services.Totals;

/// <summary>
/// Decorator: округляет итог любого правила до целого, не меняя само правило.
/// </summary>
public sealed class RoundedTotalStrategy(ITotalStrategy inner) : ITotalStrategy
{
    public decimal Calculate(IReadOnlyList<TotalLine> lines) =>
        Math.Round(inner.Calculate(lines), MidpointRounding.AwayFromZero);
}
