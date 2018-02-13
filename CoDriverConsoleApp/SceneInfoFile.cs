using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class SceneInfoFile
    {
        public struct SceneInfoNode
        {
            public int index;
            public string data;

        }
        public CSVFile csv_file;
        public void Load(string filename)
        {
            csv_file = new CSVFile();
            csv_file.Load(filename);
        }
    }
}
