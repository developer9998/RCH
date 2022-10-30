using ComputerInterface.Interfaces;
using Zenject;

namespace RCH.CI
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IComputerModEntry>().To<RchEntry>().AsSingle();
        }
    }
}
