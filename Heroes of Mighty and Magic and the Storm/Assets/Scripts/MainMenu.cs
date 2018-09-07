using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject loadingScreen;

    public Slider slider;

    public void GameStart()
    {
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;

            yield return null;
        }
    }

    //退出游戏
    public void Quit()
    {
        Application.Quit();
    }
}
