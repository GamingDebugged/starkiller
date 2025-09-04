using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for contraband items in Starkiller Base Command
    /// These define items that are not allowed on the base
    /// </summary>
    [CreateAssetMenu(fileName = "New Contraband Item", menuName = "Starkiller Base/Contraband Item")]
    public class ContrabandItem : ScriptableObject
    {
        [Header("Basic Information")]
        public string itemName;
        public enum ContrabandType { Weapon, Drug, Restricted, Political, Smuggled }
        public ContrabandType type;
        public enum Severity { Low, Medium, High, Critical }
        public Severity severity;
        
        [Header("Detection")]
        public bool visibleInScans;
        public string[] commonDisguises; // How it might be disguised or mislabeled
        public string[] commonContainers; // Where it's typically hidden
        
        [Header("Description")]
        [TextArea(3, 6)]
        public string itemDescription;
        
        [Header("Visual")]
        public Sprite itemImage;
    }
}