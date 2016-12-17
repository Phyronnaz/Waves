using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Script.Water
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WorldScript : MonoBehaviour
    {
        [SerializeField]
        private int Size = 100;
        [SerializeField]
        private int MaxResolution = 100;
        [SerializeField]
        private int ChunkSize = 25;
        [SerializeField]
        private int MaxRenderDistance = 100;
        [SerializeField]
        private int RefreshCount = 1;
        [SerializeField]
        private Material Material;
        [SerializeField]
        private AnimationCurve DensityCurve;

        public World World;

        private int currentCount = 0;

        void Start()
        {
            World = new World(transform.position, Size, MaxResolution, ChunkSize, MaxRenderDistance, DensityCurve, Material);
            for (int i = 0; i < 100; i += 10)
            {
                World.AddWave(10, 10, Vector2.one * i, 1, 1);
            }
            InvokeRepeating("Clean", 0, 1);
        }

        void Update()
        {
            currentCount++;
            if (currentCount > RefreshCount)
            {
                currentCount = 0;
                World.UpdateHeights();
            }
            if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 1000000))
                    {
                        var chunkScript = hit.transform.GetComponent<ChunkScript>();
                        if (chunkScript != null)
                        {
                            chunkScript.World.AddWave(10, 10, chunkScript.GetHitPosition(hit.triangleIndex), 1, 0);
                        }
                    }
                }
            }
        }

        void Clean()
        {
            World.Clean();
        }
    }
}