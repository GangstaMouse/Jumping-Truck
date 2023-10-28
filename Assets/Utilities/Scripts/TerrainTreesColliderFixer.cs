#if UNITY_EDITOR
using UnityEngine;

public class TerrainTreesColliderFixer : MonoBehaviour
{
    private void OnValidate()
    {
        TerrainCollider terrainCollider = GetComponent<TerrainCollider>();
        terrainCollider.enabled = false;
        terrainCollider.enabled = true;
    }
}
#endif
