using UnityEngine;

namespace Quadtree.Player.Data
{
    [CreateAssetMenu(fileName = "PlayerModel", menuName = "ScriptableObjects/PlayerModel")]
    public class PlayerModel : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float DetectRadius { get; private set; } = 10f;
        [field: SerializeField] public float DetectInterval { get; private set; } = 0.2f;
    }
}
