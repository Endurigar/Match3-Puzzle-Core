using System.Collections.Generic;

namespace Zenject
{
    [NoReflectionBaking]
    public class SignalCopyBinder
    {
        readonly List<BindInfo> _bindInfos;

        public SignalCopyBinder()
        {
            _bindInfos = new List<BindInfo>();
        }

        public SignalCopyBinder(BindInfo bindInfo)
        {
            _bindInfos = new List<BindInfo>
            {
                bindInfo
            };
        }

        public void AddCopyBindInfo(BindInfo bindInfo)
        {
            _bindInfos.Add(bindInfo);
        }

        public void CopyIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyIntoAll);
        }

        public void CopyIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyDirectOnly);
        }

        public void MoveIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveIntoAll);
        }

        public void MoveIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveDirectOnly);
        }

        void SetInheritanceMethod(BindingInheritanceMethods method)
        {
            for (int i = 0; i < _bindInfos.Count; i++)
            {
                _bindInfos[i].BindingInheritanceMethod = method;
            }
        }
    }
}
