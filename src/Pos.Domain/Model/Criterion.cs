namespace Pos.Domain.Model;

public sealed class Criterion
{
    internal Criterion(CriterionId id, string name, int maxScore, Weight weight)
    {
        if (maxScore <= 0)
        {
            throw new DomainException($"Максимальный балл должен быть больше 0: {maxScore}");
        }

        Id = id;
        Name = Text.Required(name, "Название критерия");
        MaxScore = maxScore;
        Weight = weight;
    }

    public CriterionId Id { get; }

    public string Name { get; }

    public int MaxScore { get; }

    public Weight Weight { get; }

    public void EnsureAccepts(Score score)
    {
        if (score.Value > MaxScore)
        {
            throw new DomainException(
                $"Балл {score} вне пределов критерия «{Name}»: допустимо от 0 до {MaxScore}");
        }
    }

    public decimal ShareOf(Score score) => (decimal)score.Value / MaxScore;
}
