using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace CoDriverConsoleApp
{
    class CoConfig
    {

        public Dictionary<string, string> m_config_data = new Dictionary<string, string>();
        public void Load()
        {            
            StreamReader reader = new StreamReader("Config.ini");
            int count = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == '#')
                    continue;
                if (line[0] == '[')
                    continue;
                var values = line.Split('=');
                if (values.Length < 2)
                    continue;
                
                m_config_data.Add(values[0], values[1]);
                count++;
            }
            reader.Close();
        }
        public string GetValue(string key)
        {
            string data = "";
            bool isContain = m_config_data.ContainsKey(key);
            if (isContain)
                data = m_config_data[key];
            return data;
        }
    }
}
