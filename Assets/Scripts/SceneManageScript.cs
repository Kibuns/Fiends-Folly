using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageScript : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(SceneLoadDelay(1, 5));
    }

    public void BackToStartScreen()
    {
        StartCoroutine(SceneLoadDelay(0, 3));
    }

    private IEnumerator SceneLoadDelay(int sceneIndex, float seconds)
    {
        FindObjectOfType<FadingOverlay>().FadeOut();
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(sceneIndex);
    }
}
