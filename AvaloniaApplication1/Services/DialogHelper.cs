using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaApplication1.Constants;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.ViewModels.Dialog;
using AvaloniaApplication1.ViewModels.Form;
using AvaloniaApplication1.Views;
using EditInstanceFormViewModel = AvaloniaApplication1.ViewModels.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Services;

public static class DialogHelper
{
    private const string FilePickerExecutableTypeName = "Executable files";
    private const string FilePickerExecutableTypeExtension = "*.exe";
    
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
        private Window GetMainWindow() => DialogService.GetMainWindow(participant) ?? throw new InvalidOperationException($"Main window for participant {participant.GetType().Name} not found");
        
        public Task<T> OpenDialog<T>(Window dialog)
        {
            var mainWindow = participant.GetMainWindow();
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

        public Task<IReadOnlyList<IStorageFile>> OpenFilePicker(FilePickerOpenOptions options)
        {
            var mainWindow = participant.GetMainWindow();
            return mainWindow.StorageProvider.OpenFilePickerAsync(options);
        }

        public async Task<string?> OpenFilePickerForGameExecutable()
        {
            var files = await participant.OpenFilePicker(new FilePickerOpenOptions
            {
                SuggestedFileName = GameConstants.ExecutableName,
                FileTypeFilter = [
                    new FilePickerFileType(FilePickerExecutableTypeName)
                    {
                        Patterns = [FilePickerExecutableTypeExtension] 
                    }]
            });
            return files.Count > 0 ? files[0].Path.LocalPath : null;
        }
    }
}