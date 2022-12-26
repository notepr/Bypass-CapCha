using Newtonsoft.Json;
using RestSharp;
using System;

namespace Solve_Captcha.Model
{
    class AnyCapChaHelper
    {
        public static string GuiCapCha(string key, string capcha)
        {
            capcha = capcha.Replace("data:image/jpeg;base64,", "");
            //only base64
            var client = new RestClient("https://api.anycaptcha.com/createTask");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"clientKey\": \"" + key + "\",\"task\": {\"type\": \"ImageToTextTask\",\"body\": \"" + capcha + "\"}}";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            try
            {
                if (result.Contains("{\"errorId\":0,\"errorCode\":\"SUCCESS\""))
                {
                    AnyCapResponeSend nv = JsonConvert.DeserializeObject<AnyCapResponeSend>(response.Content);
                    return nv.TaskId.ToString();
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
            var client = new RestClient("https://api.anycaptcha.com/getTaskResult");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"clientKey\": \"" + key + "\",\"taskId\": \"" + id + "\"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            try
            {
                if (result.Contains("{\"errorId\":0,\"errorCode\":\"SUCCESS\",\"status\":\"ready\","))
                {
                    AnyResponeResult nv = JsonConvert.DeserializeObject<AnyResponeResult>(response.Content);
                    return nv.Solution.Text;
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
                var client = new RestClient("https://api.anycaptcha.com/getBalance");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = "{\"clientKey\": \"" + key + "\"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                string result = response.Content;
                if (result.Contains("{\"errorId\":0,\"errorCode\":\"SUCCESS\""))
                {
                    CheckKey checkKey = JsonConvert.DeserializeObject<CheckKey>(response.Content);
                    if (checkKey.Balance > 0)
                    {
                        return String.Format("{0:0.00}", checkKey.Balance) + "$";
                        //return checkKey.Balance;
                    }
                }
                return "Error";
            }
            catch
            {

            }
            return "Error";
        }
    }
    public class CheckKey
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }
    }
    public class AnyCapResponeSend
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }
    }
    public class Solution
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class AnyResponeResult
    {
        [JsonProperty("errorId")]
        public int ErrorId { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("solution")]
        public Solution Solution { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("ip")]
        public object Ip { get; set; }

        [JsonProperty("createTime")]
        public double CreateTime { get; set; }

        [JsonProperty("endTime")]
        public double EndTime { get; set; }

        [JsonProperty("solveCount")]
        public object SolveCount { get; set; }
    }

}
