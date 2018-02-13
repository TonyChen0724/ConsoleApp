using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
namespace CoDriverConsoleApp
{
    class KMLFile
    {
        XmlDocument document = new XmlDocument();
        public void LoadXML(string filename)
        {
            XmlReader reader = XmlReader.Create(filename);
            document.Load(reader);
            reader.Close();
            return;
        }
        public void SaveXML(string output_filename)
        {
            //XmlWriter writer = XmlWriter.Create("TestSave.xmp");
            document.Save(output_filename);
        }
        public void SetName(string name)
        {
            XmlNode nameNode = document.ChildNodes[1].ChildNodes[0].ChildNodes[0];
            nameNode.InnerText = name;
        }
        public void SetCoordinates(string coordinates)
        {
            XmlNode placemarkNode = document.ChildNodes[1].ChildNodes[0].ChildNodes[3];
            XmlNode coordinatesNode = placemarkNode.ChildNodes[3].ChildNodes[3];
            coordinatesNode.InnerText = coordinates;
        }
        static public double earthRadiusKm = 6371.0;

        // This function converts decimal degrees to radians
        static double deg2rad(double deg)
        {
            return (deg * 3.1415926 / 180);
        }

        //  This function converts radians to decimal degrees
        static double rad2deg(double rad)
        {
            return (rad * 180 / 3.1415926);
        }

        /**
         * Returns the distance between two points on the Earth.
         * Direct translation from http://en.wikipedia.org/wiki/Haversine_formula
         * @param lat1d Latitude of the first point in degrees
         * @param lon1d Longitude of the first point in degrees
         * @param lat2d Latitude of the second point in degrees
         * @param lon2d Longitude of the second point in degrees
         * @return The distance between the two points in kilometers
         */
        public static double distanceEarth(double lat1d, double lon1d, double lat2d, double lon2d)
        {
            double lat1r, lon1r, lat2r, lon2r, u, v;
            lat1r = deg2rad(lat1d);
            lon1r = deg2rad(lon1d);
            lat2r = deg2rad(lat2d);
            lon2r = deg2rad(lon2d);
            u = Math.Sin((lat2r - lat1r) / 2);
            v = Math.Sin((lon2r - lon1r) / 2);
            return 2.0 * earthRadiusKm * Math.Asin(Math.Sqrt(u * u + Math.Cos(lat1r) * Math.Cos(lat2r) * v * v));
        }
    }
}
