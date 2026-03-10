public class FilterDto
{
    public string UserLocation { get; set; } = string.Empty;
    public decimal UserLatitude { get; set; }
    public decimal UserLongitude { get; set; }

    public string Destination { get; set; } = string.Empty;
    public decimal DestinationLatitude { get; set; }
    public decimal DestinationLongitude { get; set; }

  //  public string StationName { get; set; } // optional search
}


