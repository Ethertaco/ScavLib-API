using UnityEngine;

namespace ScavLib.event_bus
{

    public abstract class BusEvent
    {

        public float Timestamp { get; } = Time.realtimeSinceStartup;
    }
}
