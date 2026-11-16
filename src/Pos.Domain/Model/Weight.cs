namespace Pos.Domain.Model;

/// <summary>
/// Вес критерия в итоге — value object. Веса относительные, поэтому верхняя граница нужна
/// не для смысла, а чтобы сумма весов всего рубрикатора гарантированно помещалась в decimal.
/// </summary>
public sealed record Weight
{
    public const decimal Max = 1000m;

    public Weight(decimal value)
    {
        if (value <= 0 || value > Max)
        {
            throw new DomainException($"Вес критерия должен быть больше 0 и не больше {Max}: {value}");
        }

        Value = value;
    }

    public decimal Value { get; }
}
