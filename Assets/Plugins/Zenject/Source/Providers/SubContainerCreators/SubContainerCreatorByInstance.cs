using System.Collections.Generic;
using System;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByInstance : ISubContainerCreator
    {
        readonly DiContainer _subcontainer;

        public SubContainerCreatorByInstance(DiContainer subcontainer)
        {
            _subcontainer = subcontainer;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context, out Action injectAction)
        {
            Assert.That(args.IsEmpty());

            injectAction = null;

            return _subcontainer;
        }
    }
}

