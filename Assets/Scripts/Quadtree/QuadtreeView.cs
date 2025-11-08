using System.Collections.Generic;
using UnityEngine;

namespace Quadtree
{
    public class QuadtreeView : MonoBehaviour
    {
        private Node _root;

        private void Awake()
        {
            _root = new(new(0, 0, 50, 50), 0);

            Enemy.OnSpawn += _root.Add;
            Enemy.OnDespawn += _root.Remove;
            Enemy.OnNodeChanged += OnNodeChanged;
        }

        private void OnDestroy()
        {
            Enemy.OnSpawn -= _root.Add;
            Enemy.OnDespawn -= _root.Remove;
            Enemy.OnNodeChanged -= OnNodeChanged;
        }

        private void OnNodeChanged(ISpatialItem item)
        {
            _root.Remove(item);
            _root.Add(item);

            //_root?.DebugLog();
        }

        public List<ISpatialItem> GetEnemiesInRadius(Vector3 center, float radius)
        {
            List<ISpatialItem> enemies = new();
            _root.QueryRange(center, radius, enemies);
            return enemies;
        }


        private void OnDrawGizmos()
        {
            _root?.DrawGizmos(Color.green);
        }
    }
}
