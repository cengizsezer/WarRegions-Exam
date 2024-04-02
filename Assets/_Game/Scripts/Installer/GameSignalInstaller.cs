using Zenject;
using MyProject.Core.CustomAttribute;

namespace MyProject.Core.Installer
{
    public class GameSignalInstaller : Installer<GameSignalInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            foreach (var signal in CustomAttributeProcessor.GetSignalClasses())
            {
                Container.DeclareSignal(signal).OptionalSubscriber();
            }

        }

    }
}

