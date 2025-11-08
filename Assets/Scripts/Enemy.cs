using System;
using UnityEngine;

namespace Quadtree
{
    public class Enemy : MonoBehaviour, ISpatialItem
    {
        public static event Action<ISpatialItem> OnSpawn;
        public static event Action<ISpatialItem> OnDespawn;

        public Vector3 Position => transform.position;

        private void Start()
        {
            OnSpawn?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnDespawn?.Invoke(this);
        }
    }
}
