namespace Pos.Domain.Services.Totals;

internal static class WeightedAverage
{
    private const decimal Scale = 10m;

    public static decimal Of(IReadOnlyCollection<TotalLine> lines)
    {
        if (lines.Count == 0)
        {
            return 0m;
        }

        var totalWeight = lines.Sum(l => l.Criterion.Weight.Value);
        var weightedShares = lines.Sum(l =>
            l.Score is null
                ? 0m
                : l.Criterion.ShareOf(l.Score) * l.Criterion.Weight.Value);

        return weightedShares / totalWeight * Scale;
    }
}
