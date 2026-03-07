#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

#pragma warning disable 649

namespace Zenject
{
    public class GameObjectContext : RunnableContext
    {
        public event Action PreInstall;
        public event Action PostInstall;
        public event Action PreResolve;
        public event Action PostResolve;

        [SerializeField]
        [Tooltip("Note that this field is optional and can be ignored in most cases.  This is really only needed if you want to control the 'Script Execution Order' of your subcontainer.  In this case, define a new class that derives from MonoKernel, add it to this game object, then drag it into this field.  Then you can set a value for 'Script Execution Order' for this new class and this will control when all ITickable/IInitializable classes bound within this subcontainer get called.")]
        [FormerlySerializedAs("_facade")]
        MonoKernel _kernel;

        DiContainer _container;

        DiContainer _parentContainer;

        bool _hasInstalled;

        public override DiContainer Container
        {
            get { return _container; }
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return new[] { gameObject };
        }

        [Inject]
        public void Construct(
            DiContainer parentContainer)
        {
            Assert.IsNull(_parentContainer);
            _parentContainer = parentContainer;

            Initialize();
        }

        protected override void RunInternal()
        {
            Install(_parentContainer);
            ResolveAndStart();
        }

        public void Install(DiContainer parentContainer) 
        {
            Assert.That(_parentContainer == null || _parentContainer == parentContainer);

            if (_hasInstalled) 
            {
                return;
            }

            _hasInstalled = true;

            Assert.IsNull(_container);
            _container = parentContainer.CreateSubContainer();

            if (PreInstall != null)
            {
                PreInstall();
            }

            var injectableMonoBehaviours = new List<MonoBehaviour>();

            GetInjectableMonoBehaviours(injectableMonoBehaviours);

            foreach (var instance in injectableMonoBehaviours)
            {
                if (instance is MonoKernel)
                {
                    Assert.That(ReferenceEquals(instance, _kernel),
                        "Found MonoKernel derived class that is not hooked up to GameObjectContext.  If you use MonoKernel, you must indicate this to GameObjectContext by dragging and dropping it to the Kernel field in the inspector");
                }

                _container.QueueForInject(instance);
            }

            _container.IsInstalling = true;

            try
            {
                InstallBindings(injectableMonoBehaviours);
            }
            finally
            {
                _container.IsInstalling = false;
            }

            if (PostInstall != null)
            {
                PostInstall();
            }
        }

        void ResolveAndStart() 
        {
            if (PreResolve != null)
            {
                PreResolve();
            }

            _container.ResolveRoots();

            if (PostResolve != null)
            {
                PostResolve();
            }

            if (gameObject.scene.isLoaded && !_container.IsValidating)
            {
                _kernel = _container.Resolve<MonoKernel>();
                _kernel.Initialize();
            }
        }

        protected override void GetInjectableMonoBehaviours(List<MonoBehaviour> monoBehaviours)
        {
            ZenUtilInternal.AddStateMachineBehaviourAutoInjectersUnderGameObject(gameObject);

            foreach (var monoBehaviour in GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null)
                {
                    continue;
                }

                if (!ZenUtilInternal.IsInjectableMonoBehaviourType(monoBehaviour.GetType()))
                {
                    continue;
                }

                if (monoBehaviour == this)
                {
                    continue;
                }

                monoBehaviours.Add(monoBehaviour);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child != null)
                {
                    ZenUtilInternal.GetInjectableMonoBehavioursUnderGameObject(
                        child.gameObject, monoBehaviours);
                }
            }
        }

        void InstallBindings(List<MonoBehaviour> injectableMonoBehaviours)
        {
            _container.DefaultParent = transform;

            _container.Bind<Context>().FromInstance(this);
            _container.Bind<GameObjectContext>().FromInstance(this);

            if (_kernel == null)
            {
                _container.Bind<MonoKernel>()
                    .To<DefaultGameObjectKernel>().FromNewComponentOn(gameObject).AsSingle().NonLazy();
            }
            else
            {
                _container.Bind<MonoKernel>().FromInstance(_kernel).AsSingle().NonLazy();
            }

            InstallSceneBindings(injectableMonoBehaviours);
            InstallInstallers();
        }
    }
}

#endif
