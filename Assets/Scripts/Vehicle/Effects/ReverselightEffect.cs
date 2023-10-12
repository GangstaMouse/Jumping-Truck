using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverselightEffect : MonoBehaviour
{
    // [SerializeField] private Material material;

    private Gearbox gearbox;
    private Material materialInstance;

    private void Awake()
    {
        gearbox = GetComponentInParent<Gearbox>();
        materialInstance = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable() => gearbox.OnShiftGear += SetIntencity;

    private void OnDisable() => gearbox.OnShiftGear -= SetIntencity;

    private void SetIntencity(int value) => materialInstance.SetFloat("Intensity", value);
}
