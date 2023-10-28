using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        OptionsSavesManager.ApplyOptions();
    }
}
