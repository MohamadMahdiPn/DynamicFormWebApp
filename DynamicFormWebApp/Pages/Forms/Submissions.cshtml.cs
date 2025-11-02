using DynamicFormWebApp.Domain.Models;
using DynamicFormWebApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DynamicFormWebApp.Pages.Forms
{
    public class SubmissionsModel : PageModel
    {
        #region Constructor
        private readonly IFormService _formService;

        public SubmissionsModel(IFormService formService)
        {
            _formService = formService;
        }
        #endregion

        public BaseForm? FormDef { get; set; }
        public List<FormSubmission>? Submissions { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            FormDef = await _formService.GetForm(id);
            // load form with its fields and submissions
           
            if (FormDef == null)
                return NotFound();

            Submissions = (await _formService.GetFormSubmittions(id)).ToList();

            return Page();
        }

        public Dictionary<string, string> ParseValues(string valuesJson)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(valuesJson);
                return dict ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
