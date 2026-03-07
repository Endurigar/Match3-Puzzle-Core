using UnityEngine;

namespace Zenject.Tests.Factories.BindFactory
{
    public class Bar : ScriptableObject
    {
        public class Factory : PlaceholderFactory<Bar>
        {
        }
    }
}

