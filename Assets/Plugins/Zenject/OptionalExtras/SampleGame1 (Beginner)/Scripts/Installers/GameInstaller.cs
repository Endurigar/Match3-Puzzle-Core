using System;
using UnityEngine;

namespace Zenject.Asteroids
{
    public class GameInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings = null;

        public override void InstallBindings()
        {

            InstallAsteroids();
            InstallShip();
            InstallMisc();
            InstallSignals();
            InstallExecutionOrder();
        }

        void InstallAsteroids()
        {

            Container.BindInterfacesAndSelfTo<AsteroidManager>().AsSingle();


            Container.BindFactory<Asteroid, Asteroid.Factory>()
                .FromComponentInNewPrefab(_settings.AsteroidPrefab)
                .WithGameObjectName("Asteroid")
                .UnderTransformGroup("Asteroids");
        }

        void InstallMisc()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.Bind<LevelHelper>().AsSingle();

            Container.BindInterfacesTo<AudioHandler>().AsSingle();

            Container.BindFactory<Transform, ExplosionFactory>()
                .FromComponentInNewPrefab(_settings.ExplosionPrefab);

            Container.BindFactory<Transform, BrokenShipFactory>()
                .FromComponentInNewPrefab(_settings.BrokenShipPrefab);
        }

        void InstallSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<ShipCrashedSignal>();
        }

        void InstallShip()
        {
            Container.Bind<ShipStateFactory>().AsSingle();


            Container.BindFactory<ShipStateWaitingToStart, ShipStateWaitingToStart.Factory>().WhenInjectedInto<ShipStateFactory>();
            Container.BindFactory<ShipStateDead, ShipStateDead.Factory>().WhenInjectedInto<ShipStateFactory>();
            Container.BindFactory<ShipStateMoving, ShipStateMoving.Factory>().WhenInjectedInto<ShipStateFactory>();
        }

        void InstallExecutionOrder()
        {
            Container.BindExecutionOrder<AsteroidManager>(-20);
            Container.BindExecutionOrder<GameController>(-10);

        }

        [Serializable]
        public class Settings
        {
            public GameObject ExplosionPrefab;
            public GameObject BrokenShipPrefab;
            public GameObject AsteroidPrefab;
            public GameObject ShipPrefab;
        }
    }
}

