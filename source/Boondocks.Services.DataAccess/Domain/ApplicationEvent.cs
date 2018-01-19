namespace Boondocks.Services.DataAccess.Domain
{
    public class ApplicationEvent : EntityBase
    {
        public ApplicationEventType EventType { get; set; }

        public string Message { get; set; }
    }
}