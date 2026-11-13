using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using D2RInstanceManager.Common;

namespace D2RInstanceManager.Error;

public partial class ErrorWindowViewModel(ErrorDescriptor errorDescriptor) : ViewModelBase
{
    [ObservableProperty] 
    public partial ErrorDescriptor ErrorDescriptor { get; private set; } = errorDescriptor;
    
    public Action? ExitAction { get; set; }
    
    [RelayCommand]
    private void Exit() => ExitAction?.Invoke();
}