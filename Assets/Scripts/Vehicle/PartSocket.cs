using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSocket : MonoBehaviour
{
    public string SocketName = "Name";
    public SocketCategory PartCategory;
    // [SerializeField] private Parts parts;
    public List<BasePart> PartsToInstall;
    public BasePart InstalledPart;



    private void Start()
    {
        if (InstalledPart)
        {
            // InstalledPart.InstallPart(this, transform.root.GetComponent<CarController>());
            InstalledPart.InstallPart(this, GetComponentInParent<CarController>());
        }
    }
}

public enum SocketCategory {Visual, Perfomance}