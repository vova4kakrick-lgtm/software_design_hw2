using Pos.Domain.Model;
using Pos.Infrastructure.Persistence;
using static Pos.Tests.TestData;

namespace Pos.Tests.Infrastructure;

public sealed class JsonRepositoryTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("pos-tests-").FullName;

    private string RubricPath => Path.Combine(_dir, "rubric.json");

    private string StudentsPath => Path.Combine(_dir, "students.json");

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void NoFiles_MeansEmptyData()
    {
        var rubrics = new JsonRubricRepository(RubricPath);
        var students = new JsonStudentRepository(StudentsPath, rubrics);

        Assert.Empty(rubrics.Get().Criteria);
        Assert.Empty(students.GetAll());
        Assert.Null(students.Find(new StudentId(1)));
        Assert.Equal(new StudentId(1), students.NextId());
    }

    [Fact]
    public void NextId_FollowsSavedStudents()
    {
        var students = new JsonStudentRepository(StudentsPath, new JsonRubricRepository(RubricPath));
        students.Save(new Student(new StudentId(1), "Иванов Иван"));
        students.Save(new Student(new StudentId(2), "Петрова Анна"));

        Assert.Equal(new StudentId(3), students.NextId());
    }

    [Fact]
    public void SavedData_IsReadBackByNewRepositories()
    {
        var rubric = TestData.Rubric();
        var student = Ivanov();
        student.Record(Evaluation(rubric, Hw, 8, "аккуратно"));
        var rubrics = new JsonRubricRepository(RubricPath);
        rubrics.Save(rubric);
        new JsonStudentRepository(StudentsPath, rubrics).Save(student);

        var reloadedRubrics = new JsonRubricRepository(RubricPath);
        var reloaded = new JsonStudentRepository(StudentsPath, reloadedRubrics).Find(new StudentId(1));

        Assert.Equal([Hw, Exam], reloadedRubrics.Get().Criteria.Select(c => c.Id));
        Assert.NotNull(reloaded);
        Assert.Equal("Иванов Иван", reloaded.Name);
        Assert.Equal(Evaluation(rubric, Hw, 8, "аккуратно"), Assert.Single(reloaded.Evaluations));
    }

    [Fact]
    public void Save_UpdatesExistingStudent()
    {
        var rubrics = new JsonRubricRepository(RubricPath);
        var rubric = TestData.Rubric();
        rubrics.Save(rubric);
        var students = new JsonStudentRepository(StudentsPath, rubrics);
        var student = Ivanov();
        students.Save(student);

        student.Record(Evaluation(rubric, Exam, 70));
        students.Save(student);

        Assert.Single(students.GetAll());
        Assert.Equal(70, students.Find(new StudentId(1))?.FindEvaluation(Exam)?.Score.Value);
    }

    [Fact]
    public void BrokenJson_IsReported()
    {
        File.WriteAllText(RubricPath, "{ not json");

        Assert.Throws<InvalidDataException>(() => new JsonRubricRepository(RubricPath).Get());
    }

    [Fact]
    public void CriteriaOutOfOrder_AreRejectedOnLoad()
    {
        File.WriteAllText(RubricPath, """
            { "criteria": [ { "id": 2, "name": "Экзамен", "maxScore": 100, "weight": 1 } ] }
            """);

        Assert.Throws<InvalidDataException>(() => new JsonRubricRepository(RubricPath).Get());
    }

    [Fact]
    public void HandEditedScoreAboveMax_IsRejectedOnLoad()
    {
        var rubrics = new JsonRubricRepository(RubricPath);
        rubrics.Save(TestData.Rubric());
        File.WriteAllText(StudentsPath, """
            [
              {
                "id": 1,
                "name": "Иванов Иван",
                "evaluations": [
                  { "criterionId": 1, "score": 12, "date": "2026-11-16", "comment": "" }
                ]
              }
            ]
            """);

        Assert.Throws<InvalidDataException>(() => new JsonStudentRepository(StudentsPath, rubrics).GetAll());
    }
}
