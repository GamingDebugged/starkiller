using UnityEngine;
using UnityEngine.Video;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// ScriptableObject for media assets in Starkiller Base Command
    /// These define visual and audio assets used in the game
    /// </summary>
    [CreateAssetMenu(fileName = "New Media Asset", menuName = "Starkiller Base/Media Asset")]
    public class MediaAsset : ScriptableObject
    {
        [Header("Basic Information")]
        public string assetName;
        public enum AssetType { Ship, Captain, Background, UI, Special }
        public AssetType type;
        public string associatedId; // ID of associated ship type, captain type, etc.
        
        [Header("Visual Assets")]
        public Sprite[] sprites; // Collection of sprites
        public VideoClip[] videoClips; // Collection of video clips
        
        [Header("Audio Assets")]
        public AudioClip[] audioClips; // Collection of audio clips
        
        [Header("Description")]
        [TextArea(2, 4)]
        public string assetDescription;
    }
}