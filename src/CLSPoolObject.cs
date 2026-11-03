using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EngineGDI
{
    public class CLSPoolObject<T> where T : class, new()
    {
        // where T : class, new() pide que T sea una clase con constructor vacio
        private List<T> availableObjects = new List<T>();
        private List<T> activeObjects = new List<T>();

        public List<T> ActiveObjects => activeObjects;
        public List<T> AvailableObjects => availableObjects;

        public CLSPoolObject(int initiaSize = 100)
        {
            for (int i = 0; i < initiaSize; i++)
            {
                availableObjects.Add(new T()); // crea un objeto nuevo de tipo T
            }
        }

        public T GetObject()
        {
            T obj;

            if (availableObjects.Count > 0)
            {
                obj = availableObjects[0];
                availableObjects.RemoveAt(0);
            }
            else
            {
                obj = new T();
            }
            activeObjects.Add(obj);
            return obj;
        }
        public void RealeaseObject(T obj)
        {
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
            }
            if (!availableObjects.Contains(obj))
            {
                availableObjects.Add(obj);
            }
          
        }
        

    }
    
}