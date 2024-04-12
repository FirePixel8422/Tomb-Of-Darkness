using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    public bool used;
    public float delay;
    public float transitionDuration;
    public GameObject transitionOverlay;

    public void OnTriggerEnter(Collider other)
    {
        if (used)
        {
            return;
        }

        used = true;
        PlayerController.Instance.canMove = false;

        StartCoroutine(ReloadSceneWithTransition());
    }

    private IEnumerator ReloadSceneWithTransition()
    {
        yield return new WaitForSeconds(delay);
        PlayerController.Instance.rb.isKinematic = false;

        transitionOverlay.SetActive(true);
        yield return new WaitForSeconds(transitionDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        transitionOverlay.SetActive(false);
        PlayerController.Instance.rb.isKinematic = false;
    }
}
