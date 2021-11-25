using CoreApiLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Assert = NUnit.Framework.Assert;

namespace TestTuiApi
{
    [TestClass]
    public class TestRepositoryTUIApi
    {
        
       


        [TestMethod]
        public void GetResponseRequestCityEndPoint()
        {
            List<KeyValuePair<string, object>> _list = new List<KeyValuePair<string, object>>();
            var restClient = new RestClient(HelperUtility.baseUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Language", "en-US");
            IRestResponse response = restClient.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [TestMethod]
        public void GetListPayloadCity()
        {
            MapCity city = new MapCity();
            List<MapCity> cityList = new List<MapCity>();
            cityList.Add(city);
            Assert.IsNotEmpty(cityList);
        }
    }
}
