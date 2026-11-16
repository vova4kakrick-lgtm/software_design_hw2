using Pos.Domain.Model;

namespace Pos.Application.Students;

public sealed record StudentView(int Id, string Name)
{
    internal static StudentView From(Student student) => new(student.Id.Value, student.Name);
}
