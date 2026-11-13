using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using D2RInstanceManager.Bootstrap;
using D2RInstanceManager.CommandLine;
using D2RInstanceManager.Error;
using D2RInstanceManager.MainWindow;
using D2RInstanceManager.Unlock;
using Microsoft.Extensions.DependencyInjection;


namespace D2RInstanceManager
{
    public partial class App : Application
    {
        private bool _shutdownWasHandledInternally;

        private bool _isWaitingForConfirmation;
        
        private static ServiceProvider? _services;
        
        public static IServiceProvider Services => _services ?? throw new InvalidOperationException("Services not initialized");
        
        private Window? _mainWindow;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += OnShutdownRequested;
            desktop.Exit += (_, _) => _services?.Dispose();
            
            var initialWindow = new InitialWindowView();
            try
            {
                var cliOptions = CliParser.Parse(desktop.Args ?? []);
                
                await Dispatcher.UIThread.InvokeAsync(initialWindow.Show);
                await Task.Delay(1000);
                await SetProgressBarVisibilityAsync(true);
                
                _services = await AppBootstrapper.BootstrapAsync(cliOptions);
                _services.GetRequiredService<RuntimeBootstrapper>().Bootstrap();

                await SetProgressBarVisibilityAsync(false);
                
                var isUnlocked = await _services.GetRequiredService<UnlockCoordinator>().UnlockAsync(initialWindow);
                if (!isUnlocked)
                {
                    await Dispatcher.UIThread.InvokeAsync(() => desktop.Shutdown());
                    return;
                }
                
                await SetProgressBarVisibilityAsync(true);
                
                var mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
                await mainWindowViewModel.SetupAsync();
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _mainWindow = new MainWindowView
                    {
                        DataContext = mainWindowViewModel,
                    };
                    _mainWindow.Closing += OnMainWindowClosing;

                    _services.GetRequiredService<WindowRuntimeBootstrapper>().Bootstrap(_mainWindow);
                    
                    desktop.MainWindow = _mainWindow;
                    desktop.MainWindow.Show();
                    initialWindow.Close();
                });
            } catch (Exception e)
            {
                Serilog.Log.Fatal(e, "Application failed to start");
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SetProgressBarVisibility(false);
                    
                    var error = new ErrorDescriptor.StartupFailure(e);
                    ShowErrorWindow(error);
                    
                    Shutdown(1);
                });
            }

            return;

            void SetProgressBarVisibility(bool visible) => initialWindow.ProgressBar.Opacity = visible ? 1 : 0;
            
            DispatcherOperation SetProgressBarVisibilityAsync(bool visible) =>
                Dispatcher.UIThread.InvokeAsync(() => SetProgressBarVisibility(visible));
        }
        
        private void Shutdown(int exitCode = 0)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown(exitCode);
            else
                Environment.Exit(exitCode);
        }

        private void RegisterExceptionHandlers()
        {
            Dispatcher.UIThread.UnhandledException += OnUiThreadUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }
        
        private bool _isShowingErrorWindow;

        private void ShowErrorWindow(ErrorDescriptor errorDescriptor)
        {
            if (_isShowingErrorWindow)
                return;

            _isShowingErrorWindow = true;

            try
            {
                var errorWindowViewModel = new ErrorWindowViewModel(errorDescriptor);
                var errorWindow = new ErrorWindowView
                {
                    DataContext = errorWindowViewModel,
                };
                errorWindowViewModel.ExitAction = errorWindow.Close;

                var frame = new DispatcherFrame();
                errorWindow.Closed += (_, _) => frame.Continue = false;
                
                if (_mainWindow is not null)
                    errorWindow.ShowDialog(_mainWindow);
                else
                    errorWindow.Show();
                
                Dispatcher.UIThread.PushFrame(frame);
            }
            catch (Exception exception)
            {
                Serilog.Log.Fatal(exception,"Failed to show error window");
            }
            finally
            {
                _isShowingErrorWindow = false;
            }
        }

        private void OnUiThreadUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Serilog.Log.Fatal(e.Exception, "Unhandled UI thread exception");
            
            e.Handled = true;

            var error = new ErrorDescriptor.RuntimeFailure(e.Exception);
            ShowErrorWindow(error);
            
            Shutdown(1);
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Serilog.Log.Fatal(e.ExceptionObject as Exception, "Unhandled app domain exception");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Serilog.Log.Fatal(e.Exception, "Unhandled task exception");
            e.SetObserved();
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

                    Shutdown();
                }
                finally
                {
                    _isWaitingForConfirmation = false;
                }
            }
            catch (Exception exception)
            {
                Serilog.Log.Fatal(exception, "Application failed to exit");
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var error = new ErrorDescriptor.ShutdownFailure(exception);
                    ShowErrorWindow(error);
                
                    _shutdownWasHandledInternally = true;
                    
                    Shutdown(1);
                });
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            RegisterExceptionHandlers();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _ = InitializeAsync(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }


    }
}
