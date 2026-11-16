namespace Pos.Domain.Services.Totals;

public sealed record StudentTotal(decimal Final, IReadOnlyList<TotalLine> Lines);
