using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static event Action<float> OnOperationProgress;
    public static event Action<bool> OnOperationStateChanged;
    private static AsyncOperation m_Operation;

    public static void LoadScene(string sceneName, Action onCompleteLoading = null)
    {
        BeginLoad(sceneName, onCompleteLoading);
    }

    public static void ReloadScene(Action onCompleteLoading = null) =>
        LoadScene(SceneManager.GetActiveScene().name, onCompleteLoading);

    private static async void BeginLoad(string sceneName, Action onCompleteLoading)
    {
        m_Operation = SceneManager.LoadSceneAsync(sceneName);
        OnOperationStateChanged?.Invoke(true);

        while (m_Operation.isDone == false)
        {
            OnOperationProgress?.Invoke(m_Operation.progress);
            await Task.Yield();
        }

        m_Operation = null;
        OnOperationStateChanged?.Invoke(false);
        OnOperationProgress?.Invoke(0f);
        onCompleteLoading?.Invoke();
    }
}
