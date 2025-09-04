using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Helper utility to properly handle DontDestroyOnLoad for managers
    /// Fixes the "DontDestroyOnLoad only works for root GameObjects" warnings
    /// </summary>
    public static class DontDestroyOnLoadHelper
    {
        /// <summary>
        /// Safely apply DontDestroyOnLoad, ensuring the object is at root level
        /// </summary>
        public static void SafeDontDestroyOnLoad(this MonoBehaviour component)
        {
            SafeDontDestroyOnLoad(component.gameObject);
        }
        
        /// <summary>
        /// Safely apply DontDestroyOnLoad to a GameObject
        /// </summary>
        public static void SafeDontDestroyOnLoad(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("DontDestroyOnLoadHelper: Cannot apply to null GameObject");
                return;
            }
            
            // Check if already at root level
            if (obj.transform.parent == null)
            {
                Object.DontDestroyOnLoad(obj);
                Debug.Log($"DontDestroyOnLoad applied to root GameObject: {obj.name}");
                return;
            }
            
            // Move to root level first
            Debug.Log($"Moving {obj.name} to root level before applying DontDestroyOnLoad");
            
            // Store original position and rotation for debugging
            Vector3 originalPosition = obj.transform.position;
            Quaternion originalRotation = obj.transform.rotation;
            
            // Detach from parent
            obj.transform.SetParent(null);
            
            // Apply DontDestroyOnLoad now that it's at root
            Object.DontDestroyOnLoad(obj);
            
            Debug.Log($"Successfully moved {obj.name} to root and applied DontDestroyOnLoad. " +
                     $"Original position: {originalPosition}, Original rotation: {originalRotation}");
        }
        
        /// <summary>
        /// Check if a GameObject can use DontDestroyOnLoad without warnings
        /// </summary>
        public static bool CanUseDontDestroyOnLoad(GameObject obj)
        {
            return obj != null && obj.transform.parent == null;
        }
        
        /// <summary>
        /// Create a root container for managers if needed
        /// </summary>
        public static GameObject GetOrCreateManagerContainer(string containerName = "PersistentManagers")
        {
            GameObject container = GameObject.Find(containerName);
            
            if (container == null)
            {
                container = new GameObject(containerName);
                Object.DontDestroyOnLoad(container);
                Debug.Log($"Created persistent manager container: {containerName}");
            }
            
            return container;
        }
        
        /// <summary>
        /// Move a manager to the persistent container and apply DontDestroyOnLoad
        /// </summary>
        public static void MoveToPersistentContainer(this MonoBehaviour component, string containerName = "PersistentManagers")
        {
            GameObject container = GetOrCreateManagerContainer(containerName);
            
            if (component.transform.parent != container.transform)
            {
                Debug.Log($"Moving {component.name} to persistent container: {containerName}");
                component.transform.SetParent(container.transform);
            }
            
            // The container already has DontDestroyOnLoad, so child objects will persist too
        }
    }
}