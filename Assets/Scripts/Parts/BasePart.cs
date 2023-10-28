using UnityEngine;

[CreateAssetMenu]
public class BasePart : OwnableItemDataSO
{
    [Header("Base Data")]
    public string Name;
    public string Description;
    public CollectionType collection;
    public bool Hiden;
    public string Condition;
    public float Cost;
    public int MinLevel;
    public GameObject ModelPrefab;

    public override string ResourceFolderName => throw new System.NotImplementedException();

    // public int MinReputation;
    // Или если будет выполнено какое то определенное задание / событие

    public virtual PartInstance CreatePartInstance(Transform socketTransform)
    {
        PartInstance partInstance = new(this, socketTransform);

        if (ModelPrefab)
            partInstance.ModelInstance = Instantiate(ModelPrefab, socketTransform);
            
        return partInstance;
    }
}

public class PartInstance
{
    public readonly string Identifier;
    public readonly Transform SocketTransform;
    public GameObject ModelInstance;

    public PartInstance(BasePart part, Transform socketTransform)
    {
        Identifier = part.Identifier;
        SocketTransform = socketTransform;
    }

    public virtual void Remove()
    {
        MonoBehaviour.Destroy(ModelInstance);
    }
}

public enum CollectionType {Buy, Collectible}