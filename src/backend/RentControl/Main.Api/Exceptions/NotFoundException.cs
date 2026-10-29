namespace Main.Api.Exceptions;

/// <summary>
/// Выбрасывается, когда сущность не найдена. Обрабатывается middleware как 404.
/// </summary>
public class NotFoundException(string message) : Exception(message);
