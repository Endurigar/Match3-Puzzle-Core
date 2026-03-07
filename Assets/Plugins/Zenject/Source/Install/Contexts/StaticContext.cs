#if !NOT_UNITY3D

namespace Zenject
{
    public static class StaticContext
    {
        static DiContainer _container;

        public static void Clear()
        {
            _container = null;
        }

        public static bool HasContainer
        {
            get { return _container != null; }
        }

        public static DiContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new DiContainer();
                }

                return _container;
            }
        }
    }
}

#endif
