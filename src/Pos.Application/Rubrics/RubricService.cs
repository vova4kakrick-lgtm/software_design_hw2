using Pos.Application.Common;
using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Application.Rubrics;

public sealed class RubricService(IRubricRepository rubrics)
{
    public CriterionView AddCriterion(string name, int maxScore, decimal weight) =>
        DomainErrors.Translate(() =>
        {
            var rubric = rubrics.Get();
            var criterion = rubric.AddCriterion(name, maxScore, new Weight(weight));
            rubrics.Save(rubric);

            return CriterionView.From(criterion);
        });

    public IReadOnlyList<CriterionView> List() =>
        rubrics.Get().Criteria.Select(CriterionView.From).ToList();
}
