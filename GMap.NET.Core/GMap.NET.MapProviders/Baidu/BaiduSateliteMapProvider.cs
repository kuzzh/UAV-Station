using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.MapProviders
{
    public class BaiduSateliteMapProvider : BaiduMapProviderBase
    {
        public static readonly BaiduSateliteMapProvider Instance;

        readonly Guid id = new Guid("89A10DFA-2557-431a-9656-20064E8D1342");
        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = Resources.Strings.BaiduSateliteMap;
        public override string Name
        {
            get { return name; }
        }


        static BaiduSateliteMapProvider()
        {
            Instance = new BaiduSateliteMapProvider();
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            zoom = zoom - 1;
            var offsetX = Math.Pow(2, zoom);
            var offsetY = offsetX - 1;

            var numX = pos.X - offsetX;
            var numY = -pos.Y + offsetY;

            zoom = zoom + 1;
            var num = (pos.X + pos.Y) % 8 + 1;
            var x = numX.ToString().Replace("-", "M");
            var y = numY.ToString().Replace("-", "M");
           
            //http://shangetu0.map.bdimg.com/it/u=x=6171;y=2001;z=15;v=009;type=sate&fm=46&udt=20150601
            
            string url = string.Format(UrlFormat,  x, y, zoom, "009", "sate", "46");
            Console.WriteLine("url:" + url);
            return url;
        }

        static readonly string UrlFormat = "http://shangetu0.map.bdimg.com/it/u=x={0};y={1};z={2};v={3};type={4}&fm={5}&udt=20150601";
    }
}
