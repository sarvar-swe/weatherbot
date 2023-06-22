using System.Net.Http.Json;

public class WeatherService
{
    private readonly HttpClient openMeteoClient;
    public WeatherService(IHttpClientFactory httpClientFactory)
    {
        this.openMeteoClient = httpClientFactory.CreateClient("OpenMeteoClient");
    }

    public async Task<string> GetWeatherTextAsync(double longitude, double latitude, CancellationToken cancellationToken = default)
    {
        var route = $"/v1/forecast?latitude={latitude.ToString().Replace(",", "."):F5}&longitude={longitude.ToString().Replace(",", "."):F5}&current_weather=true";
        var weather = await this.openMeteoClient.GetFromJsonAsync<WeatherResponse>(route);

        var weatherEmoji = weather.CurrentWeather.Temperature switch
        {
            > 40 => "🥵",
            > 30 => "🔥",
            > 20 => "🌞",
            > 15 => "⛅️",
            > 10 => "⛅️",
            > 5 => "⛅️",
            > 0 => "❄️",
            _ => "🥶",
        };

        return $"Hozirgi ob-havo: {weather.CurrentWeather.Temperature:F1} {weatherEmoji}";
    }
}