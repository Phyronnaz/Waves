using UnityEngine;  

namespace Assets.Script
{
    public struct Wave
    {
        public float Amplitude;
        public float Pulsation;
        public Vector2 Position;
        public float WaveVector;

        public Wave(float amplitude, float pulsation, Vector2 position, float waveVector)
        {
            Amplitude = amplitude;
            Pulsation = pulsation;
            Position = position;
            WaveVector = waveVector;
        }
    }
}
