using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class InputHandlerInstancer<T> : MonoBehaviour where T : InputHandler
{
    public T InputHandlerInstance { get; protected set; }

    protected abstract void Initialize();

    private void Awake() => Initialize();
    private void Start()
    {
        List<IInputReceiver> inputReceivers = new(GetComponentsInChildren<IInputReceiver>());

        foreach (var inputReceiver in inputReceivers)
            inputReceiver.SetInputHandler(InputHandlerInstance);

        Destroy(this);
    }
}

public class InputHandler
{
    public virtual float GasRawInput { get; protected set; }
    public virtual float GasInput { get; protected set; }
    public virtual float SteerInput { get; protected set; }
    public virtual float BrakeRawInput { get; protected set; }
    public virtual float BrakeInput { get; protected set; }
    public virtual float HandbrakeInput { get; protected set; }
    public bool IsFlipped;
}
