namespace WebPortal.API.Models
{
    public class MessageButtonResponseModel : BaseButtonResponseModel
    {
        public int MessageId { get; set; }

        public string MessageEN { get; set; }

        public string MessageAR { get; set; }
    }
}