namespace Zenject
{
    public interface IInstaller
    {
        void InstallBindings();

        bool IsEnabled
        {
            get;
        }
    }

}
