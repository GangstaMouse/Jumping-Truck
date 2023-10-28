using UnityEngine;

[CreateAssetMenu(fileName = "New PaintData", menuName = "Resources/Paint")]
public sealed class PaintDataSO : OwnableItemDataSO
{
    [field: SerializeField] public Color Color { get; private set; } = Color.white;
    public override string ResourceFolderName => "Paints";
}
