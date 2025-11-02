using DynamicFormWebApp.Domain.Dtos;
using DynamicFormWebApp.Domain.Enums;
using DynamicFormWebApp.Domain.Models;
using DynamicFormWebApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DynamicFormWebApp.Pages.Forms
{
    public class CreateFormModel : PageModel
    {
        #region Constructor
        private readonly IFormService _formService;
        public CreateFormModel(IFormService formService)
        {
            _formService = formService;
        }
        #endregion

        [BindProperty]
        public BaseFormDto BaseFormItem { get; set; } = new();

        [BindProperty]
        public string? FormName { get; set; }

        [BindProperty]
        public string? FieldsJson { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(BaseFormItem.Title)) ModelState.AddModelError(nameof(BaseFormItem.Title), "Title required");

            if (!ModelState.IsValid) return Page();

            var form = new BaseForm { Title = BaseFormItem.Title.Trim(), Description = BaseFormItem.Description?.Trim() ?? "" };

            // parse FieldsJson which is an array of field objects { label, name, type, isRequired, options (array), order }
            try
            {
                var fields = JsonSerializer.Deserialize<List<FormFieldDto>>(FieldsJson,new JsonSerializerOptions
{
                    PropertyNameCaseInsensitive = true
                }) ?? new();
                int order = 0;

                foreach (var f in fields)
                {
                    var field = new FormField
                    {
                        Label = f.Label ?? "Field",
                        Name = string.IsNullOrWhiteSpace(f.Name) ? $"f_{Guid.NewGuid():N}" : f.Name,
                        Type = Enum.Parse<FieldType>(f.Type, ignoreCase: true),
                        IsRequired = f.IsRequired,
                        OptionsJson = (f.Options != null && f.Options.Any())
                                        ? JsonSerializer.Serialize(f.Options)
                                        : null,
                        Order = ++order
                    };

                    if (f.Validations != null)
                    {
                        foreach (var v in f.Validations)
                        {
                            field.Validations.Add(new FormFieldValidation
                            {
                                RuleType = v.RuleType,
                                RuleValue = v.RuleValue,
                                ErrorMessage = v.ErrorMessage
                            });
                        }
                    }

                    form.Fields.Add(field);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Invalid fields payload: " + ex.Message);
                return Page();
            }

            await _formService.AddNew(form);

            return RedirectToPage("/Forms/List"); // or to details
        }
    }
}
