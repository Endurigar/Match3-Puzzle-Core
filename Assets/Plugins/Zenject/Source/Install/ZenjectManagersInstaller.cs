
namespace Zenject
{
    public class ZenjectManagersInstaller : Installer<ZenjectManagersInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(TickableManager), typeof(InitializableManager), typeof(DisposableManager))
                .ToSelf().AsSingle().CopyIntoAllSubContainers();
        }
    }
}

