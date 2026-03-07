using System.Linq;
using ModestTree;
using UnityEngine;

#pragma warning disable 219

namespace Zenject
{
    public class CheatSheet : Installer<CheatSheet>
    {
        public override void InstallBindings()
        {
            Container.Bind<Foo>().AsTransient();

            Container.Bind<IFoo>().To<Foo>().AsTransient();

            Container.Bind(typeof(IFoo)).To(typeof(Foo)).AsTransient();


            Container.Bind<Foo>().AsSingle();

            Container.Bind<IFoo>().To<Foo>().AsSingle();

            Container.Bind(typeof(Foo), typeof(IFoo), typeof(IFoo2)).To<Foo>().AsSingle();


            Container.BindInterfacesAndSelfTo<Foo>().AsSingle();

            Container.BindInterfacesTo<Foo>().AsSingle();


            Container.Bind<Foo>().FromInstance(new Foo());

            Container.BindInstance(new Foo());

            Container.BindInstances(new Foo(), new Bar());


            Container.Bind<int>().FromInstance(10);
            Container.Bind<bool>().FromInstance(false);

            Container.BindInstance(10);
            Container.BindInstance(false);

            Container.BindInstance(10).WhenInjectedInto<Foo>();


            Container.Bind<Foo>().FromMethod(GetFoo);

            Container.Bind<IFoo>().FromMethod(GetRandomFoo);

            Container.Bind<Foo>().FromMethod(ctx => new Foo());

            Container.Bind<Foo>().FromMethod(ctx => ctx.Container.Instantiate<Foo>());

            InstallMore();
        }

        Foo GetFoo(InjectContext ctx)
        {
            return new Foo();
        }

        IFoo GetRandomFoo(InjectContext ctx)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                {
                    return ctx.Container.Instantiate<Foo1>();
                }
                case 1:
                {
                    return ctx.Container.Instantiate<Foo2>();
                }
            }

            return ctx.Container.Instantiate<Foo3>();
        }

        void InstallMore()
        {

            Container.Bind<Foo>().AsSingle();

            Container.Bind<Bar>().FromResolveGetter<Foo>(foo => foo.GetBar());

            Container.Bind<string>().FromResolveGetter<Foo>(foo => foo.GetTitle());


            Container.Bind<Foo>().FromNewComponentOnNewGameObject().AsSingle();

            Container.Bind<Foo>().FromNewComponentOnNewGameObject().WithGameObjectName("Foo1").AsSingle();

            Container.Bind<IFoo>().To<Foo>().FromNewComponentOnNewGameObject().AsSingle();


            GameObject prefab = null;
            Container.Bind<Foo>().FromComponentInNewPrefab(prefab).AsSingle();

            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(prefab).AsSingle();

            Container.Bind(typeof(Foo), typeof(Bar)).FromComponentInNewPrefab(prefab).AsSingle();


            Container.Bind<Foo>().FromComponentInNewPrefab(prefab).AsTransient();

            Container.Bind<IFoo>().To<Foo>().FromComponentInNewPrefab(prefab);


            Container.Bind<string>().WithId("PlayerName").FromInstance("name of the player");

            Container.BindInstance("name of the player").WithId("PlayerName");

            Container.BindInstance("foo").WithId("FooA");
            Container.BindInstance("asdf").WithId("FooB");

            InstallMore2();
        }

        public class Norf
        {
            [Inject(Id = "FooA")]
            public string Foo;
        }

        public class Qux
        {
            [Inject(Id = "FooB")]
            public string Foo;
        }

        public void InstallMore2()
        {

            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().WithId("FooA").AsCached();
            Container.Bind<Foo>().WithId("FooA").AsCached();

            InstallMore3();
        }

        public class Norf2
        {
            [Inject]
            public Foo Foo;
        }

        public class Qux2
        {
            [Inject]
            public Foo Foo;

            [Inject(Id = "FooA")]
            public Foo Foo2;
        }

        public void InstallMore3()
        {

            Container.Bind<Foo>().AsSingle().WhenInjectedInto<Bar>();

            Container.Bind<IFoo>().To<Foo1>().AsSingle().WhenInjectedInto<Bar>();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            Container.Bind<IFoo>().To<Foo1>().AsSingle();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Qux>();

            Container.Bind<Foo>().AsSingle().WhenInjectedInto(typeof(Bar), typeof(Qux), typeof(Baz));

            Container.BindInstance("my game").WithId("Title").WhenInjectedInto<Gui>();

            Container.BindInstance(5).WhenInjectedInto<Gui>();

            Container.BindInstance(5.0f).When(ctx =>
                ctx.ObjectType == typeof(Gui) && ctx.MemberName == "width");

            Container.Bind<IFoo>().To<Foo>().AsTransient().When(
                ctx => ctx.AllObjectTypes.Contains(typeof(Bar)));


            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Bar>().WithId("Bar1").AsCached();
            Container.Bind<Bar>().WithId("Bar2").AsCached();

            Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar1")).Any());
            Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar2")).Any());

            Assert.That(Container.ResolveId<Bar>("Bar1").Foo == foo1);
            Assert.That(Container.ResolveId<Bar>("Bar2").Foo == foo2);


            GameObject fooPrefab = null;
            Container.Bind<Foo>().FromComponentInNewPrefab(fooPrefab).AsSingle();
            Container.Bind<IBar>().To<Foo>().FromResolve();
            Container.Bind<IFoo>().To<IBar>().FromResolve();

            Container.Bind(typeof(Foo), typeof(IBar), typeof(IFoo)).To<Foo>().FromComponentInNewPrefab(fooPrefab).AsSingle();

            InstallMore4();
        }

        public class FooInstaller : Installer<FooInstaller>
        {
            public FooInstaller(string foo)
            {
            }

            public override void InstallBindings()
            {
            }
        }

        public class FooInstallerWithArgs : Installer<string, FooInstallerWithArgs>
        {
            public FooInstallerWithArgs(string foo)
            {
            }

            public override void InstallBindings()
            {
            }
        }

        void InstallMore4()
        {

            FooInstaller.Install(Container);

            Container.BindInstance("foo").WhenInjectedInto<FooInstaller>();
            FooInstaller.Install(Container);

            FooInstallerWithArgs.Install(Container, "foo");


            var foo = new Foo();
            Container.Inject(foo);

            Container.Resolve<IFoo>();

            Container.TryResolve<IFoo>();

            Container.BindInstance(new Foo());
            Container.BindInstance(new Foo());
            var foos = Container.ResolveAll<IFoo>();

            Container.Instantiate<Foo>();

            GameObject prefab1 = null;
            GameObject prefab2 = null;

            GameObject go = Container.InstantiatePrefab(prefab1);

            Foo foo2 = Container.InstantiatePrefabForComponent<Foo>(prefab2);

            Foo foo3 = Container.InstantiateComponent<Foo>(go);
        }

        public interface IFoo2
        {
        }

        public interface IFoo
        {
        }

        public interface IBar : IFoo
        {
        }

        public class Foo : MonoBehaviour, IFoo, IFoo2, IBar
        {
            public Bar GetBar()
            {
                return new Bar();
            }

            public string GetTitle()
            {
                return "title";
            }
        }

        public class Foo1 : IFoo
        {
        }

        public class Foo2 : IFoo
        {
        }

        public class Foo3 : IFoo
        {
        }

        public class Baz
        {
        }

        public class Gui
        {
        }

        public class Bar : IBar
        {
            public Foo Foo
            {
                get
                {
                    return null;
                }
            }
        }
    }
}
