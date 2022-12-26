using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solve_Captcha.Model
{
    class CapChaHelper
    {
        public static string GuiCapCha(string key, string capcha)
        {
            var client = new RestClient("https://2captcha.com/in.php");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"method\": \"base64\",\"json\":1,\"key\": \"" + key + "\",\"body\": \"" + capcha + "\"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            try
            {
                if (result.Contains("\"status\":1,\"request\":"))
                {
                    NhanVe nv = JsonConvert.DeserializeObject<NhanVe>(response.Content);
                    return nv.Request;
                }
                else
                {
                    return "VGDVCHGT-Error-Send";
                }
            }
            catch
            {
                return "VGDVCHGT-Error-Send";
            }
        }
        public static string NhanKetQua(string key, string id)
        {
            var client = new RestClient("http://2captcha.com/res.php?key=" + key + "&id=" + id + "&json=1&action=get");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            try
            {
                if (result.Contains("\"status\":1,\"request\":"))
                {
                    NhanVe nv = JsonConvert.DeserializeObject<NhanVe>(response.Content);
                    return nv.Request;
                }
                else
                {
                    return "VGDVCHGT-Error-GET";
                }
            }
            catch
            {
                return "VGDVCHGT-Error-GET";
            }
        }
        public static string CheckAmount(string key)
        {
            try
            {
                var client = new RestClient("https://2captcha.com/res.php?key=" + key + "&action=getbalance");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                return String.Format("{0:0.00}", Convert.ToDouble(response.Content)) + "$";
            }
            catch
            {

            }
            return "Error";
        }
    }
    public class NhanVe
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }
    }
}