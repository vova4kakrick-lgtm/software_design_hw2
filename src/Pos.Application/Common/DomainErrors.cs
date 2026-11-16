using Pos.Domain.Model;

namespace Pos.Application.Common;

/// <summary>
/// Переводит нарушения доменных правил в <see cref="UseCaseException"/>,
/// чтобы UI не зависел от исключений домена.
/// </summary>
internal static class DomainErrors
{
    public static T Translate<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (DomainException ex)
        {
            throw new UseCaseException(ex.Message, ex);
        }
    }

    public static void Translate(Action action) => Translate(() =>
    {
        action();
        return true;
    });
}
