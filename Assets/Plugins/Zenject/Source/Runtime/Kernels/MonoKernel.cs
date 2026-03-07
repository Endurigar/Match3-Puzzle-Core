#if !NOT_UNITY3D

#pragma warning disable 649

using ModestTree;
using UnityEngine;
using UnityEngine.Analytics;

namespace Zenject
{
    public abstract class MonoKernel : MonoBehaviour
    {
        [InjectLocal]
        TickableManager _tickableManager = null;

        [InjectLocal]
        InitializableManager _initializableManager = null;

        [InjectLocal]
        DisposableManager _disposablesManager = null;

        [InjectOptional] 
        private IDecoratableMonoKernel decoratableMonoKernel;

        bool _hasInitialized;
        bool _isDestroyed;

        protected bool IsDestroyed
        {
            get { return _isDestroyed; }
        }

        public virtual void Start()
        {
            if (decoratableMonoKernel?.ShouldInitializeOnStart()??true)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (!_hasInitialized)
            {
                _hasInitialized = true;

                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.Initialize();
                }
                else
                {
                    _initializableManager.Initialize();
                }
            }
        }

        public virtual void Update()
        {
            if (_tickableManager != null)
            {
                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.Update();
                }
                else
                {
                    _tickableManager.Update();
                }
            }
        }

        public virtual void FixedUpdate()
        {
            if (_tickableManager != null)
            {
                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.FixedUpdate();
                }
                else
                {
                    _tickableManager.FixedUpdate();
                }
            }
        }

        public virtual void LateUpdate()
        {
            if (_tickableManager != null)
            {
                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.LateUpdate();
                }
                else
                {
                    _tickableManager.LateUpdate();
                }
            }
        }

        public virtual void OnDestroy()
        {
            if (_disposablesManager != null)
            {
                Assert.That(!_isDestroyed);
                _isDestroyed = true;

                if (decoratableMonoKernel != null)
                {
                    decoratableMonoKernel.Dispose();
                    decoratableMonoKernel.LateDispose();
                }
                else
                {
                    _disposablesManager.Dispose();
                    _disposablesManager.LateDispose();
                }
            }
        }
    }
}

#endif
