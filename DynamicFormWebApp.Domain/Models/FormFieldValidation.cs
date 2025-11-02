namespace DynamicFormWebApp.Domain.Models;

public class FormFieldValidation
{
    public Guid Id { get; set; }

    public Guid FormFieldId { get; set; }
    public virtual FormField? Field { get; set; }

    // e.g. "Required", "MinLength", "MaxValue", "Regex"
    public string RuleType { get; set; } = "";

    // e.g. "10" for MaxLength=10 or pattern for Regex
    public string? RuleValue { get; set; }

    public string? ErrorMessage { get; set; }
}