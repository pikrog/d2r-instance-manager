using D2RInstanceManager.Common;

namespace D2RInstanceManager.Form.ViewModels;

public interface IFormViewModel : IViewModel
{
    bool Validate();
}