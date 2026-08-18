using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Navigation;

public class NavigationService(IPageController controller, IPageProvider provider)
{
    public async Task NavigateAsync<T>() where T : PageViewModel => await NavigateAsync(typeof(T));

    public async Task NavigateAsync(Type pageType)
    {
        var page = provider.Get(pageType);
        
        if (controller.CurrentPage == page)
            return;
        if (controller.CurrentPage is not null)
        {
            if(!await controller.CurrentPage.CanLeaveAsync())
                return;
            await controller.CurrentPage.OnLeaveAsync();
        }
        controller.CurrentPage = page;
        await page.OnEnterAsync();
    }
}