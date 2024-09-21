using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelector : MonoBehaviour
{
    public void Red() {
      PlayerPrefs.SetInt("red", 1);
      PlayerPrefs.SetInt("blue", 0);
      PlayerPrefs.SetInt("black", 0);
      PlayerPrefs.Save();
    }
    public void Blue() {
      PlayerPrefs.SetInt("red", 0);
      PlayerPrefs.SetInt("blue", 1);
      PlayerPrefs.SetInt("black", 0);
      PlayerPrefs.Save();
    }
    public void Black() {
      PlayerPrefs.SetInt("red", 0);
      PlayerPrefs.SetInt("blue", 0);
      PlayerPrefs.SetInt("black", 1);
      PlayerPrefs.Save();
    }
    
}
