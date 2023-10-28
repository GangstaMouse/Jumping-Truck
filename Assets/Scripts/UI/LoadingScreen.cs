using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider m_ProgressBar;

    private static LoadingScreen m_Instance;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneLoader.OnOperationProgress += SetProgressBarValue;
        SceneLoader.OnOperationStateChanged += (state) => gameObject.SetActive(state);
    }
    public void SetProgressBarValue(float value) => m_ProgressBar.value = value;
}
