using DynamicFormWebApp.Domain.Models;
using DynamicFormWebApp.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DynamicFormWebApp.Pages.Forms
{
    public class ListModel : PageModel
    {
        #region Constructor
        private readonly IFormService _formService;
        public ListModel(IFormService formService)
        {
            _formService = formService;
           
        }

        #endregion
        public List<BaseForm> Forms { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            var items = await _formService.GetForms();
            Forms = items.ToList();
            return Page();
        }
    }
}
