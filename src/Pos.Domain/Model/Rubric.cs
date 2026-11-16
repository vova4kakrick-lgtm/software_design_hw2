namespace Pos.Domain.Model;

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = [];

    public IReadOnlyList<Criterion> Criteria => _criteria.AsReadOnly();

    public Criterion AddCriterion(string name, int maxScore, Weight weight)
    {
        var id = new CriterionId(_criteria.Count == 0 ? 1 : _criteria.Max(c => c.Id.Value) + 1);
        var criterion = new Criterion(id, name, maxScore, weight);

        _criteria.Add(criterion);
        return criterion;
    }

    public Criterion? Find(CriterionId id) => _criteria.FirstOrDefault(c => c.Id == id);

    public Evaluation Evaluate(CriterionId criterionId, Score score, DateOnly date, string? comment)
    {
        var criterion = Find(criterionId)
            ?? throw new DomainException($"Нельзя оценить критерий №{criterionId}: его нет в рубрикаторе");

        criterion.EnsureAccepts(score);

        return new Evaluation(criterion.Id, score, date, comment);
    }
}
