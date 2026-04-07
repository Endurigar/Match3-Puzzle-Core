using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Systems;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.Core
{
    [CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "Installers/ConfigsInstaller")]
    public class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
    {
        [Header("Configurations")]
        [SerializeField] private GemsScriptableObject _gemsScriptableObject;
        [SerializeField] private AudioLibrary _audioLibrary;

        /// <summary>
        /// Installs the configuration instances into the Zenject container.
        /// </summary>
        public override void InstallBindings()
        {
            Container.BindInstance(_gemsScriptableObject).AsSingle();
            Container.BindInstance(_audioLibrary).AsSingle();
        }
    }
}