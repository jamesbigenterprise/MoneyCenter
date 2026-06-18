namespace MoneyCenter.Services;

public interface IConfirmationService
{
    Task<bool> ConfirmAsync(string title, string message, string confirmText = "Delete");
}
