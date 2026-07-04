using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaApplication1.Bootstrap;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Microsoft.Extensions.DependencyInjection;
using MainWindowViewModel = AvaloniaApplication1.ViewModels.MainWindowViewModel;

namespace AvaloniaApplication1
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            // todo: cleanup. error window. initial window = progress bar
            var initialWindow = new InitialWindow();
            try
            {
                await Dispatcher.UIThread.InvokeAsync(initialWindow.Show);
                await Task.Delay(1000);
                await Dispatcher.UIThread.InvokeAsync(() => initialWindow.ProgressBar.Opacity = 1);
                
                Services = await AppBootstrapper.BootstrapAsync();
                //await Task.Delay(1000);
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = Services.GetRequiredService<MainWindowViewModel>(),
                    };
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