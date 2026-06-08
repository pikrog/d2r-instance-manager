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
            InitialWindow? initialWindow = null;
            try
            {
                initialWindow = new InitialWindow();
                initialWindow.Show();
                
                Services = await AppBootstrapper.BootstrapAsync();
                await Task.Delay(1000);
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
                initialWindow?.Close();
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