using DynamicFormWebApp.Domain.Models.Bases;

namespace DynamicFormWebApp.Domain.Models;

public class FormSubmission: BaseTable<Guid>
{
    public Guid FormId { get; set; }
    public virtual BaseForm? Form { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    public string SubmittedBy { get; set; } = ""; // optional user identifier
    public string ValuesJson { get; set; } = "{}"; // JSON: { "<fieldId>": "<value>" }
}
