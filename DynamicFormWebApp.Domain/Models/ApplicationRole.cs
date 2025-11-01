using Microsoft.AspNetCore.Identity;

namespace DynamicFormWebApp.Domain.Models;

public class ApplicationRole:IdentityRole<Guid>
{
    public string PersianName { get; set; }
}