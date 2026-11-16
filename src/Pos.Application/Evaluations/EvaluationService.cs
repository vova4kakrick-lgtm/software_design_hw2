using Pos.Application.Common;
using Pos.Domain.Model;
using Pos.Domain.Repositories;

namespace Pos.Application.Evaluations;

public sealed class EvaluationService(
    IStudentRepository students,
    IRubricRepository rubrics,
    TimeProvider time)
{
    public EvaluationView Record(int studentId, int criterionId, int score, string? comment = null) =>
        DomainErrors.Translate(() =>
        {
            var student = students.GetRequired(new StudentId(studentId));
            var rubric = rubrics.Get();
            var evaluation = Evaluate(rubric, criterionId, score, comment);

            student.Record(evaluation);
            students.Save(student);
            return ToView(student, rubric, evaluation);
        });

    public EvaluationView Correct(int studentId, int criterionId, int score, string? comment = null) =>
        DomainErrors.Translate(() =>
        {
            var student = students.GetRequired(new StudentId(studentId));
            var rubric = rubrics.Get();
            var evaluation = Evaluate(rubric, criterionId, score, comment);

            student.Correct(evaluation);
            students.Save(student);
            return ToView(student, rubric, evaluation);
        });

    public void Cancel(int studentId, int criterionId) =>
        DomainErrors.Translate(() =>
        {
            var student = students.GetRequired(new StudentId(studentId));
            student.Cancel(new CriterionId(criterionId));
            students.Save(student);
        });

    public IReadOnlyList<EvaluationView> ListAll()
    {
        var rubric = rubrics.Get();
        return students.GetAll()
            .SelectMany(s => s.Evaluations.Select(e => ToView(s, rubric, e)))
            .ToList();
    }

    private Evaluation Evaluate(Rubric rubric, int criterionId, int score, string? comment)
    {
        var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);
        return rubric.Evaluate(new CriterionId(criterionId), new Score(score), today, comment);
    }

    private static EvaluationView ToView(Student student, Rubric rubric, Evaluation evaluation)
    {
        var criterion = rubric.Find(evaluation.CriterionId)!;
        return new EvaluationView(
            student.Id.Value,
            student.Name,
            criterion.Id.Value,
            criterion.Name,
            evaluation.Score.Value,
            criterion.MaxScore,
            evaluation.Date,
            evaluation.Comment);
    }
}
