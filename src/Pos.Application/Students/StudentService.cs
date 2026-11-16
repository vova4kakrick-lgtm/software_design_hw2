using Pos.Application.Common;
using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Application.Students;

public sealed class StudentService(IStudentRepository students)
{
    public StudentView Add(string name) =>
        DomainErrors.Translate(() =>
        {
            var student = new Student(students.NextId(), name);
            students.Save(student);
            return StudentView.From(student);
        });

    public IReadOnlyList<StudentView> List() =>
        students.GetAll().Select(StudentView.From).ToList();
}
