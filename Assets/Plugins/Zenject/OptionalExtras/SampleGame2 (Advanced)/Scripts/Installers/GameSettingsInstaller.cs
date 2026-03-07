using System;

namespace Zenject.SpaceFighter
{
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public EnemySpawner.Settings EnemySpawner;
        public GameRestartHandler.Settings GameRestartHandler;
        public GameInstaller.Settings GameInstaller;
        public PlayerSettings Player;
        public EnemySettings Enemy;

        [Serializable]
        public class PlayerSettings
        {
            public PlayerMoveHandler.Settings PlayerMoveHandler;
            public PlayerShootHandler.Settings PlayerShootHandler;
            public PlayerDamageHandler.Settings PlayerCollisionHandler;
            public PlayerHealthWatcher.Settings PlayerHealthWatcher;
        }

        [Serializable]
        public class EnemySettings
        {
            public EnemyTunables DefaultSettings;
            public EnemyStateIdle.Settings EnemyStateIdle;
            public EnemyRotationHandler.Settings EnemyRotationHandler;
            public EnemyStateFollow.Settings EnemyStateFollow;
            public EnemyStateAttack.Settings EnemyStateAttack;
            public EnemyDeathHandler.Settings EnemyHealthWatcher;
            public EnemyCommonSettings EnemyCommonSettings;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(EnemySpawner).IfNotBound();
            Container.BindInstance(GameRestartHandler).IfNotBound();
            Container.BindInstance(GameInstaller).IfNotBound();

            Container.BindInstance(Player.PlayerMoveHandler).IfNotBound();
            Container.BindInstance(Player.PlayerShootHandler).IfNotBound();
            Container.BindInstance(Player.PlayerCollisionHandler).IfNotBound();
            Container.BindInstance(Player.PlayerHealthWatcher).IfNotBound();

            Container.BindInstance(Enemy.EnemyStateIdle).IfNotBound();
            Container.BindInstance(Enemy.EnemyRotationHandler).IfNotBound();
            Container.BindInstance(Enemy.EnemyStateFollow).IfNotBound();
            Container.BindInstance(Enemy.EnemyStateAttack).IfNotBound();
            Container.BindInstance(Enemy.EnemyHealthWatcher).IfNotBound();
            Container.BindInstance(Enemy.EnemyCommonSettings).IfNotBound();
        }
    }
}

