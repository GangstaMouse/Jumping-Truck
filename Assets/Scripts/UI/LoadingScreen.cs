using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Start() => SceneLoader.instance.OnOperationProgress += SetProgressBarValue;

    public void SetProgressBarValue(float value) => progressBar.value = value;

}
