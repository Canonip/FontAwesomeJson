using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FontAwesomeJson
{
    class Program
    {
        const string YAML_URI = "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/master/advanced-options/metadata/icons.yml";
        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
                Console.ReadLine();
                throw;
            }

        }
        static async Task MainAsync(string[] args)
        {
            var info = @"
This Tool will take FontAwesomes YAML File and will create a JSON file, you can use as a Dictionary if you want to use FA in a desktop Application

the produced JSON File will have this format:

{
""iconName"" :
    {
    ""Id"" : ""icon"",
    ""Styles"" : [""style1"", ""style2""]
    }  
}

Where the styles can be ""regular"", ""solid"" or ""brands"".

Press Enter to continue";
            Console.WriteLine(info);
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Loading Fontawesome YAML File");
            var yamlString = await LoadFontAwesomeYamlFile();
            var deserializer = new DeserializerBuilder().Build();
            var yaml = deserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(new StringReader(yamlString));
            Console.WriteLine("Loaded Fontawesome YAML File");
            var dictionary = yaml.Select(x => KeyValuePair.Create(x.Key, new FA()
            {
                Id = GetFontAwesomeChar(x.Value["unicode"] as string),
                Styles = (x.Value["styles"] as ICollection<object>).Select(i => (string)i).ToList()
            }
            )).ToDictionary(x => x.Key, x => x.Value);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
            Console.WriteLine($"Done! JSON File contains {dictionary.Count} Icons.");
            Console.WriteLine("enter path to save file");
            var path = Console.ReadLine();
            await File.WriteAllTextAsync(path, json);
            Console.WriteLine($"Saved json file at {path}");
            Console.WriteLine("Press Any Key to exit");
            Console.ReadKey();

        }

        public static string GetFontAwesomeChar(string hex)
        {
            var bytes = StringToByteArray(hex);
            return ByteToString(bytes);
        }

        public static string ByteToString(byte[] array)
        {
            var enc = Encoding.Unicode;
            var chars = enc.GetChars(array);
            return new string(chars);
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        static async Task<string> LoadFontAwesomeYamlFile()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(YAML_URI);
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception("Could not download Yaml file: " + result.StatusCode);
                }
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
    class FA
    {
        public string Id { get; set; }
        public ICollection<string> Styles { get; set; }
    }
}
