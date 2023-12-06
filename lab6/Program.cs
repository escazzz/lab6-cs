using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

    struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }

        public void Print()
        {
            Console.WriteLine($"Country: {Country}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Temp: {Temp-273}");
            Console.WriteLine($"Description: {Description}");
        }
    }
    internal class Lab6
    {
        static void Main(string[] args)
        {
            List<Weather> list = FetchWeatherList(50);

            foreach (var weather in list)
            {
                weather.Print();
            }

            PrintInfo(list);
            PrintUniqueCountries(list);
    }
        private static readonly string API_KEY = "5e9529dfc67ed27293e38ca71793f3c4";


        private static List<Weather> FetchWeatherList(int num)
        {
            List<Weather> weatherList = new List<Weather>();
            Random random = new Random();

            while (weatherList.Count !=30)
            {
                double latitude = random.NextDouble() * 180 - 90;
                double longitude = random.NextDouble() * 360 - 180;
                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={API_KEY}";

                string jsonData = FetchData(apiUrl);
                JObject json = JObject.Parse(jsonData);

                if (json["sys"] != null && json["sys"]["country"] != null)
                {
                    string country = json["sys"]["country"].ToString();
                    string name = json["name"].ToString();
                    double temp = Convert.ToDouble(json["main"]["temp"]);
                    string description = json["weather"][0]["description"].ToString();

                    weatherList.Add(new Weather
                    {
                        Country = country,
                        Name = name,
                        Temp = temp,
                        Description = description
                    });
                }
                /*else
                {
                    Console.WriteLine("We can not find country lat={0}, long={1}", latitude, longitude);
                }*/
            }

            return weatherList;
        }

        private static string FetchData(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new Exception($"HTTP error: {response.StatusCode}");
                }
            }
        }

        private static void PrintInfo(List<Weather> list)
        {
            var maxTempCountry = list.OrderByDescending(x => x.Temp).FirstOrDefault();
            Console.WriteLine($"Country with max temp: {maxTempCountry.Country}");


            var minTempCountry = list.OrderBy(x => x.Temp).FirstOrDefault();
            Console.WriteLine($"Country with min temp: {minTempCountry.Country}");


            var averageTemp = list.Average(w => w.Temp);
            Console.WriteLine($"Average temp: {averageTemp-273:F2} °C");


            var countryCount = list.Select(w => w.Country).Distinct().Count();
            Console.WriteLine($"Count of countries: {countryCount}");


            var locationWithDescription = list
                .Where(w => w.Description == "clear sky" || w.Description == "rain" || w.Description == "few clouds")
                .Select(w => new { w.Country, w.Name })
                .FirstOrDefault();
            Console.WriteLine($"First country and location with the description 'clear sky', 'rain', or 'few clouds': {locationWithDescription?.Country}, {locationWithDescription?.Name}");

        }
    private static void PrintUniqueCountries(List<Weather> list)
    {
        var uniqueCountries = list.Select(w => w.Country).Distinct();
        Console.WriteLine("Unique countries in the list:");
        foreach (var country in uniqueCountries)
        {
            Console.WriteLine(country);
        }
    }

}

