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
        }

        private void OnDestroy()
        {
            Enemy.OnSpawn -= _root.Add;
            Enemy.OnDespawn -= _root.Remove;
        }

        //private void Update()
        //{
        //    _root?.DebugLog();
        //}

        private void OnDrawGizmos()
        {
            _root?.DrawGizmos();
        }
    }
}
