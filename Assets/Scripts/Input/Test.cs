using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public void OnGas(InputAction.CallbackContext context)
    {
        Debug.Log("Test");
    }
}
