namespace Zenject.Tests.Installers.ScriptableObjectInstallers
{
    public class Foo
    {
    }

    public class FooInstaller : ScriptableObjectInstaller<FooInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();
        }
    }
}
