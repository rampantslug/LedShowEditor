using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LedShowEditor.Config
{
    /// <summary>
    /// This class gets serialised to json
    /// </summary>
    public class ShowConfig
    {
        public uint Frames { get; set; }
        public IList<LedInShowConfig> Leds { get; set; }

        public ShowConfig()
        {
            Leds = new List<LedInShowConfig>();
        }

        public static ShowConfig FromJson(string json)
        {
            var showConfiguration = JsonConvert.DeserializeObject<ShowConfig>(json);
            return showConfiguration;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static ShowConfig FromFile(string pathToFile)
        {
            var streamReader = new StreamReader(pathToFile);
            var text = streamReader.ReadToEnd();
            streamReader.Close();
            return FromJson(text);
        }


        public void ToFile(string filename)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            using (var sw = new StreamWriter(filename))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }
    }
}
