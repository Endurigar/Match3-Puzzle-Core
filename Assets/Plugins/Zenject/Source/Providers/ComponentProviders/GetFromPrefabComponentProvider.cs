#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class GetFromPrefabComponentProvider : IProvider
    {
        readonly IPrefabInstantiator _prefabInstantiator;
        readonly Type _componentType;
        readonly bool _matchSingle;

        public GetFromPrefabComponentProvider(
            Type componentType,
            IPrefabInstantiator prefabInstantiator, bool matchSingle)
        {
            _prefabInstantiator = prefabInstantiator;
            _componentType = componentType;
            _matchSingle = matchSingle;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _componentType;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            var gameObject = _prefabInstantiator.Instantiate(context, args, out injectAction);


            if (_matchSingle)
            {
                var match = gameObject.GetComponentInChildren(_componentType, true);

                Assert.IsNotNull(match, "Could not find component with type '{0}' on prefab '{1}'",
                _componentType, _prefabInstantiator.GetPrefab(context).name);

                buffer.Add(match);
                return;
            }

            var allComponents = gameObject.GetComponentsInChildren(_componentType, true);

            Assert.That(allComponents.Length >= 1,
                "Expected to find at least one component with type '{0}' on prefab '{1}'",
                _componentType, _prefabInstantiator.GetPrefab(context).name);

            buffer.AllocFreeAddRange(allComponents);
        }
    }
}

#endif
