using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Water
{
    public class World
    {
        public readonly List<Chunk> Chunks;
        public readonly GameObject gameObject;

        private Wave[] Waves;
        private AnimationCurve densityCurve;
        private int maxResolution;
        private int maxRenderDistance;

        public World(Vector3 position, int size, int maxResolution, int chunkSize, int maxRenderDistance, AnimationCurve densityCurve, Material material)
        {
            this.densityCurve = densityCurve;
            this.maxResolution = maxResolution;
            this.maxRenderDistance = maxRenderDistance;
            Waves = new Wave[0];
            Chunks = new List<Chunk>();
            gameObject = new GameObject();
            gameObject.transform.position = position;
            gameObject.name = "Water";
            if (maxResolution > 250)
            {
                throw new Exception("Chunks resolution too high");
            }
            else
            {
                for (int i = 0; i < size; i += chunkSize)
                {
                    for (int j = 0; j < size; j += chunkSize)
                    {
                        Chunks.Add(new Chunk(chunkSize, maxResolution, new Vector2(i + position.x, j + position.z), material, this));
                    }
                }
            }
        }

        public void UpdateHeights()
        {
            foreach (var chunk in Chunks)
            {
                chunk.UpdateHeights();
            }
        }

        public float GetHeight(Vector2 position)
        {
            var h = 0f;
            var time = Time.time;
            foreach (var wave in Waves)
            {
                var r = (position - wave.Position).magnitude;
                h += wave.Amplitude * Mathf.Cos(wave.WaveVector * r - wave.Pulsation * time) / (r + 1) * Mathf.Exp(-wave.Absorption * (time - wave.CreationTime));
            }
            return h;
        }

        public void Clean()
        {
            var keep = new bool[Waves.Length];
            var c = 0;
            for (int i = 0; i < Waves.Length; i++)
            {
                if (Mathf.Exp(-Waves[i].Absorption * (Time.time - Waves[i].CreationTime)) * Waves[i].Amplitude > 0.01f)
                {
                    keep[i] = true;
                    c++;
                }
            }
            var waves = new Wave[c];
            var j = 0;
            for (int i = 0; i < Waves.Length; i++)
            {
                if (keep[i])
                {
                    waves[j] = Waves[i];
                    j++;
                }
            }
            Waves = waves;
        }

        public void AddWave(float amplitude, float pulsation, Vector2 position, float waveVector, float absorption)
        {
            Array.Resize(ref Waves, Waves.Length + 1);
            Waves[Waves.Length - 1] = new Wave(amplitude, pulsation, position, waveVector, absorption, Time.time);
        }

        public void SetCameraPosition(Vector3 position)
        {
            foreach (var chunk in Chunks)
            {
                int resolution;
                var d = Vector3.Distance(position, chunk.gameObject.transform.position) / maxRenderDistance;
                if (d < 1)
                {
                    resolution = (int)(densityCurve.Evaluate(d) * maxResolution);
                }
                else
                {
                    resolution = 1;
                }
                if (Mathf.Abs(chunk.Resolution - resolution) > 10)
                {
                    chunk.SetResolution(resolution);
                }
            }
        }
    }
}
