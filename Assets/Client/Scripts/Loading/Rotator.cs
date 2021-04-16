using UnityEngine;

namespace MonsterWorld.Unity
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 axis = Vector3.forward;
        public float anglePerSecond = 60f;

        void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(anglePerSecond * Time.deltaTime, axis);
        }
    }
}
