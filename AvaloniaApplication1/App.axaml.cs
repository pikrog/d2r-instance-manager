using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaApplication1.Bootstrap;
using AvaloniaApplication1.MainWindow;
using Microsoft.Extensions.DependencyInjection;


namespace AvaloniaApplication1
{
    public partial class App : Application
    {
        private bool _shutdownWasHandledInternally;

        private bool _isWaitingForConfirmation;
        
        private static ServiceProvider? _services;
        
        public static IServiceProvider Services => _services ?? throw new InvalidOperationException("Services not initialized");
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            // todo: error window
            
            desktop.ShutdownRequested += OnShutdownRequested;
            desktop.Exit += (_, _) => _services?.Dispose();
            
            var initialWindow = new InitialWindowView();
            try
            {
                await Dispatcher.UIThread.InvokeAsync(initialWindow.Show);
                await Task.Delay(1000);
                await Dispatcher.UIThread.InvokeAsync(() => initialWindow.ProgressBar.Opacity = 1);
                
                _services = await AppBootstrapper.BootstrapAsync();
                _services.GetRequiredService<AppRuntimeBootstrapper>().Bootstrap();
                
                var mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
                await mainWindowViewModel.SetupAsync();
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var mainWindow = new MainWindowView
                    {
                        DataContext = mainWindowViewModel,
                    };
                    mainWindow.Closing += OnMainWindowClosing;

                    _services.GetRequiredService<MainWindowRuntimeBootstrapper>().Bootstrap(mainWindow);
                    
                    desktop.MainWindow = mainWindow;
                    desktop.MainWindow.Show();
                    initialWindow.Close();
                });
            } catch (Exception e)
            {
                Serilog.Log.Fatal(e, "Application failed to start");
                
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
                Serilog.Log.Fatal(exception, "Application failed to exit");
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
