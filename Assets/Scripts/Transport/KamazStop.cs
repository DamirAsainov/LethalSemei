using UnityEngine;

namespace Transports
{
    public class KamazStop: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<WaypointDrive>())
            {
                other.GetComponent<WaypointDrive>().finishedDriving = true;
            }
        }
    }
}