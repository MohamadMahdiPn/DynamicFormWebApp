using Microsoft.AspNetCore.Identity;

namespace DynamicFormWebApp.Domain.Models;

public class ApplicationUser:IdentityUser<Guid>
{
    public string FullName { get; set; }
}