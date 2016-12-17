using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Water
{
    public class Chunk
    {
        public readonly World World;
        public readonly Vector2 Position;
        public readonly GameObject gameObject;

        private readonly Mesh mesh;
        private readonly MeshCollider meshCollider;
        private readonly int size;
        private int resolution;
        private Dictionary<int, Vector2> trianglesPositions;

        public float Density { get { return (float)resolution / size; } }
        public float Resolution { get { return resolution; } }

        public Chunk(int size, int resolution, Vector2 position, Material material, World world)
        {
            Position = position;
            this.size = size;
            World = world;
            mesh = new Mesh();
            gameObject = new GameObject();
            gameObject.transform.position = new Vector3(Position.x, 0, Position.y);
            gameObject.transform.SetParent(World.gameObject.transform);
            gameObject.name = "Chunk " + (position / size).ToString();
            gameObject.AddComponent<ChunkScript>().Init(this);
            gameObject.AddComponent<MeshRenderer>().material = material;
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            meshCollider = gameObject.AddComponent<MeshCollider>();
            SetResolution(resolution);
        }


        public Vector2 GetHitPosition(int triangleIndex)
        {
            Vector2 v;
            if (trianglesPositions.TryGetValue(triangleIndex, out v))
            {
                v += Position;
            }
            else
            {
                throw new System.Exception("Hit position not found");
            }
            return v;
        }
        public void UpdateHeights()
        {
            var vertices = new Vector3[(resolution + 1) * (resolution + 1)];
            for (int i = 0; i < resolution + 1; i++)
            {
                for (int j = 0; j < resolution + 1; j++)
                {
                    var v = new Vector2(i, j) / Density;
                    vertices[i + (resolution + 1) * j] = new Vector3(v.x, World.GetHeight(v + Position), v.y);
                }
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
        public void SetResolution(int resolution)
        {
            if (this.resolution != resolution)
            {
                this.resolution = resolution;
                mesh.Clear();
                InitMesh();
                meshCollider.sharedMesh = mesh;
            }
        }
        public void Destroy()
        {
            Object.Destroy(gameObject);
            Object.Destroy(mesh);
        }

        void InitMesh()
        {
            trianglesPositions = new Dictionary<int, Vector2>();
            var vertices = new Vector3[(resolution + 1) * (resolution + 1)];
            for (int i = 0; i < resolution + 1; i++)
            {
                for (int j = 0; j < resolution + 1; j++)
                {
                    vertices[i + (resolution + 1) * j] = new Vector3(i, 0, j) / Density;
                }
            }

            var triangles = new int[6 * resolution * resolution];
            var count = 0;
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    var a = i + (resolution + 1) * j;
                    var b = i + (resolution + 1) * (j + 1);
                    var c = i + 1 + (resolution + 1) * (j + 1);
                    var d = i + 1 + (resolution + 1) * j;
                    triangles[count] = a;
                    triangles[count + 1] = b;
                    triangles[count + 2] = d;
                    trianglesPositions.Add(count / 3, new Vector2(i, j) / Density);
                    triangles[count + 3] = b;
                    triangles[count + 4] = c;
                    triangles[count + 5] = d;
                    trianglesPositions.Add(count / 3 + 1, new Vector2(i, j) / Density);
                    count += 6;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.UploadMeshData(false);
        }
    }
}
