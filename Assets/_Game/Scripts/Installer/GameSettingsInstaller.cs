using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts;

namespace MyProject.Core.Installer
{
    [CreateAssetMenu(fileName = nameof(GlobalSettingsInstaller), menuName = AssetMenuName.INSTALLERS + nameof(GlobalSettingsInstaller))]
    public class GlobalSettingsInstaller : ScriptableObjectInstaller<GlobalSettingsInstaller>
    {
        [SerializeField]
        private ScriptableObject[] settings;

        public override void InstallBindings()
        {
            foreach (ScriptableObject setting in settings)
            {
                Container.BindInterfacesAndSelfTo(setting.GetType()).FromInstance(setting);
            }
        }
    }

}

