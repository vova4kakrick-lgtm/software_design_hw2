namespace Pos.Application.Totals;

/// <summary>Итоги всех студентов по одному правилу.</summary>
public sealed record TotalsReport(TotalMode Mode, bool Rounded, IReadOnlyList<TotalView> Totals);

public sealed record TotalView(
    int StudentId,
    string StudentName,
    decimal Final,
    IReadOnlyList<TotalLineView> Lines);

public sealed record TotalLineView(int CriterionId, string CriterionName, int? Score, int MaxScore);

/// <summary>Результат выгрузки: куда легли итоги и что именно выгружено.</summary>
public sealed record ExportResult(string Location, TotalsReport Report);
