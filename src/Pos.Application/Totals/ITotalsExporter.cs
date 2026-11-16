namespace Pos.Application.Totals;

/// <summary>Порт выгрузки итогов. Формат (JSON, CSV, …) — забота адаптера в Infrastructure.</summary>
public interface ITotalsExporter
{
    /// <returns>Где теперь лежат итоги — чтобы показать пользователю.</returns>
    string Export(TotalsReport report);
}
