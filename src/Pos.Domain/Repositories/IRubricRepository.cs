using Pos.Domain.Model;

namespace Pos.Domain.Repositories;

public interface IRubricRepository
{
    Rubric Get();

    void Save(Rubric rubric);
}
