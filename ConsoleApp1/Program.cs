using System;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

class Program
{

public class StockData
{
    public Dictionary<string, StockEntry> TimeSeries { get; set; }
}

public class StockEntry
{
    public string Name { get; set; }
    public string Open { get; set; }
    public string High { get; set; }
    public string Low { get; set; }
    public string Close { get; set; }
    public string Volume { get; set; }
}

    static void Main()
    {

        string symbol = "INFY"; // Replace this with the stock symbol you want to fetch data for
        string apiKey = "JLDUJANTRZBNTWA8"; // Replace this with your Alpha Vantage API key

        // Construct the API URL
        string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&interval=5min&apikey={apiKey}";

        // Fetch data from the API
        using (WebClient client = new WebClient())
        {
            string json = client.DownloadString(url);

            // Deserialize JSON response
            dynamic data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json);
            StockData stockData = new StockData(){TimeSeries = new Dictionary<string, StockEntry>()};

            // Check if the response contains an error message
            if (data.ContainsKey("Error Message"))
            {
                string errorMessage = data["Error Message"].Values.First();
                Console.WriteLine($"Error: {errorMessage}");
                return;
            }

            // Extract time series data
            var timeSeriesData = data["Time Series (Daily)"].EnumerateObject();
            foreach (var entry in timeSeriesData)
            {
                string time = entry.Name;
                string open = entry.Value.GetProperty("1. open").GetString();
                string high = entry.Value.GetProperty("2. high").GetString();
                string low = entry.Value.GetProperty("3. low").GetString();
                string close = entry.Value.GetProperty("4. close").GetString();
                string volume = entry.Value.GetProperty("5. volume").GetString();

                var stockEntry = new StockEntry
                    {
                        Name = entry.Name,
                        Open = entry.Value.GetProperty("1. open").GetString(),
                        High = entry.Value.GetProperty("2. high").GetString(),
                        Low = entry.Value.GetProperty("3. low").GetString(),
                        Close = entry.Value.GetProperty("4. close").GetString(),
                        Volume = entry.Value.GetProperty("5. volume").GetString()
                    };

                    stockData.TimeSeries.Add(entry.Name,stockEntry);

                Console.WriteLine("Time: " + time + ", Open: " + open + ", High: " + high + ", Low: " + low + ", Close: " + close + ", Volume: " + volume);
            }
        }
    }
}
