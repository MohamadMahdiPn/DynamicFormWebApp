using DynamicFormWebApp.Domain.Data;
using DynamicFormWebApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormWebApp.Domain.Services
{
    public class FormService : IFormService
    {
        private ApplicationDbContext _context;

        public FormService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> AddNew(BaseForm form)
        {
            _context.Forms.Add(form);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<BaseForm>> GetForms()
        {
            var data = await _context.Forms.ToListAsync();
            return data;
        }
        public async Task<BaseForm?> GetForm(Guid id)
        {
            var formDef = await _context.Forms
            .Where(f => f.Id == id)
            .Select(f => new BaseForm
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                Fields = f.Fields.OrderBy(ff => ff.Order).ToList()
            })
            .FirstOrDefaultAsync();

            return formDef;
        }
        public async Task<IEnumerable<FormSubmission>> GetFormSubmittions(Guid formId)
        {
           var data =  await _context.FormSubmissions
                .Where(s => s.FormId == formId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return data;
        }

        public async Task<bool> SubmitAForm(FormSubmission formSubmission)
        {
            _context.FormSubmissions.Add(formSubmission);
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public interface IFormService
    {
        Task<bool> AddNew(BaseForm form);
        Task<IEnumerable<BaseForm>> GetForms();
        Task<BaseForm?> GetForm(Guid id);
        Task<bool> SubmitAForm(FormSubmission formSubmission);
        Task<IEnumerable<FormSubmission>> GetFormSubmittions(Guid formId);
    }


}
