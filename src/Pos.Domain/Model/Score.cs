namespace Pos.Domain.Model;

/// <summary>
/// Балл — value object: неотрицательное целое. Верхнюю границу знает только критерий,
/// поэтому её проверяет <see cref="Criterion.EnsureAccepts"/>.
/// </summary>
public sealed record Score
{
    public Score(int value)
    {
        if (value < 0)
        {
            throw new DomainException($"Балл не может быть отрицательным: {value}");
        }

        Value = value;
    }

    public int Value { get; }

    public override string ToString() => Value.ToString();
}
