namespace TestClientsRequests.Entity
{
    public class ConsultDetail
    {
        public int Id { get; set; }
        public Region? Region { get; set; }
        public string? City { get; set; }
        public string? FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Organization { get; set; }
        public TypeOfProduct? TypeOfProduct { get; set;}
        public string FromLandingPage { get; set; }
    }
}
