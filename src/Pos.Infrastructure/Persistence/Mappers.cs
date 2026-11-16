using Pos.Domain.Model;

namespace Pos.Infrastructure.Persistence;

internal static class Mappers
{
    public static RubricDto ToDto(Rubric rubric) =>
        new(rubric.Criteria
            .Select(c => new CriterionDto(c.Id.Value, c.Name, c.MaxScore, c.Weight.Value))
            .ToList());

    public static Rubric ToDomain(RubricDto dto)
    {
        var rubric = new Rubric();
        foreach (var c in dto.Criteria ?? [])
        {
            // Номера выдаёт рубрикатор; критерии не удаляются, поэтому при добавлении по порядку номера совпадают.
            var restored = rubric.AddCriterion(c.Name, c.MaxScore, new Weight(c.Weight));
            if (restored.Id.Value != c.Id)
            {
                throw new InvalidDataException($"ожидался критерий №{restored.Id}, а в файле №{c.Id}");
            }
        }

        return rubric;
    }

    public static StudentDto ToDto(Student student) =>
        new(student.Id.Value,
            student.Name,
            student.Evaluations
                .Select(e => new EvaluationDto(e.CriterionId.Value, e.Score.Value, e.Date, e.Comment))
                .ToList());

    public static Student ToDomain(StudentDto dto, Rubric rubric)
    {
        var student = new Student(new StudentId(dto.Id), dto.Name);
        foreach (var e in dto.Evaluations ?? [])
        {
            student.Record(rubric.Evaluate(new CriterionId(e.CriterionId), new Score(e.Score), e.Date, e.Comment));
        }

        return student;
    }
}
