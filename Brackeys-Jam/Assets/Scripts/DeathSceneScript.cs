using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneScript : MonoBehaviour
{

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ChangeScene(int scene)
    {
        ScoreManager.Instance.Reset();
        LevelLoader.Instance.LoadLevel(scene);
    }
}
