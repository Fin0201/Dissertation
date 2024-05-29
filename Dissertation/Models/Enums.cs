namespace Dissertation.Models
{
    public class Enums
    {
        public enum ItemStatus
        {
            Available,
            Unavailable
        }

        public enum RequestStatus
        {
            Pending,
            Accepted,
            Rejected,
            Completed,
            NoResponse
        }
    }
}
