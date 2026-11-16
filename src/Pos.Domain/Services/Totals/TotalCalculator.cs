using Pos.Domain.Model;

namespace Pos.Domain.Services.Totals;

public sealed class TotalCalculator
{
    public StudentTotal Calculate(Student student, Rubric rubric, ITotalStrategy strategy)
    {
        if (rubric.Criteria.Count == 0)
        {
            throw new DomainException("Рубрикатор пуст: итог не из чего считать");
        }

        var lines = rubric.Criteria
            .Select(c => new TotalLine(c, student.FindEvaluation(c.Id)?.Score))
            .ToList();

        return new StudentTotal(strategy.Calculate(lines), lines);
    }
}
