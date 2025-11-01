namespace DynamicFormWebApp.Domain.Models.Bases;

public abstract class BaseTable<IdType>
{
    public IdType Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}