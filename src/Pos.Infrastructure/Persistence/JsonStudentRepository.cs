using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Infrastructure.Persistence;

public sealed class JsonStudentRepository(string filePath, IRubricRepository rubrics) : IStudentRepository
{
    private readonly JsonFile<List<StudentDto>> _file = new(filePath);

    public StudentId NextId() => new(Load().Select(d => d.Id).DefaultIfEmpty(0).Max() + 1);

    public IReadOnlyList<Student> GetAll()
    {
        var rubric = rubrics.Get();
        return Load().Select(dto => ToDomain(dto, rubric)).ToList();
    }

    public Student? Find(StudentId id)
    {
        var dto = Load().FirstOrDefault(d => d.Id == id.Value);
        return dto is null ? null : ToDomain(dto, rubrics.Get());
    }

    public void Save(Student student)
    {
        var dtos = Load();
        var index = dtos.FindIndex(d => d.Id == student.Id.Value);
        var dto = Mappers.ToDto(student);

        if (index >= 0)
        {
            dtos[index] = dto;
        }
        else
        {
            dtos.Add(dto);
        }

        _file.Write(dtos);
    }

    private List<StudentDto> Load() => _file.Read() ?? [];

    private Student ToDomain(StudentDto dto, Rubric rubric)
    {
        try
        {
            return Mappers.ToDomain(dto, rubric);
        }
        catch (Exception ex) when (ex is DomainException or ArgumentException)
        {
            throw new InvalidDataException(
                $"Файл «{_file.Path}», студент №{dto.Id}: недопустимые данные — {ex.Message}", ex);
        }
    }
}
