using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class TerrainTreesColliderFixer : MonoBehaviour
{
    private void Awake()
    {
        TerrainCollider terrainCollider = GetComponent<TerrainCollider>();
        terrainCollider.enabled = false;
        terrainCollider.enabled = true;
    }
}
