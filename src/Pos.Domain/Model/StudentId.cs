namespace Pos.Domain.Model;

/// <summary>Номер студента. Выдаёт репозиторий: <see cref="Repositories.IStudentRepository.NextId"/>.</summary>
public sealed record StudentId(int Value)
{
    public override string ToString() => Value.ToString();
}
