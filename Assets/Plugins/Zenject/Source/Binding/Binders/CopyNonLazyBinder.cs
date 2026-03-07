using System.Collections.Generic;

namespace Zenject
{
    [NoReflectionBaking]
    public class CopyNonLazyBinder : NonLazyBinder
    {
        List<BindInfo> _secondaryBindInfos;

        public CopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        internal void AddSecondaryCopyBindInfo(BindInfo bindInfo)
        {
            if (_secondaryBindInfos == null)
            {
                _secondaryBindInfos = new List<BindInfo>();
            }
            _secondaryBindInfos.Add(bindInfo);
        }

        public NonLazyBinder CopyIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyIntoAll);
            return this;
        }

        public NonLazyBinder CopyIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyDirectOnly);
            return this;
        }

        public NonLazyBinder MoveIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveIntoAll);
            return this;
        }

        public NonLazyBinder MoveIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveDirectOnly);
            return this;
        }

        void SetInheritanceMethod(BindingInheritanceMethods method)
        {
            BindInfo.BindingInheritanceMethod = method;

            if (_secondaryBindInfos != null)
            {
                foreach (var secondaryBindInfo in _secondaryBindInfos)
                {
                    secondaryBindInfo.BindingInheritanceMethod = method;
                }
            }
        }
    }
}
