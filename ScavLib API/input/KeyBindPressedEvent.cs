namespace ScavLib.input.events_
{

    public class KeyBindPressedEvent : ScavLib.event_bus.BusEvent
    {
        public KeyBindDefinition Bind { get; }
        public string FullId => Bind?.FullId;
        public string OwnerModName => Bind?.OwnerModName;
        public string LocalId => Bind?.LocalId;

        public KeyBindPressedEvent(KeyBindDefinition bind) { Bind = bind; }
    }
}