using DynamicFormWebApp.Domain.Enums;
using DynamicFormWebApp.Domain.Models;
using DynamicFormWebApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DynamicFormWebApp.Pages.Forms
{
    public class FillFormModel : PageModel
    {

        private readonly IFormService _formService;

        public FillFormModel(IFormService formService)
        {
            _formService = formService;
        }

        public BaseForm? Form { get; set; }

        [BindProperty]
        public Dictionary<string, string?> SubmittedValues { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Form = await _formService.GetForm(id);
            if (Form == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            Form = await _formService.GetForm(id);
            if (Form == null)
                return NotFound();

            // --- Normalize field data (ensure every field exists in SubmittedValues) ---
            foreach (var field in Form.Fields)
            {
                if (!SubmittedValues.ContainsKey(field.Name))
                    SubmittedValues[field.Name] = null;
            }

            // --- Dynamic server-side validation ---
            foreach (var field in Form.Fields.OrderBy(f => f.Order))
            {
                var value = SubmittedValues[field.Name];

                // Handle checkbox (unchecked means no key in form)
                if (field.Type == FieldType.Checkbox && value == null)
                    value = "false";

                // Required check
                if (field.IsRequired && string.IsNullOrWhiteSpace(value))
                {
                    ModelState.AddModelError($"SubmittedValues[{field.Name}]", $"{field.Label} is required.");
                    continue;
                }

                // Skip further validation if empty and not required
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                // --- Custom validation rules ---
                if (field.Validations != null && field.Validations.Any())
                {
                    foreach (var rule in field.Validations)
                    {
                        var ruleType = rule.RuleType?.Trim().ToLowerInvariant() ?? "";
                        var ruleValue = rule.RuleValue ?? "";
                        var errorMessage = string.IsNullOrWhiteSpace(rule.ErrorMessage)
                            ? $"{field.Label} is invalid."
                            : rule.ErrorMessage;

                        try
                        {
                            switch (ruleType)
                            {
                                case "minlength":
                                    if (value!.Length < int.Parse(ruleValue))
                                        ModelState.AddModelError($"SubmittedValues[{field.Name}]", errorMessage);
                                    break;

                                case "maxlength":
                                    if (value!.Length > int.Parse(ruleValue))
                                        ModelState.AddModelError($"SubmittedValues[{field.Name}]", errorMessage);
                                    break;

                                case "minvalue":
                                    if (decimal.TryParse(value, out var valMin) && valMin < decimal.Parse(ruleValue))
                                        ModelState.AddModelError($"SubmittedValues[{field.Name}]", errorMessage);
                                    break;

                                case "maxvalue":
                                    if (decimal.TryParse(value, out var valMax) && valMax > decimal.Parse(ruleValue))
                                        ModelState.AddModelError($"SubmittedValues[{field.Name}]", errorMessage);
                                    break;

                                case "regex":
                                    if (!Regex.IsMatch(value!, ruleValue))
                                        ModelState.AddModelError($"SubmittedValues[{field.Name}]", errorMessage);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            // if the admin misconfigured a rule (e.g., wrong regex)
                            ModelState.AddModelError($"SubmittedValues[{field.Name}]",
                                $"{field.Label}: invalid rule ({ruleType}) - {ex.Message}");
                        }
                    }
                }
            }

            // --- Validation summary ---
            if (!ModelState.IsValid)
            {
                // re-display the same form with validation messages
                return Page();
            }

            // --- Save submission ---
            var submission = new FormSubmission
            {
                FormId = Form.Id,
                ValuesJson = JsonSerializer.Serialize(SubmittedValues),
                SubmittedAt = DateTime.Now,
                CreatedDate = DateTime.Now,
                 SubmittedBy = User?.Identity?.Name ?? "Anonymous",
                 
            };
            await _formService.SubmitAForm(submission);

            TempData["SuccessMessage"] = "Your response has been submitted successfully!";
            return RedirectToPage("/Forms/list");
        }
    }
}
