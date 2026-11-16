using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Application.Common;

internal static class RepositoryExtensions
{
    public static Student GetRequired(this IStudentRepository students, StudentId id) =>
        students.Find(id) ?? throw new UseCaseException($"Студент №{id} не найден");
}
