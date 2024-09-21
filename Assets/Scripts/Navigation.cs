using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
  public void LoadGame()
  {
    SceneManager.LoadScene("Game", LoadSceneMode.Single);
  }
  public void LoadMainMenu()
  {
    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
  }
  public void LoadRules()
  {
    SceneManager.LoadScene("Rules", LoadSceneMode.Single);
  }
  public void LoadSettings()
  {
    SceneManager.LoadScene("Settings", LoadSceneMode.Single);
  }

  public void Quit() {
    Application.Quit();
  }
}
