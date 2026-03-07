using System;
using System.Collections.Generic;

#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public interface IInstantiator
    {
        T Instantiate<T>();
        T Instantiate<T>(IEnumerable<object> extraArgs);

        object Instantiate(Type concreteType);
        object Instantiate(Type concreteType, IEnumerable<object> extraArgs);

#if !NOT_UNITY3D

        TContract InstantiateComponent<TContract>(GameObject gameObject)
            where TContract : Component;
        TContract InstantiateComponent<TContract>(
            GameObject gameObject, IEnumerable<object> extraArgs)
            where TContract : Component;
        Component InstantiateComponent(
            Type componentType, GameObject gameObject);
        Component InstantiateComponent(
            Type componentType, GameObject gameObject, IEnumerable<object> extraArgs);

        T InstantiateComponentOnNewGameObject<T>()
            where T : Component;
        T InstantiateComponentOnNewGameObject<T>(string gameObjectName)
            where T : Component;
        T InstantiateComponentOnNewGameObject<T>(IEnumerable<object> extraArgs)
            where T : Component;
        T InstantiateComponentOnNewGameObject<T>(string gameObjectName, IEnumerable<object> extraArgs)
            where T : Component;

        GameObject InstantiatePrefab(UnityEngine.Object prefab);
        GameObject InstantiatePrefab(
            UnityEngine.Object prefab, Transform parentTransform);
        GameObject InstantiatePrefab(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform);

        GameObject InstantiatePrefabResource(string resourcePath);
        GameObject InstantiatePrefabResource(
            string resourcePath, Transform parentTransform);
        GameObject InstantiatePrefabResource(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform);

        T InstantiatePrefabForComponent<T>(UnityEngine.Object prefab);
        T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, IEnumerable<object> extraArgs);
        T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Transform parentTransform);
        T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Transform parentTransform, IEnumerable<object> extraArgs);
        T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform);
        T InstantiatePrefabForComponent<T>(
            UnityEngine.Object prefab, Vector3 position, Quaternion rotation, Transform parentTransform, IEnumerable<object> extraArgs);
        object InstantiatePrefabForComponent(
            Type concreteType, UnityEngine.Object prefab, Transform parentTransform, IEnumerable<object> extraArgs);

        T InstantiatePrefabResourceForComponent<T>(string resourcePath);
        T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, IEnumerable<object> extraArgs);
        T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Transform parentTransform);
        T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Transform parentTransform, IEnumerable<object> extraArgs);
        T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform);
        T InstantiatePrefabResourceForComponent<T>(
            string resourcePath, Vector3 position, Quaternion rotation, Transform parentTransform, IEnumerable<object> extraArgs);
        object InstantiatePrefabResourceForComponent(
            Type concreteType, string resourcePath, Transform parentTransform, IEnumerable<object> extraArgs);

        T InstantiateScriptableObjectResource<T>(string resourcePath)
            where T : ScriptableObject;
        T InstantiateScriptableObjectResource<T>(
            string resourcePath, IEnumerable<object> extraArgs)
            where T : ScriptableObject;
        object InstantiateScriptableObjectResource(
            Type scriptableObjectType, string resourcePath);
        object InstantiateScriptableObjectResource(
            Type scriptableObjectType, string resourcePath, IEnumerable<object> extraArgs);

        GameObject CreateEmptyGameObject(string name);
#endif
    }
}

