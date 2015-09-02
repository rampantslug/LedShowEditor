namespace LedShowEditor.ViewModels.Events
{
    public class DeleteLedFromShowEvent
    {
        public DeleteLedFromShowEvent(LedInShowViewModel led)
        {
            Led = led;
        }

        public LedInShowViewModel Led { get; private set; }
    }
}