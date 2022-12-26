using Newtonsoft.Json;

namespace Solve_Captcha.Model
{
    public class KeyApi
    {
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Key")]
        public string Key { get; set; }
        [JsonProperty("IsSelect")]
        public bool IsSelect { get; set; }

        public string Amount { get; set; }
        public override string ToString()
        {
            return "ID = " + this.ID + " | API= " + this.Key + " | IsSelect = " + this.IsSelect;
        }
    }
}
