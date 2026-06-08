using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels;

public abstract class FormViewModelBase : ObservableValidator, INotifyDataErrorInfo, IFormViewModel
{
    private bool _showValidationErrors;

    public bool Validate()
    {
        _showValidationErrors = true;
        ValidateAllProperties();
        return !HasErrors;
    }

    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
    {
        return _showValidationErrors ? base.GetErrors(propertyName) : new List<ValidationResult>();
    }
}
