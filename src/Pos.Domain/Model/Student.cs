namespace Pos.Domain.Model;

public sealed class Student(StudentId id, string name)
{
    private readonly List<Evaluation> _evaluations = [];

    public StudentId Id { get; } = id;

    public string Name { get; } = Text.Required(name, "ФИО студента");

    public IReadOnlyList<Evaluation> Evaluations => _evaluations.AsReadOnly();

    public Evaluation? FindEvaluation(CriterionId criterionId) =>
        _evaluations.FirstOrDefault(e => e.CriterionId == criterionId);

    public void Record(Evaluation evaluation)
    {
        if (FindEvaluation(evaluation.CriterionId) is not null)
        {
            throw new DomainException(
                $"У студента №{Id} уже есть оценка по критерию №{evaluation.CriterionId}");
        }

        _evaluations.Add(evaluation);
    }

    public void Correct(Evaluation evaluation)
    {
        _evaluations[IndexOf(evaluation.CriterionId)] = evaluation;
    }

    public void Cancel(CriterionId criterionId) => _evaluations.RemoveAt(IndexOf(criterionId));

    private int IndexOf(CriterionId criterionId)
    {
        var index = _evaluations.FindIndex(e => e.CriterionId == criterionId);
        return index >= 0
            ? index
            : throw new DomainException($"У студента №{Id} нет оценки по критерию №{criterionId}");
    }
}
