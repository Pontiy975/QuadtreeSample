using System;
using UnityEngine;
using UnityEngine.AI;

namespace Quadtree
{
    public class Enemy : MonoBehaviour, ISpatialItem
    {
        public static event Action<ISpatialItem> OnSpawn;
        public static event Action<ISpatialItem> OnDespawn;
        public static event Action<ISpatialItem> OnNodeChanged;

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float randomRadius = 10f;
        [SerializeField] private float waitTime = 1.5f;

        public Vector3 Position => transform.position;
        public Node CurrentNode { get; private set; }

        private Material _materialInstance;
        private Color _baseColor;

        private float _timer;

        private void Awake()
        {
            _materialInstance = new(meshRenderer.material);
            _baseColor = _materialInstance.color;
            meshRenderer.material = _materialInstance;
        }

        private void Start()
        {
            OnSpawn?.Invoke(this);
            MoveToRandomPoint();
        }

        private void OnDestroy()
        {
            OnDespawn?.Invoke(this);
        }

        private void Update()
        {
            if (CurrentNode != null && !CurrentNode.Contains(this))
                OnNodeChanged?.Invoke(this);

            if (agent.pathPending)
                return;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                _timer += Time.deltaTime;
                if (_timer >= waitTime)
                {
                    MoveToRandomPoint();
                    _timer = 0f;
                }
            }
        }

        public void SetNode(Node node)
        {
            CurrentNode = node;
        }

        public void SetHighlight(Color color)
        {
            _materialInstance.color = color;
        }

        public void ResetColor()
        {
            _materialInstance.color = _baseColor;
        }

        private void MoveToRandomPoint()
        {
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
            randomDir.y = 0f;

            Vector3 randomPos = transform.position + randomDir * randomRadius;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }
}
