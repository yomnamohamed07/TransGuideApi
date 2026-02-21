namespace TransGuide.Services.Modelsls
{
    public class UpdateUserDataRequest
    {
        public string? FullName { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
    }
}