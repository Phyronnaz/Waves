using UnityEngine;  

namespace Assets.Script.Water
{
    struct Wave
    {
        public readonly float Amplitude;
        public readonly float Pulsation;
        public readonly Vector2 Position;
        public readonly float WaveVector;
        public readonly float Absorption;
        public readonly float CreationTime;

        public Wave(float amplitude, float pulsation, Vector2 position, float waveVector, float absorption, float creationTime)
        {
            Amplitude = amplitude;
            Pulsation = pulsation;
            Position = position;
            WaveVector = waveVector;
            Absorption = absorption;
            CreationTime = creationTime;
        }
    }
}
