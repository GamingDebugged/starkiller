using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for locations in Starkiller Base Command
    /// These define origin and destination locations for ships
    /// </summary>
    [CreateAssetMenu(fileName = "New Location", menuName = "Starkiller Base/Location")]
    public class LocationData : ScriptableObject
    {
        [Header("Basic Information")]
        public string locationName;
        public string locationCode; // 3-letter code
        public enum LocationType { Planet, Moon, Station, Base, Outpost, Fleet }
        public LocationType type;
        
        public enum Affiliation { Imperial, Insurgent, Neutral, Disputed, Unknown }
        [Header("Affiliation")]
        public Affiliation affiliation;
        public bool isSecretLocation; // Whether it's a secret base not on public registries
        
        [Header("Description")]
        [TextArea(3, 6)]
        public string description;
        
        [Header("Visual")]
        public Sprite locationImage;
    }
}