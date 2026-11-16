using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Infrastructure.Persistence;

public sealed class JsonRubricRepository(string filePath) : IRubricRepository
{
    private readonly JsonFile<RubricDto> _file = new(filePath);

    public Rubric Get()
    {
        var dto = _file.Read();
        if (dto is null)
        {
            return new Rubric();
        }

        try
        {
            return Mappers.ToDomain(dto);
        }
        catch (Exception ex) when (ex is DomainException or ArgumentException or InvalidDataException)
        {
            throw new InvalidDataException($"Файл «{_file.Path}» содержит недопустимые данные: {ex.Message}", ex);
        }
    }

    public void Save(Rubric rubric) => _file.Write(Mappers.ToDto(rubric));
}
