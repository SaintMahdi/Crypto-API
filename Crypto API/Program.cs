using Newtonsoft.Json;

HttpClient httpClient = new HttpClient();
string stringAPI = "https://api.wallex.ir/v1/currencies/stats";

await RunInBackground(TimeSpan.FromSeconds(10), () => InitAsync());

async Task RunInBackground(TimeSpan timeSpan, Action action)
{
    var periodicTimer = new PeriodicTimer(timeSpan);
    do { action(); }
    while (await periodicTimer.WaitForNextTickAsync());
    {
        action();
    }
}



async Task InitAsync()
{
    HttpResponseMessage response = await httpClient.GetAsync(stringAPI);
    if (response.IsSuccessStatusCode)
    {
        string apiresponse = await response.Content.ReadAsStringAsync();
        ApiResponseWrapper apiWrapper = JsonConvert.DeserializeObject<ApiResponseWrapper>(apiresponse);
        List<ResultItem> ResultItems = apiWrapper.Result;
        foreach (var item in ResultItems)
        {
            Console.WriteLine($"Key: {item.key}");
            Console.WriteLine($"Name: {item.name_en}");
            Console.WriteLine($"Price: {item.price}");
            Console.WriteLine(item.prediction());
            Console.WriteLine(item.prediction(9));
            Console.WriteLine();
        }
    }
}
Console.ReadLine();




public class ApiResponseWrapper
{
    public List<ResultItem> Result { get; set; }
}


public class ResultItem
{
    public string key { get; set; }
    public string name_en { get; set; }
    public double price { get; set; }

    public double? percent_change_1h { get; set; }

    public string prediction()
    {
        double newPrice = 0;
        newPrice = price + (price * Convert.ToDouble(percent_change_1h) / 100);
        string output = $"Possible price in 1 hour is: {newPrice}";
        return output;
    }

    public string prediction(int hour)
    {
        double? newPrice = 0;
        newPrice = (price + (price * hour * percent_change_1h / 100));
        string output = $"Possible price in {hour} hours is: {newPrice}";
        return output;
    }
}
