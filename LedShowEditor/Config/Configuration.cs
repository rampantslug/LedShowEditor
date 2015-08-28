using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace LedShowEditor.Config
{
    /// <summary>
    /// Contains the required configuration information.
    /// </summary>
    public class Configuration
    {
        public string PlayfieldImage { get; set; }
        public IList<LedConfig> Leds { get; set; }
        public IList<GroupConfig> Groups { get; set; }

        /// <summary>
        /// Creates a new Configuration object and initializes all subconfiguration objects
        /// </summary>
        public Configuration()
        {
            Leds = new List<LedConfig>();
        }

        /// <summary>
        /// Initialize configuration from a string of Json code
        /// </summary>
        /// <param name="json">Json serialized Configuration data</param>
        /// <returns>A deserialized Configuration object</returns>
        public static Configuration FromJson(string json)
        {
            var configuration = JsonConvert.DeserializeObject<Configuration>(json);
            return configuration;
        }

        /// <summary>
        /// Convert the entire Configuration to Json code
        /// </summary>
        /// <returns>Pretty formatted Json code</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Initialize configuration from a Json file on disk
        /// </summary>
        /// <param name="PathToFile">The file to deserialize</param>
        /// <returns>A MachineConfiguration object deserialized from the specified Json file</returns>
        public static Configuration FromFile(string PathToFile)
        {
            var streamReader = new StreamReader(PathToFile);
            var text = streamReader.ReadToEnd();
            streamReader.Close();
            return FromJson(text);
        }

        /// <summary>
        /// Convert the entire Configuration to Json code and save to a file
        /// </summary>
        /// <param name="filename">The filename to save to</param>
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


        /// <summary>
        /// Convert the entire Configuration to Xml code and save to a file
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void SaveAsXml(string filename)
        {
            var serializer = new XmlSerializer(typeof(Configuration));
            var textWriter = new StreamWriter(filename, false);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }
    }



   
}
