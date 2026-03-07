using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByInstanceGetter : ISubContainerCreator
    {
        readonly Func<InjectContext, DiContainer> _subcontainerGetter;

        public SubContainerCreatorByInstanceGetter(
            Func<InjectContext, DiContainer> subcontainerGetter)
        {
            _subcontainerGetter = subcontainerGetter;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context, out Action injectAction)
        {
            Assert.That(args.IsEmpty());

            injectAction = null;

            return _subcontainerGetter(context);
        }
    }
}

