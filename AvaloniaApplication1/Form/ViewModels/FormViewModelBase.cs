using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Form.ViewModels;

public abstract class FormViewModelBase : ObservableValidator, INotifyDataErrorInfo, IFormViewModel
{
    protected FormViewModelBase() { }
    
    protected FormViewModelBase(IServiceProvider serviceProvider) : base(serviceProvider, null) { }
    
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
