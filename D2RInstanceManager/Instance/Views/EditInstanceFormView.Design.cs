using D2RInstanceManager.Design.Services;

namespace D2RInstanceManager.Instance.Views;

public partial class EditInstanceFormView
{
    public EditInstanceFormView() : this(new FakeHotKeySuspensionCoordinator())
    {
        
    }
}