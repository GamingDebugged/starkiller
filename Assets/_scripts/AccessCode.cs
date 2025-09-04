using UnityEngine;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for access codes in Starkiller Base Command
    /// These define codes that ships need to dock at the base
    /// </summary>
    [CreateAssetMenu(fileName = "New Access Code", menuName = "Starkiller Base/Access Code")]
    public class AccessCode : ScriptableObject
    {
        [Header("Basic Information")]
        public string codeName; // Name/description of the code
        public string codeValue; // The actual code value
        public enum CodeType { Standard, VIP, Emergency, Temporary, Special }
        public CodeType type;
        
        [Header("Validity")]
        public int validFromDay; // First day it's valid
        public int validUntilDay = -1; // Last day it's valid (-1 = no expiration)
        public bool isRevoked; // Whether code has been revoked
        
        [Header("Red Herring")]
        public bool isRedHerring = false;
        public string mimicsCode = ""; // e.g., "MIL-1001"

        public enum AccessLevel { Low, Medium, High, Unrestricted }
        [Header("Access Level")]
        public AccessLevel level;
        public string[] authorizedFactions; // Which factions are authorized to use this
    }
}