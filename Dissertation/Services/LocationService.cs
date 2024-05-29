using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Dissertation.Services
{
    public interface ILocationService
    {
        Task<(double? lat, double? lon)> PostcodeToCoordinates(string postalCode);
        Task<string?> CoordinatesToPostcode(double lat, double lon);
        bool WithinRadius(double lat1, double lon1, double lat2, double lon2, int distance);
    }

    public class LocationService : ILocationService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const double EarthRadiusMiles = 3958.8;

        static LocationService()
        {
            // Add a default User-Agent header to the HttpClient
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyAppName/1.0");
        }

        public async Task<(double? lat, double? lon)> PostcodeToCoordinates(string postalCode)
        {
            var url = $"https://nominatim.openstreetmap.org/search?format=jsonv2&postalcode={postalCode}";
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws if the status code is not successful
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(responseString);

                if (data.Count > 0)
                {
                    var location = data[0];
                    var lat = (double?)location.Value<double?>("lat");
                    var lon = (double?)location.Value<double?>("lon");
                    Console.WriteLine($"Latitude: {lat}, Longitude: {lon}");
                    return (lat, lon);
                }
                else
                {
                    Console.WriteLine("Location not found");
                    return (null, null);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error fetching geocode data: {ex.Message}");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                return (null, null);
            }
        }

        public async Task<string?> CoordinatesToPostcode(double lat, double lon)
        {
            var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}";
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws if the status code is not successful
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(responseString);

                var address = data["address"];
                var postalCode = address?["postcode"]?.ToString();

                if (!string.IsNullOrEmpty(postalCode))
                {
                    Console.WriteLine($"Postal Code: {postalCode}");
                    return postalCode;
                }
                else
                {
                    Console.WriteLine("Postal code not found");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Error fetching reverse geocode data: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                return null;
            }
        }

        public bool WithinRadius(double lat1, double lon1, double lat2, double lon2, int distance)
        {
            double dLat = (lat2 - lat1) * (Math.PI / 180);
            double dLon = (lon2 - lon1) * (Math.PI / 180);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double calculatedDistance = EarthRadiusMiles * c;

            return calculatedDistance <= distance;
        }

        // Method to convert degrees to radians
        /*public static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }*/
    }
}
