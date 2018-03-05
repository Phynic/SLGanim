namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    // Example script from the Unity asset store.
    public class SerializeAnything : SerializedMonoBehaviour
    {
        public MyGeneric<float> MyGenericFloat;

        public MyGeneric<GameObject[]> MyGenericGameObjects;

        public Dictionary<string, ISomeInterface> MyDictionary;

        public Vector3? NullableVector3;
    }

    public interface ISomeInterface { }

    public class ImplA : ISomeInterface
    {
        public float A;
    }

    public class ImplB : ISomeInterface
    {
        public float B;
        public ISomeInterface C;
    }

    public class MyGeneric<T>
    {
        public T SomeVariable;
    }
}