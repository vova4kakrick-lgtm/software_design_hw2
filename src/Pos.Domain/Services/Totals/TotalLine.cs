using Pos.Domain.Model;

namespace Pos.Domain.Services.Totals;

public sealed record TotalLine(Criterion Criterion, Score? Score);
