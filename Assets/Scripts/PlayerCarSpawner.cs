using UnityEngine;

class PlayerCarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mobileControlsUIPrefab;

    private CarDataSO m_CarData;

    private GameObject m_CarInstance;
    // private GameObject uiInstance;
    private GameObject mobileUIControlsInstance;

    private void Awake()
    {
        SpawnCar();

        if ((Application.isMobilePlatform || Application.isEditor) && mobileControlsUIPrefab)
            mobileUIControlsInstance = Instantiate(mobileControlsUIPrefab);
    }

    public void SpawnCar()
    {
        m_CarData = ItemDataSO.LoadAssetFromIdentifier<CarDataSO>(PlayerSavesManager.Vehicle.SelectedCarIdentifier, "Vehicles");
        m_CarInstance = Instantiate(m_CarData.Prefab, transform.position, transform.rotation);

        PlayerSavesManager.Vehicle.Upgrades.Utils.LoadPlayerCarTuning(m_CarInstance, PlayerSavesManager.Vehicle.SelectedCarIdentifier);

        var staticInputHandleInstancer = m_CarInstance.AddComponent<StaticInputHandlerInstancer>();
        staticInputHandleInstancer.BrakeInput = 1.0f;
    }
}
