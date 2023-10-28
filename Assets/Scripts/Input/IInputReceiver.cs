public interface IInputReceiver
{
    public abstract InputHandler InputHandler { get; protected set; }

    internal void SetInputHandler(in InputHandler inputHandler)
    {
        InputHandler = inputHandler;
        OnInputHandlerChanger(inputHandler);
    }

    internal protected abstract void OnInputHandlerChanger(in InputHandler inputHandler);
}
