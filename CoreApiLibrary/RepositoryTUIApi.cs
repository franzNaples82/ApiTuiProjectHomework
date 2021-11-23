using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApiLibrary
{
    public class RepositoryTUIApi
    {
        private string TuiEndPoint { get; }

        private string WeatherEndPoint { get; }

        private RestClient restClient { get; set; }

        private List<MapCity> cityList = new List<MapCity>();
        private List<MapForecastCity> payloadForecastList = new List<MapForecastCity>();
        private dynamic objResultWeather { get; set; }

        public RepositoryTUIApi()
        {
            TuiEndPoint = HelperUtility.baseUrl;
            WeatherEndPoint = HelperUtility.BaseApiWeatherUrl;
        }

        public List<KeyValuePair<string, object>> GetResponseRequestCityEndPoint()
        {
            List<KeyValuePair<string, object>> _list = new List<KeyValuePair<string, object>>();
            restClient = new RestClient(TuiEndPoint);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Language", "en-US");
            IRestResponse response = restClient.Execute(request);
            List<RequestOutClient> requestOutClients = new List<RequestOutClient>();
            dynamic outObject = JsonConvert.DeserializeObject(response.Content);
            string status = response.StatusCode.ToString();
            Dictionary<string, object> _dictonaryInternal = new Dictionary<string, object>()
            {
                {status,outObject}
            };

            foreach (KeyValuePair<string, object> i in _dictonaryInternal)
            {
                _list.Add(i);
            }

            return _list;
        }

        public IRestResponse GetResponseForecastEndPoint(MapCity cityItem )
        {

            StringBuilder sApi = new StringBuilder();
            sApi.Append("key=" + HelperUtility.ApiKeyWeather);
            sApi.Append("&q=");
            sApi.Append(cityItem.Latitude);
            sApi.Append(",");
            sApi.Append(cityItem.Longitute);
            sApi.Append("&days=2");
            string fullweatherUrl = String.Concat(WeatherEndPoint, sApi);
            var clientWeather = new RestClient(fullweatherUrl.ToString());
            var requestWeather = new RestRequest(Method.GET);
            requestWeather.AddHeader("Accept", "application/json");
            requestWeather.AddHeader("Accept-Language", "en-US");
            return clientWeather.Execute(requestWeather);
        }

        public List<MapCity> GetListPayloadCity(dynamic objectRequestJson)
        {

            foreach (var item in objectRequestJson)
            {
                MapCity city = new MapCity();
                city.Id = item.id;
                city.Name = item.name;
                city.Latitude = item.latitude;
                city.Longitute = item.longitude;
                city.urlMuseeum = item.url;
                IRestResponse responseWeather = GetResponseForecastEndPoint(city);
                objResultWeather = JsonConvert.DeserializeObject(responseWeather.Content);
                foreach (var itemForecast in objResultWeather.forecast.forecastday)
                {
                    MapForecastCity payloadForecast = new MapForecastCity();
                    payloadForecast.name = objResultWeather.location.name;
                    payloadForecast.date = itemForecast.date;
                    payloadForecast.condition =  itemForecast.day.condition.text;
                    payloadForecastList.Add(payloadForecast);
                }
                cityList.Add(city);
            }

            return cityList;
        }

        public List<MapForecastCity> GetResponseListWeather()
        {
            return payloadForecastList;
        }
    }
    public class RequestOutClient
    {
        public string statusCode { get; set; }

        public object outjsonObject { get; set; }

    }
}
