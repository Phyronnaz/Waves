using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Water
{
    public class Chunk
    {
        public readonly World World;
        public readonly int Size;
        public readonly Vector2 Position;

        private readonly Mesh mesh;
        private readonly GameObject gameObject;
        private readonly MeshCollider meshCollider;
        private int density;
        private Dictionary<int, Vector2> trianglesPositions;

        public Chunk(int size, int density, Vector2 position, Material material, World world)
        {
            Position = position;
            Size = size;
            World = world;
            mesh = new Mesh();
            gameObject = new GameObject();
            gameObject.transform.position = new Vector3(Position.x, 0, Position.y);
            gameObject.AddComponent<ChunkScript>().Init(this);
            gameObject.AddComponent<MeshRenderer>().material = material;
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            meshCollider = gameObject.AddComponent<MeshCollider>();
            SetDensity(density);
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
            var vertices = new Vector3[(Size * density + 1) * (Size * density + 1)];
            for (int i = 0; i < Size * density + 1; i++)
            {
                for (int j = 0; j < Size * density + 1; j++)
                {
                    var v = new Vector2((float)i / density, (float)j / density);
                    vertices[i + (Size * density + 1) * j] = new Vector3(v.x, World.GetHeight(v + Position), v.y);
                }
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
        public void SetDensity(int density)
        {
            if (this.density != density)
            {
                this.density = density;
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
            var vertices = new Vector3[(Size * density + 1) * (Size * density + 1)];
            for (int i = 0; i < Size * density + 1; i++)
            {
                for (int j = 0; j < Size * density + 1; j++)
                {
                    vertices[i + (Size * density + 1) * j] = new Vector3(i, 0, j) / density;
                }
            }

            var triangles = new int[6 * Size * density * Size * density];
            var count = 0;
            for (int i = 0; i < Size * density; i++)
            {
                for (int j = 0; j < Size * density; j++)
                {
                    var a = i + (Size * density + 1) * j;
                    var b = i + (Size * density + 1) * (j + 1);
                    var c = i + 1 + (Size * density + 1) * (j + 1);
                    var d = i + 1 + (Size * density + 1) * j;
                    triangles[count] = a;
                    triangles[count + 1] = b;
                    triangles[count + 2] = d;
                    trianglesPositions.Add(count / 3, new Vector2(i, j) / density);
                    triangles[count + 3] = b;
                    triangles[count + 4] = c;
                    triangles[count + 5] = d;
                    trianglesPositions.Add(count / 3 + 1, new Vector2(i, j) / density);
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
