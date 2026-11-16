namespace Pos.Application.Common;

/// <summary>
/// Сценарий не выполнен из-за нарушенного правила или неверного запроса; сообщение — для пользователя.
/// Нарушения доменных правил приходят наружу только в таком виде.
/// </summary>
public sealed class UseCaseException(string message, Exception? inner = null) : Exception(message, inner);
