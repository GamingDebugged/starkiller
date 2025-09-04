using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starkiller.Core
{
    /// <summary>
    /// Service Locator pattern implementation to replace FindObjectOfType calls
    /// This is the foundation for proper dependency management
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private Dictionary<Type, Lazy<object>> _lazyServices = new Dictionary<Type, Lazy<object>>();

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ServiceLocator>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ServiceLocator");
                        _instance = go.AddComponent<ServiceLocator>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Register a service instance
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            Instance._services[typeof(T)] = service;
            Debug.Log($"[ServiceLocator] Registered service: {typeof(T).Name}");
        }

        /// <summary>
        /// Register a lazy-loaded service
        /// </summary>
        public static void RegisterLazy<T>(Func<T> factory) where T : class
        {
            Instance._lazyServices[typeof(T)] = new Lazy<object>(() => factory());
            Debug.Log($"[ServiceLocator] Registered lazy service: {typeof(T).Name}");
        }

        /// <summary>
        /// Get a registered service
        /// </summary>
        public static T Get<T>() where T : class
        {
            // Check regular services first
            if (Instance._services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }

            // Check lazy services
            if (Instance._lazyServices.TryGetValue(typeof(T), out Lazy<object> lazyService))
            {
                service = lazyService.Value;
                Instance._services[typeof(T)] = service; // Cache it
                Instance._lazyServices.Remove(typeof(T));
                return service as T;
            }

            // Fallback to FindFirstObjectByType (for migration period)
            // Only works for MonoBehaviour-derived classes
            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                MonoBehaviour found = FindFirstObjectByType(typeof(T)) as MonoBehaviour;
                if (found != null)
                {
                    T foundService = found as T;
                    if (foundService != null)
                    {
                        Debug.LogWarning($"[ServiceLocator] Service {typeof(T).Name} not registered, found via FindFirstObjectByType");
                        Register(foundService);
                        return foundService;
                    }
                }
            }

            Debug.LogError($"[ServiceLocator] Service {typeof(T).Name} not found!");
            return null;
        }

        /// <summary>
        /// Try to get a service without errors
        /// </summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            service = Get<T>();
            return service != null;
        }

        /// <summary>
        /// Check if a service is registered
        /// </summary>
        public static bool Has<T>() where T : class
        {
            return Instance._services.ContainsKey(typeof(T)) || 
                   Instance._lazyServices.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Clear all services (useful for testing)
        /// </summary>
        public static void Clear()
        {
            Instance._services.Clear();
            Instance._lazyServices.Clear();
            Debug.Log("[ServiceLocator] All services cleared");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}