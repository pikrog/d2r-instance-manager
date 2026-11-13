using System;
using D2RInstanceManager.Error;

namespace D2RInstanceManager.Design.ViewModels;

public class ErrorWindowDesignViewModel() : 
    ErrorWindowViewModel(new ErrorDescriptor.StartupFailure(
        new UnauthorizedAccessException(@"Access to the path 'C:\SomeFolder\config.json' is denied.")))
{
    
}