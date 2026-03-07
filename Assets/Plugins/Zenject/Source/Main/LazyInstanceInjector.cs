
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class LazyInstanceInjector
    {
        readonly DiContainer _container;
        readonly HashSet<object> _instancesToInject = new HashSet<object>();

        public LazyInstanceInjector(DiContainer container)
        {
            _container = container;
        }

        public IEnumerable<object> Instances
        {
            get { return _instancesToInject; }
        }

        public void AddInstance(object instance)
        {
            _instancesToInject.Add(instance);
        }

        public void AddInstances(IEnumerable<object> instances)
        {
            _instancesToInject.UnionWith(instances);
        }

        public void LazyInject(object instance)
        {
            if (_instancesToInject.Remove(instance))
            {
                _container.Inject(instance);
            }
        }

        public void LazyInjectAll()
        {
#if UNITY_EDITOR
            using (ProfileBlock.Start("Zenject.LazyInstanceInjector.LazyInjectAll"))
#endif
            {
                var tempList = new List<object>();

                while (!_instancesToInject.IsEmpty())
                {
                    tempList.Clear();
                    tempList.AddRange(_instancesToInject);

                    foreach (var instance in tempList)
                    {
                        LazyInject(instance);
                    }
                }
            }
        }
    }
}

