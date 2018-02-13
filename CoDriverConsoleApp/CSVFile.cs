using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CoDriverConsoleApp
{
    class CSVFile
    {
        public struct CSVLine
        {
            public string[] values;
            public string data;
        }
        public Dictionary<int, CSVLine> m_csv_data = new Dictionary<int, CSVLine>();
        public int number;
        public string[] keywords;
        public string src_filename;
        public void Load(string filename)
        {
            src_filename = filename;
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == '#')
                    continue;
                var values = line.Split(',');
                if (values.Length < 2)
                    continue;
                
                CSVLine line_data = new CSVLine();
                line_data.values = values;
                line_data.data = line;
                if (count == 0)
                    keywords = values;
                
                m_csv_data.Add(count, line_data);
                count++;
            }
            number = m_csv_data.Count - 1;
            reader.Close();
        }
        public void Save()
        {
            string folder = Path.GetDirectoryName(src_filename);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            StreamWriter writer = new StreamWriter(src_filename);
            foreach(var line in m_csv_data)
            {
                writer.WriteLine(line.Value.data);
            }
            writer.Close();
        }
        public void BuildFile(string filename,string head_string)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            src_filename = filename;
            m_csv_data.Clear();
            CSVLine headData = new CSVLine();
            headData.values = head_string.Split(',');
            headData.data = head_string;
            m_csv_data[0] = headData;
            number = 0;
        }
        public void RemoveData()
        {
            if (m_csv_data.Count < 1)
                return;
            CSVLine lineData = m_csv_data[0];
            m_csv_data.Clear();
            m_csv_data[0] = lineData;
            number = 0;
        }
        public void AddLine(string line)
        {
            //CSVLine lineEnd  = m_csv_data[m_csv_data.Count - 1];
            //if (lineEnd.values.Length < 1)
            //    return;
            string idxStr = Convert.ToString(m_csv_data.Count);
            CSVLine new_line = new CSVLine();
            new_line.data = idxStr + line;
            new_line.values = new_line.data.Split(',');
            m_csv_data.Add(m_csv_data.Count,new_line);
            return;
        }

        public void AddLineWithoutIdx(string line)
        {
            
            CSVLine new_line = new CSVLine();
            new_line.data = line;
            new_line.values = new_line.data.Split(',');
            m_csv_data.Add(m_csv_data.Count, new_line);
            return;
        }
    }
}
