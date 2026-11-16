using Pos.Domain.Model;

namespace Pos.Application.Rubrics;

public sealed record CriterionView(int Id, string Name, int MaxScore, decimal Weight)
{
    internal static CriterionView From(Criterion criterion) =>
        new(criterion.Id.Value, criterion.Name, criterion.MaxScore, criterion.Weight.Value);
}
