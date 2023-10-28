using UnityEngine;

public abstract class OwnableItemDataSO : ItemDataSO
{
    [field: SerializeField] public int Price { get; private set; } = 100;
}

public interface IResource
{
    public abstract string Identifier { get; }
    public abstract string ResourceFolderName { get; }
}

public interface IUnlockable
{
    public abstract string Requirements { get; }
}
