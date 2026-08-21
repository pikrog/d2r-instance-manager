using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaApplication1.Form;
using AvaloniaApplication1.Form.ViewModels;

namespace AvaloniaApplication1.Dialog;

public static class DialogHelper
{
    private static readonly ViewLocator ViewLocator = new();
    
    private static Window ResolveFormWindow(IFormViewModel form)
    {
        var control = ViewLocator.Build(form);
        return control switch
        {
            Window window => window,
            _ => throw new InvalidOperationException($"Form view {form.GetType().FullName} is not a window")
        };
    }
    
    extension(IDialogParticipant participant)
    {
        private Window GetMainWindow() => DialogService.GetMainWindow(participant) ?? throw new InvalidOperationException($"Main window for participant {participant.GetType().Name} not found");
        
        public Task<T> OpenDialog<T>(Window dialog)
        {
            var mainWindow = participant.GetMainWindow();
            return dialog.ShowDialog<T>(mainWindow);
        }

        public Task<bool> OpenForm(IFormViewModel form)
        {
            var dialogViewModel = FormDialogViewModelResolver.Resolve(form);
            var dialogWindow = ResolveFormWindow(form);
            dialogWindow.DataContext = dialogViewModel;
            dialogViewModel.CloseDialog = result => dialogWindow.Close(result);
            return participant.OpenDialog<bool>(dialogWindow);
        }
        
        public Task<IReadOnlyList<IStorageFile>> OpenFilePicker(FilePickerOpenOptions options)
        {
            var mainWindow = participant.GetMainWindow();
            return mainWindow.StorageProvider.OpenFilePickerAsync(options);
        }

        public async Task<string?> PickPathByOpenFilePicker(FilePickerOpenOptions options)
        {
            var files = await participant.OpenFilePicker(options);
            return files.Count > 0 ? files[0].Path.LocalPath : null;
        }
    }
}