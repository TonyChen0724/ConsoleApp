using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class CSVGPSData
    {
        public struct CSVData
        {
            public double timestamp;
            public DateTime dt;
            public double time;
            public double v1;
            public double v2;
            public double v3;
        }
        public Dictionary<int, CSVData> m_csv_data = new Dictionary<int, CSVData>();
        public int number;
        public double start_tm;
        public double total_tm;
        public void Load(string filename)
        {
            bool is_first = true;
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
                if (values.Length < 3)
                    continue;
                if (values[0] == "Time")
                    continue;
                CSVData data = new CSVData();
                data.timestamp= Convert.ToDouble(values[0]);
                if (is_first)
                {
                    start_tm = data.timestamp;
                    is_first = false;
                }
                data.time = data.timestamp - start_tm;
                data.v1 = Convert.ToDouble(values[1]);
                data.v2 = Convert.ToDouble(values[2]);
                data.v3 = Convert.ToDouble(values[3]);
                count++;
                total_tm = data.time;
                m_csv_data.Add(count, data);
            }
            number = m_csv_data.Count;
        }

        public int FindData(double tm)
        {
            double dMin = 10.0;
            int findIdx = -1;
            foreach (var node in m_csv_data)
            {
                double temp = node.Value.time - tm;
                temp = Math.Abs(temp);
                if(temp < dMin)
                {
                    dMin = temp;
                    findIdx = node.Key;
                }
            }
            return findIdx;
        }

        public CSVData GetData(int idx)
        {
            return m_csv_data[idx];
        }

    }
}
