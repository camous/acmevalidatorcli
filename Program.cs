using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace acmevalidatorcli
{
    class Program
    {
        static void Main(string[] args)
        {           
            var inputjson = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
            var input = JObject.Parse(inputjson);

            var rulesjson = Encoding.UTF8.GetString(Convert.FromBase64String(args[1]));
            var rules = JObject.Parse(rulesjson);

            bool ignoreRequired = false;
            if (args.Length == 3)
            {
                if(bool.TryParse(args[2], out ignoreRequired))
                {}
            }

            var valid = new acmevalidator.Validator().Validate(input, rules, out Dictionary<JToken, JToken> delta, ignoreRequired);
            Console.WriteLine(valid);

            var arraydelta = new JArray();
            foreach (var d in delta)
                arraydelta.Add(new JObject { { "expected", d.Key }, { "actual", d.Value ?? null } });

            Console.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(arraydelta))));
        }
    }
}
