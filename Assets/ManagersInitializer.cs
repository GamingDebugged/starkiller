using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using StarkillerBaseCommand;

  public class ManagersInitializer : MonoBehaviour
  {
      private void Awake()
      {
          // Make the entire managers container persistent
          DontDestroyOnLoad(gameObject);
      }
  }
