using UnityEngine;

namespace Assets.Script.Water
{
    public class CameraScript : MonoBehaviour
    {
        [SerializeField]
        private WorldScript WorldScript;

        void Update()
        {
            WorldScript.World.SetCameraPosition(transform.position);
        }
    }
}
