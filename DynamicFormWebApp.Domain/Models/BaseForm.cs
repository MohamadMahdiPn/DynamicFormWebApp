using DynamicFormWebApp.Domain.Models.Bases;

namespace DynamicFormWebApp.Domain.Models;

public class BaseForm : BaseTable<Guid>
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<FormField> Fields { get; set; } = new List<FormField>();
}