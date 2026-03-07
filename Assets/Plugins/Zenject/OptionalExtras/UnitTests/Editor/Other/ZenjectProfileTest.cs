using NUnit.Framework;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class ZenjectProfileTest
    {
        class Test0
        {
            public void DoStuff()
            {
            }

            public void DoStuff1()
            {
            }
        }

        class Test1
        {
            [Inject]
            public Test0 TestB
            {
                set;
                get;
            }

            [Inject]
            public Test0 _testC = null;

            public Test1(Test0 test1, Test0 test2, Test0 test3, Test0 test4)
            {
            }

            public void DoStuff()
            {
            }

            public void DoStuff1()
            {
            }
        }

        class Test2
        {
            [Inject]
            public Test1 TestB
            {
                set;
                get;
            }

            [Inject]
            public Test1 _testC = null;

            public Test2(Test1 test1, Test1 test2, Test1 test3, Test1 test4)
            {
            }
        }

        [Test]
        public void Test()
        {






        }
    }
}


