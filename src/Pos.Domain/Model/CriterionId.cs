namespace Pos.Domain.Model;

/// <summary>Номер критерия внутри рубрикатора. Выдаёт сам <see cref="Rubric"/>.</summary>
public sealed record CriterionId(int Value)
{
    public override string ToString() => Value.ToString();
}
