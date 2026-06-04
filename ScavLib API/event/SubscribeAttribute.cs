using System;

namespace ScavLib.event_bus
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class SubscribeAttribute : Attribute { }
}
