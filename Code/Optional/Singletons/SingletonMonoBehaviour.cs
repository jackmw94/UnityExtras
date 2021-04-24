using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private class SingletonNotAloneException : Exception
    {
        public SingletonNotAloneException(Type t) : base($"There are multiple instances of singleton type {t.Name}!") { }
    }
    
    private class SingletonNotPresentException : Exception
    {
        public SingletonNotPresentException(Type t) : base($"There is no instance of singleton type {t.Name}!") { }
    }
    
    private static Lazy<T> _instance = new Lazy<T>(CreateInstance);

    private static T CreateInstance()
    {
        // Search for existing instances
        T[] instances = (T[])FindObjectsOfType(typeof(T), true);
        
        // Throw exception if there is not an object of type T present
        if (instances.Length == 0)
        {
            throw new SingletonNotPresentException(typeof(T));
        }

        // Throw exception if there is multiple instances of type T present
        if (instances.Length > 1)
        {
            throw new SingletonNotAloneException(typeof(T));
        }
        
        return instances[0];
    }
    
    public static T Instance => _instance.Value;
}