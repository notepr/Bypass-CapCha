using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solve_Captcha.Model
{
    class CaiDat
    {
        [JsonProperty("DSAPI")]
        public List<KeyApi> keys { get; set; }
        [JsonProperty("Clipboard")]
        public bool Clipboard { get; set; }
    }
}
