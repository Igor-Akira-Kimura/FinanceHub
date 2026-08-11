namespace FinanceHub.Api.ExceptionHandling;

public interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    Task HandleAsync(
        HttpContext context,
        Exception exception);
}