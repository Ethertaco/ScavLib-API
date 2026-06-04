namespace ScavLib.event_bus.events
{

    public class ItemPickedUpEvent : BusEvent
    {

        public Item Item { get; }

        public int Slot { get; }

        public Body Body { get; }

        public string ItemId => Item != null ? Item.id : null;

        public ItemPickedUpEvent(Item item, int slot, Body body)
        {
            Item = item;
            Slot = slot;
            Body = body;
        }
    }
}
