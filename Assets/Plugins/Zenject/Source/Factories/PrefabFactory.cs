#if !NOT_UNITY3D

using ModestTree;
using UnityEngine;

namespace Zenject
{

    public class PrefabFactory<T> : IFactory<UnityEngine.Object, T>
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return _container.InstantiatePrefabForComponent<T>(prefab);
        }

    }

    public class PrefabFactory<P1, T> : IFactory<UnityEngine.Object, P1, T>
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param));
        }
    }

    public class PrefabFactory<P1, P2, T> : IFactory<UnityEngine.Object, P1, P2, T>
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2));
        }
    }

    public class PrefabFactory<P1, P2, P3, T> : IFactory<UnityEngine.Object, P1, P2, P3, T>
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2, P3 param3)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2, param3));
        }
    }

    public class PrefabFactory<P1, P2, P3, P4, T> : IFactory<UnityEngine.Object, P1, P2, P3, P4, T>
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2, P3 param3, P4 param4)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2, param3, param4));
        }
    }
}

#endif


