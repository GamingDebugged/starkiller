using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipImageManager : MonoBehaviour
{
    [System.Serializable]
    public class ShipImage
    {
        public string shipType;
        public Sprite image;
    }
    
    [System.Serializable]
    public class CaptainImage
    {
        public string faction;
        public Sprite[] portraits;
    }
    
    public ShipImage[] shipImages;
    public CaptainImage[] captainImages;
    
    void Start()
    {
        Debug.Log("ShipImageManager started successfully");
    }
    
    public Sprite GetShipImage(string shipType)
    {
        foreach (var item in shipImages)
        {
            if (item.shipType == shipType)
                return item.image;
        }
        return null;
    }
    
    public Sprite GetCaptainImage(string faction)
    {
        foreach (var item in captainImages)
        {
            if (item.faction == faction && item.portraits.Length > 0)
                return item.portraits[Random.Range(0, item.portraits.Length)];
        }
        return null;
    }
}