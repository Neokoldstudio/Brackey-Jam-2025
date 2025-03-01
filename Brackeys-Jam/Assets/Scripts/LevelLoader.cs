using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public static LevelLoader Instance;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    IEnumerator LoadLevelCoroutine(int levelIndex)
    {
        transition.SetTrigger("End");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelIndex);
    }
}
