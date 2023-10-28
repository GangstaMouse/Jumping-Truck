using UnityEngine;

[DisallowMultipleComponent]
public class BrakelightEffect : MonoBehaviour
{
    private Vehicle m_Vehicle;
    private Material m_Material;

    private void Awake()
    {
        m_Vehicle = GetComponentInParent<Vehicle>();
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable() => m_Vehicle.OnLateUpdateVisualEffects += SetIntencity;
    private void OnDisable() => m_Vehicle.OnLateUpdateVisualEffects -= SetIntencity;
    private void SetIntencity() => m_Material.SetFloat("Intensity", m_Vehicle.InputHandler.BrakeInput);
}
