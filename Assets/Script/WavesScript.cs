using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Script
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WavesScript : MonoBehaviour
    {
        public static int RefreshCount = 2;

        Mesh mesh;
        int size = 250;
        float density = 0.5f;
        int currentCount = 0;

        Wave[] Waves = new Wave[0];

        Dictionary<int, Vector2> trianglesPositions = new Dictionary<int, Vector2>();

        void Start()
        {
            Profiler.maxNumberOfSamplesPerFrame = 8000000;

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            InitMesh();

            //AddWave(Vector2.one * 10, 1, 10, new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) / 2);
            AddWave(10, 10, Vector2.one * 50, 1);
        }

        void InitMesh()
        {
            var vertices = new Vector3[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    vertices[i + size * j] = new Vector3(i, 0, j) * density;
                    //var go = new GameObject();
                    //var txt = go.AddComponent<TextMesh>();
                    //txt.text = (i + size * j).ToString();
                    //txt.fontSize = 100;
                    //txt.characterSize = 0.05f;
                    //go.transform.position = new Vector3(i, 0, j);
                }
            }

            var triangles = new int[6 * (size - 1) * (size - 1)];
            var count = 0;
            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size - 1; j++)
                {
                    var a = i + size * j;
                    var b = i + size * (j + 1);
                    var c = i + 1 + size * (j + 1);
                    var d = i + 1 + size * j;
                    triangles[count] = a;
                    triangles[count + 1] = b;
                    triangles[count + 2] = d;
                    trianglesPositions.Add(count / 3, new Vector2(a % size, a / size));
                    triangles[count + 3] = b;
                    triangles[count + 4] = c;
                    triangles[count + 5] = d;
                    trianglesPositions.Add(count / 3 + 1, new Vector2(b % size, a / size));
                    count += 6;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.UploadMeshData(false);
            gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        void Update()
        {
            currentCount++;
            if (currentCount > RefreshCount)
            {
                currentCount = 0;
                UpdateHeights();
            }
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000000))
                {
                    Vector2 v;
                    if (trianglesPositions.TryGetValue(hit.triangleIndex, out v))
                    {
                        AddWave(10, 10, v, 1);
                    }
                }
            }
            //}
        }

        void UpdateHeights()
        {
            var vertices = new Vector3[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    vertices[i + size * j] = new Vector3(i * density, GetHeight(new Vector2(i, j), Time.time), j * density);
                }
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        float GetHeight(Vector2 position, float time)
        {
            var h = 0f;
            for (int i = 0; i < Waves.Length; i++)
            {
                var r = (position - Waves[i].Position).magnitude;
                h += Waves[i].Amplitude * Mathf.Cos(Waves[i].WaveVector * r - Waves[i].Pulsation * time) / (r + 0.1f);
            }
            return h;
        }

        void AddWave(float amplitude, float pulsation, Vector2 position, float waveVector)
        {
            System.Array.Resize<Wave>(ref Waves, Waves.Length + 1);
            Waves[Waves.Length - 1] = new Wave(amplitude, pulsation, position, waveVector);
        }
    }
}