using UnityEngine;

[DisallowMultipleComponent]
public class ReverselightEffect : MonoBehaviour
{
    private Gearbox m_Gearbox;
    private Material m_Material;

    private void Awake()
    {
        m_Gearbox = GetComponentInParent<Gearbox>();
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable() => m_Gearbox.OnChangengGearCompleted += SetIntencity;
    private void OnDisable() => m_Gearbox.OnChangengGearCompleted -= SetIntencity;
    private void SetIntencity(int value) => m_Material.SetFloat("Intensity", value == 0 ? 1.0f : 0.0f);
}
