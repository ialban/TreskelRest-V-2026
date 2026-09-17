namespace LibHttpClient.Model
{
    public class StandardJsonResponse
    {
        public bool success { get; set; }
        public string reason { get; set; }
        public object data { get; set; }
    }
}
