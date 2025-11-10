namespace FGT.Models
{
    public class FormSectionViewModel
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public int GridColumns { get; set; } = 1;
        public List<FormFieldViewModel> Fields { get; set; } = [];
    }
}
