using System.Collections.Generic;
using UnityEngine;

namespace Quadtree
{
    public class Node
    {
        private const int MAX_ITEMS = 4;
        private const int MAX_LEVEL = 5;

        private Rect _rect;
        private int _level;

        public int Level => _level;

        private readonly List<Node> _children = new();
        private readonly HashSet<ISpatialItem> _items = new();

        public Node(Rect rect, int level)
        {
            _rect = rect;
            _level = level;
        }

        public void Add(ISpatialItem newItem)
        {
            if (_children.Count == 0)
            {
                if (_items.Count < MAX_ITEMS || _level == MAX_LEVEL)
                {
                    _items.Add(newItem);
                    newItem.SetNode(this);
                    return;
                }

                float width = _rect.width / 2f;
                float height = _rect.height / 2f;
                int level = _level + 1;

                Node n1 = new(new(_rect.x, _rect.y, width, height), level);
                Node n2 = new(new(_rect.x + width, _rect.y, width, height), level);
                Node n3 = new(new(_rect.x, _rect.y + height, width, height), level);
                Node n4 = new(new(_rect.x + width, _rect.y + height, width, height), level);

                _children.AddRange(new Node[] { n1, n2, n3, n4 });

                foreach (ISpatialItem item in _items)
                {
                    foreach (Node child in _children)
                    {
                        if (child.Contains(item))
                            child.Add(item);
                    }
                }

                _items.Clear();
            }

            foreach (Node child in _children)
            {
                if (child.Contains(newItem))
                    child.Add(newItem);
            }
        }

        public void Remove(ISpatialItem item)
        {
            if (_items.Contains(item))
                _items.Remove(item);

            foreach (Node child in _children)
                child.Remove(item);

            TryToMergeChildren();
        }

        public bool Contains(ISpatialItem item) => _rect.Contains(new Vector2(item.Position.x, item.Position.z));
        
        // just for tests
        public void QueryRange(Vector3 center, float radius, List<ISpatialItem> result)
        {
            if (!IntersectsCircle(center, radius))
                return;

            foreach (ISpatialItem item in _items)
            {
                result.Add(item);
            }

            foreach (Node child in _children)
                child.QueryRange(center, radius, result);
        }

        //public void QueryRange(Vector3 center, float radius, List<ISpatialItem> result)
        //{
        //    float radiusSqr = radius * radius;

        //    if (!IntersectsCircle(center, radius))
        //        return;

        //    foreach (ISpatialItem item in _items)
        //    {
        //        float sqrDist = (new Vector3(item.Position.x, 0, item.Position.z) - new Vector3(center.x, 0, center.z)).sqrMagnitude;
        //        if (sqrDist <= radiusSqr)
        //            result.Add(item);
        //    }

        //    foreach (Node child in _children)
        //        child.QueryRange(center, radius, result);
        //}

        private void TryToMergeChildren()
        {
            int count = 0;

            foreach (Node child in _children)
            {
                if (child._children.Count > 0)
                    return;

                count += child._items.Count;
            }

            //Debug.Log($"{Level}: {count} > {MAX_ITEMS}");
            if (count > MAX_ITEMS)
                return;

            foreach (Node child in _children)
            {
                foreach (var item in child._items)
                    item.SetNode(this);

                _items.UnionWith(child._items);
            }

            _children.Clear();
        }

        private void Reset()
        {
            _children.Clear();
            _items.Clear();
        }

        private bool IntersectsCircle(Vector3 center, float radius)
        {
            float closestX = Mathf.Clamp(center.x, _rect.x, _rect.x + _rect.width);
            float closestZ = Mathf.Clamp(center.z, _rect.y, _rect.y + _rect.height);

            float dx = center.x - closestX;
            float dz = center.z - closestZ;

            return dx * dx + dz * dz <= radius * radius;
        }

        #region debug
        public void DrawGizmos(Color color)
        {
            Vector3 center = new(_rect.x + _rect.width / 2f, 0, _rect.y + _rect.height / 2f);
            Vector3 size = new(_rect.width, 0, _rect.height);

            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);

            foreach (var child in _children)
                child.DrawGizmos(color);
        }

        //public void DebugLog(string indent = "")
        //{
        //    Debug.Log($"{indent}Level {_level} | Rect: ({_rect.x}, {_rect.y}, {_rect.width}, {_rect.height}) | Items: {_items.Count}");

        //    foreach (var child in _children)
        //        child.DebugLog(indent + "  ");
        //}
        #endregion
    }
}
