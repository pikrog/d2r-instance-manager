namespace AvaloniaApplication1.Bootstrap;

public class AppRuntimeBootstrapper(InstanceRuntimeBootstrapper instanceRuntimeBootstrapper)
{
    public void Bootstrap()
    {
        instanceRuntimeBootstrapper.Bootstrap();
    }
}
