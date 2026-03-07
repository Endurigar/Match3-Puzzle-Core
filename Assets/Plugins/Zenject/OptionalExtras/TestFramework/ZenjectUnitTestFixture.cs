using NUnit.Framework;

namespace Zenject
{
    public abstract class ZenjectUnitTestFixture
    {
        DiContainer _container;

        protected DiContainer Container
        {
            get { return _container; }
        }

        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer(StaticContext.Container);
        }

        [TearDown]
        public virtual void Teardown()
        {
            StaticContext.Clear();
        }
    }
}
