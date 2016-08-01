using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.GPX
{
    static class Tracker
    {
        private static readonly StringBuilder _xml;
        private static readonly string _fileName;

        static Tracker()
        {
            _xml = new StringBuilder();
            _xml.Append("<?xml version='1.0' encoding='UTF-8' standalone='no' ?><gpx><trk><name></name><trkseg>");
            _fileName = $"PokemonGo.SimpleBot.trace-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.gpx";
        }

        public static async Task RegisterWayPoint(double latitude, double longitude, double elevation)
        {
            _xml.Append($"<trkpt lat='{latitude}' lon='{longitude}'><ele>{elevation}</ele><time>{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")}</time></trkpt>");
            await FlushToFile();
        }

        public static async Task FlushToFile()
        {
            //File.WriteAllText(_fileName, _xml.ToString()+"</gpx>");
            var encodedText = Encoding.Unicode.GetBytes(_xml.ToString() + "</trkseg></trk></gpx>");
            using (var sourceStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }
}
