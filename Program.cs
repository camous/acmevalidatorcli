using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace acmevalidatorcli
{
    class Program
    {
        public class Options
        {
            [Option('i', "input", Required = true, HelpText ="input json in base64")]
            public string input { get; set; }
            [Option('r', "rules", Required = true, HelpText = "rules json in base64")]
            public string rules { get; set; }

            [Option('f', "ignorerequired", Required = false, HelpText = "ignore $required / $requiredOrNull", Default =false)]
            public bool ignoreRequired { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(o =>
               {
                   var inputjson = Encoding.UTF8.GetString(Convert.FromBase64String(o.input));
                   var input = JObject.Parse(inputjson);

                   var rulesjson = Encoding.UTF8.GetString(Convert.FromBase64String(o.rules));
                   var rules = JObject.Parse(rulesjson);

                   var valid = new acmevalidator.Validator().Validate(input, rules, out Dictionary<JToken, JToken> delta, o.ignoreRequired);
                   Console.WriteLine(valid);

                   var arraydelta = new JArray();
                   foreach (var d in delta)
                       arraydelta.Add(new JObject { { "expected", d.Key }, { "actual", d.Value ?? null } });

                   Console.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(arraydelta))));
               });
        }
    }
}
