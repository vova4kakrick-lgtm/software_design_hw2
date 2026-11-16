using Pos.Domain.Services.Totals;

namespace Pos.Application.Totals;

/// <summary>
/// Фабрика правила итога: единственное место, где выбор пользователя (правило + округление)
/// превращается в готовую стратегию. Базовые стратегии приходят из DI, декоратор навешивается по запросу.
/// </summary>
public sealed class TotalRuleFactory(StrictTotalStrategy strict, CurrentTotalStrategy current)
{
    public ITotalStrategy Create(TotalMode mode, bool rounded)
    {
        ITotalStrategy rule = mode switch
        {
            TotalMode.Strict => strict,
            TotalMode.Current => current,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Неизвестное правило итога"),
        };

        return rounded ? new RoundedTotalStrategy(rule) : rule;
    }
}
