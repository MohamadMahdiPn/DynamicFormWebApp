using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormWebApp.Domain.Dtos
{
    public class BaseFormDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
  
    public class FormFieldDto
    {
        public string? Label { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = "Text";
        public bool IsRequired { get; set; }
        public List<string>? Options { get; set; }
        public List<ValidationDto>? Validations { get; set; }
        public int Order { get; set; }
    }

    public class ValidationDto
    {
        public string RuleType { get; set; } = "";
        public string? RuleValue { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
