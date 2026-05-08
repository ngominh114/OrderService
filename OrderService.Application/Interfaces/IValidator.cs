namespace OrderService.Application.Interfaces;

public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationFailure> Failures { get; set; } = new();
}

public class ValidationFailure
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
