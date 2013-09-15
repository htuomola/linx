namespace LinkLogger.DataAccess
{
    public class Link
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Sender { get; set; }
        public string Channel { get; set; }
        public System.DateTime RegisteredAt { get; set; }
    }
}
