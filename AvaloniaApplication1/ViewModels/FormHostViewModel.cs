using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public abstract partial class FormHostViewModel<TForm>(TForm form) : ObservableObject 
    where TForm : FormViewModelBase
{
    public TForm Form => form;

    protected abstract void OnSaved();
    protected abstract void OnCanceled();

    [RelayCommand]
    private void Save()
    {
        if (!form.Validate())
            return;
        OnSaved();
    }
    
    [RelayCommand]
    private void Cancel() => OnCanceled();
}
