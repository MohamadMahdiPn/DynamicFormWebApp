using DynamicFormWebApp.Domain.Enums;
using DynamicFormWebApp.Domain.Models.Bases;

namespace DynamicFormWebApp.Domain.Models;

public class FormField : BaseTable<Guid>
{
    public Guid FormId { get; set; }
    public virtual BaseForm? Form { get; set; }

    public string Label { get; set; } = "";
    public string Name { get; set; } = ""; // used to generate html name (unique or generated)
    public FieldType Type { get; set; }
    public bool IsRequired { get; set; }
    public string? OptionsJson { get; set; } // for Select/Radio - JSON array of option strings
    public int Order { get; set; }
    public virtual ICollection<FormFieldValidation> Validations { get; set; } = new List<FormFieldValidation>();

}