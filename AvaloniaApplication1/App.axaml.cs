using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaApplication1.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using MainWindowView = AvaloniaApplication1.MainWindow.MainWindowView;
using MainWindowViewModel = AvaloniaApplication1.MainWindow.MainWindowViewModel;

namespace AvaloniaApplication1
{
    public partial class App : Application
    {
        private bool _shutdownWasHandledInternally;

        private bool _isWaitingForConfirmation;
        
        public static IServiceProvider? Services { get; private set; }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            // todo: cleanup. error window. initial window = progress bar
            desktop.ShutdownRequested += OnShutdownRequested;
            var initialWindow = new InitialWindowView();
            try
            {
                await Dispatcher.UIThread.InvokeAsync(initialWindow.Show);
                await Task.Delay(1000);
                await Dispatcher.UIThread.InvokeAsync(() => initialWindow.ProgressBar.Opacity = 1);
                
                Services = await AppBootstrapper.BootstrapAsync();
                Services.GetRequiredService<AppRuntimeBootstrapper>().Bootstrap();
                
                var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
                await mainWindowViewModel.SetupAsync();
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var mainWindow = new MainWindowView
                    {
                        DataContext = mainWindowViewModel,
                    };
                    mainWindow.Closing += OnMainWindowClosing;
                    desktop.MainWindow = mainWindow;
                    desktop.MainWindow.Show();
                    initialWindow.Close();
                });
            } catch (Exception e)
            {
                Console.Error.WriteLine(e);
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    initialWindow.Close();
                    desktop.Shutdown(1);
                });
            }
        }

        private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            if (_shutdownWasHandledInternally)
                return;

            if (sender is not IClassicDesktopStyleApplicationLifetime { MainWindow: {  } mainWindow })
                return;

            e.Cancel = true;
            mainWindow.Close();
        }

        private async void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
        {
            try
            {
                if (_shutdownWasHandledInternally)
                    return;
                
                if (sender is not MainWindowView mainWindow) 
                    return;
                
                if (mainWindow.DataContext is not MainWindowViewModel viewModel)
                    return;
                
                if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                    return;

                e.Cancel = true;

                if (_isWaitingForConfirmation)
                    return;

                _isWaitingForConfirmation = true;

                try
                {
                    if (!await viewModel.TryExitAsync())
                        return;
                    
                    _shutdownWasHandledInternally = true;
                    
                    mainWindow.Closing -= OnMainWindowClosing;

                    desktop.Shutdown();
                }
                finally
                {
                    _isWaitingForConfirmation = false;
                }
            }
            catch (Exception exception)
            {
                // todo: log and show error window
                Console.WriteLine(exception);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _ = InitializeAsync(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }


    }
}