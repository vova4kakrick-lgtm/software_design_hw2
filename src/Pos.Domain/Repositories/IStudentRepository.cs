using Pos.Domain.Model;

namespace Pos.Domain.Repositories;

public interface IStudentRepository
{
    /// <summary>Следующий свободный номер студента.</summary>
    StudentId NextId();

    IReadOnlyList<Student> GetAll();

    Student? Find(StudentId id);

    void Save(Student student);
}
