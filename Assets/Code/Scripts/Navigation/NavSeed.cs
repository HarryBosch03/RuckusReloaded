using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

namespace RuckusReloaded.Runtime.Navigation
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class NavSeed : MonoBehaviour
    {
        public float gridSize;
        public float checkHeight;
        public float stepHeight;
        public LayerMask checkMask;
        public bool bake;
        public bool stop;
        public bool clear;
        public string status;

        [SerializeField]
        [HideInInspector]
        public List<Cell> bakeData = new();

        private void OnValidate()
        {
            if (bake)
            {
                bake = false;
                Bake();
            }

            if (clear)
            {
                clear = false;
                bakeData.Clear();
            }
        }

        public void Bake()
        {
            status = "Initializing Bake";

            bakeData.Clear();
            var closedCells = new HashSet<Vector2Int>();
            var openCells = new Queue<Cell>();

            openCells.Enqueue(new Cell(ToKey(transform.position), transform.position));

#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutine(routine(), this);
#else
            StartCoroutine(routine());
#endif

            IEnumerator routine()
            {
                var neighbours = new Vector2Int[]
                {
                    new(-1, 0),
                    new(1, 0),
                    new(0, -1),
                    new(0, 1),
                };

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (openCells.Count > 0)
                {
                    var seed = openCells.Dequeue();

                    foreach (var offset in neighbours)
                    {
                        var key = seed.key + offset;
                        if (closedCells.Contains(key)) continue;
                        closedCells.Add(key);

                        status = $"Checking [{key.x}, {key.y}]";

                        var position = ToWorldPosition(key, seed.groundPosition.y);
                        if (!CheckPosition(position, out var groundPosition)) continue;

                        var cell = new Cell(key, groundPosition);
                        openCells.Enqueue(cell);
                        bakeData.Add(cell);

                        if (stopwatch.ElapsedMilliseconds < 10.0f) continue;
                        stopwatch.Restart();
                        yield return null;
                        if (stop)
                        {
                            stop = false;
                            status = "Aborted Bake";
                            yield break;
                        }
                    }
                }

                status = "Bake Complete";
            }
        }

        private bool CheckPosition(Vector3 position, out Vector3 groundPosition)
        {
            groundPosition = position;
            var center = Vector3.up * (checkHeight + stepHeight) / 2.0f + position;
            var size = new Vector3(gridSize, checkHeight - stepHeight, gridSize);

            if (Physics.CheckBox(center, size / 2.0f, Quaternion.identity, checkMask)) return false;

            var ray = new Ray(Vector3.up * stepHeight + position, Vector3.down);
            if (!Physics.Raycast(ray, out var hit, stepHeight * 2.0f, checkMask)) return false;

            groundPosition = hit.point;
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var cell in bakeData)
            {
                Gizmos.DrawWireCube(cell.groundPosition, new Vector3(gridSize, 0.0f, gridSize) * 0.9f);
            }

            var center = Vector3.up * (checkHeight + stepHeight) / 2.0f + transform.position;
            var size = new Vector3(gridSize, checkHeight - stepHeight, gridSize);
            Gizmos.DrawCube(center, size);
            Gizmos.DrawRay(Vector3.up * stepHeight + transform.position, Vector3.down * stepHeight);
        }

        public Vector2Int ToKey(Vector3 position)
        {
            return new Vector2Int
            {
                x = Mathf.RoundToInt(position.x / gridSize),
                y = Mathf.RoundToInt(position.z / gridSize),
            };
        }

        public Vector3 ToWorldPosition(Vector2Int key, float y = 0.0f)
        {
            var planePosition = (Vector2)key * gridSize;
            return new Vector3(planePosition.x, y, planePosition.y);
        }

        [Serializable]
        public struct Cell
        {
            public Vector2Int key;
            public Vector3 groundPosition;

            public Cell(Vector2Int key, Vector3 groundPosition)
            {
                this.key = key;
                this.groundPosition = groundPosition;
            }
        }
    }
}