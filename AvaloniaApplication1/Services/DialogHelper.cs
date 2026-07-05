using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaApplication1.Constants;
using AvaloniaApplication1.Resolvers;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Dialog;
using AvaloniaApplication1.ViewModels.Form;
using AvaloniaApplication1.Views;
using EditInstanceFormViewModel = AvaloniaApplication1.ViewModels.Instance.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Services;

public static class DialogHelper
{
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
            var dialogWindow = FormViewResolver.Resolve(form);
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