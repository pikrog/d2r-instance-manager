using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;

namespace AvaloniaApplication1.Services;

public static class DialogHelper
{
    public static Window ResolveFormView(IFormViewModel form)
    {
        return form switch
        {
            EditInstanceFormViewModel => new EditInstanceForm(),
            _ => throw new NotImplementedException($"Form view not mapped: {form.GetType().FullName}")
        };
    }

    public static IDialogAware ResolveFormDialogViewModel(IFormViewModel form)
    {
        return form switch
        {
            EditInstanceFormViewModel vm => new EditInstanceFormDialogViewModel(vm),
            _ => throw new NotImplementedException($"Form dialog view model not mapped: {form.GetType().FullName}")
        };
    }
    
    extension(IDialogParticipant participant)
    {
        public Task<T> OpenDialog<T>(Window dialog)
        {
            var mainWindow = DialogService.GetMainWindow(participant);
            ArgumentNullException.ThrowIfNull(mainWindow);
            return dialog.ShowDialog<T>(mainWindow);
        }

        public Task<bool> OpenForm(IFormViewModel form)
        {
            var dialogViewModel = ResolveFormDialogViewModel(form);
            var dialogWindow = ResolveFormView(form);
            dialogWindow.DataContext = dialogViewModel;
            dialogViewModel.CloseDialog = result => dialogWindow.Close(result);
            return participant.OpenDialog<bool>(dialogWindow);
        }
    }
}