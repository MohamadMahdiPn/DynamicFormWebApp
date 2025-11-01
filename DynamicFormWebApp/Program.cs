using DynamicFormWebApp.Domain;
using DynamicFormWebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configurations = builder.Configuration;
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.ConfigureDomainRegistrations(configurations);

#region Identity

builder.Services.ConfigureIdentityPolicies();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.AppendMigrations();
app.ApplyRoles();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
