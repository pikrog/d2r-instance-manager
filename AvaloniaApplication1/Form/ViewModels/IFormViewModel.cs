using AvaloniaApplication1.Common;

namespace AvaloniaApplication1.Form.ViewModels;

public interface IFormViewModel : IViewModel
{
    bool Validate();
}