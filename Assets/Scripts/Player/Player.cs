using System.Collections;
using System.Collections.Generic;
using Quadtree.Player.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Quadtree.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerModel model;
        [SerializeField] private QuadtreeView quadtree;

        private List<ISpatialItem> _detectedEnemies = new();
        private HashSet<ISpatialItem> _previouslyDetected = new();

        private void Start()
        {
            StartCoroutine(EnemyDetectionRoutine());
        }

        private void Update()
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * model.Speed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            newPosition.x = Mathf.Clamp(newPosition.x, 0f, 50f);
            newPosition.z = Mathf.Clamp(newPosition.z, 0f, 50f);

            transform.position = newPosition;
        }

        private IEnumerator EnemyDetectionRoutine()
        {
            WaitForSeconds wait = new(model.DetectInterval);

            while (true)
            {
                _detectedEnemies.Clear();
                _detectedEnemies = quadtree.GetEnemiesInRadius(transform.position, model.DetectRadius);

                HashSet<ISpatialItem> currentlyDetected = new();

                foreach (var enemy in _detectedEnemies)
                {
                    (enemy as Enemy).SetHighlight(Color.cyan);
                    currentlyDetected.Add(enemy);
                }

                foreach (var enemy in _detectedEnemies)
                {
                    Vector3 enemyPos = (enemy as Enemy).Position;
                    float sqrDist = (enemyPos - transform.position).sqrMagnitude;
                    if (sqrDist <= model.DetectRadius * model.DetectRadius)
                    {
                        (enemy as Enemy).SetHighlight(Color.red);
                    }
                }

                foreach (var enemy in _previouslyDetected)
                {
                    if (!currentlyDetected.Contains(enemy))
                        (enemy as Enemy).ResetColor();
                }

                _previouslyDetected.Clear();
                foreach (var enemy in currentlyDetected)
                    _previouslyDetected.Add(enemy);

                yield return wait;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (model == null)
                return;

            Handles.color = Color.yellow;
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, model.DetectRadius);
        }
#endif
    }
}
