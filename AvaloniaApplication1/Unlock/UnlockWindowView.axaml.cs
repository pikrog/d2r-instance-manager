using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace D2RInstanceManager.Unlock;

public partial class UnlockWindowView : Window
{
    private UnlockWindowViewModel? _viewModel;

    private CancellationTokenSource? _animationCts;

    public UnlockWindowView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (_viewModel is not null)
            _viewModel.UnlockFailed -= OnUnlockFailed;

        _viewModel = DataContext as UnlockWindowViewModel;

        if (_viewModel is not null)
            _viewModel.UnlockFailed += OnUnlockFailed;
    }

    private async Task RunShakeAnimationAsync(CancellationToken token)
    {
        var animation = (Animation)Resources["ShakeAnimation"]!;
        try
        {
            await animation.RunAsync(PassphraseTextBox, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
    }

    private void OnUnlockFailed(object? sender, EventArgs e)
    {
        PassphraseTextBox.SelectAll();

        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = new CancellationTokenSource();

        _ = RunShakeAnimationAsync(_animationCts.Token);
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.UnlockFailed -= OnUnlockFailed;
            _viewModel.Close();
            _viewModel = null;
        }

        base.OnClosed(e);
    }

    private void PassphraseTextBox_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        (sender as TextBox)?.Focus();
    }
}