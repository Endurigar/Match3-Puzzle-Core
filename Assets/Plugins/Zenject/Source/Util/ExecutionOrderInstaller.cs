using System;
using System.Collections.Generic;

namespace Zenject
{
    public class ExecutionOrderInstaller : Installer<List<Type>, ExecutionOrderInstaller>
    {
        List<Type> _typeOrder;

        public ExecutionOrderInstaller(List<Type> typeOrder)
        {
            _typeOrder = typeOrder;
        }

        public override void InstallBindings()
        {
            int order = -1 * _typeOrder.Count;

            foreach (var type in _typeOrder)
            {
                Container.BindExecutionOrder(type, order);
                order++;
            }
        }
    }
}

