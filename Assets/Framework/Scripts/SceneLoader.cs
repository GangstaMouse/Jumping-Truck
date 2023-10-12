using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance { get; private set; }

    private Canvas canvas;
    private AsyncOperation operation;
    public event Action<float> OnOperationProgress;
    // public event Action OnSceneLoaded;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        canvas = GetComponent<Canvas>();
    }

    public void LoadScene(string sceneName, Action onCompleteLoading = null)
    {
        canvas.enabled = true;
        StartCoroutine(BeginLoad(sceneName, onCompleteLoading));
    }

    public void ReloadScene() => LoadScene(SceneManager.GetActiveScene().name);

    private IEnumerator BeginLoad(string sceneName, Action onCompleteLoading)
    {
        operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            OnOperationProgress?.Invoke(operation.progress);
            yield return null;
        }

        operation = null;
        OnOperationProgress?.Invoke(0f);
        canvas.enabled = false;
        onCompleteLoading?.Invoke();
    }
}
