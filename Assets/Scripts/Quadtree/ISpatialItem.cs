using UnityEngine;

namespace Quadtree
{
    public interface ISpatialItem
    {
        public Vector3 Position { get; }
        public Node CurrentNode { get; }

        public void SetNode(Node node);
    }
}
