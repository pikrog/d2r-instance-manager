namespace AvaloniaApplication1.Bootstrap;

public class AppRuntimeBootstrapper(InstanceManagerBootstrapper instanceManagerBootstrapper)
{
    public void Bootstrap()
    {
        instanceManagerBootstrapper.Bootstrap();
    }
}
