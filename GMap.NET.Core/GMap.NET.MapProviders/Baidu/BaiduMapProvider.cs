using GMap.NET.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.MapProviders
{
    public abstract class BaiduMapProviderBase : GMapProvider
    {
        public BaiduMapProviderBase()
        {
            MaxZoom = null;
            RefererUrl = "http://map.baidu.com";
            Copyright = string.Format("©{0} Baidu Corporation, ©{0} NAVTEQ, ©{0} Image courtesy of NASA", DateTime.Today.Year);
        }

        public override PureProjection Projection
        {
            get { return BaiduProjection.Instance; }
        }

        GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }

        
    }

    public class BaiduMapProvider : BaiduMapProviderBase
    {
        public static readonly BaiduMapProvider Instance;

        readonly Guid id = new Guid("608748FC-5FDD-4d3a-9027-356F24A755E5");
        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = Resources.Strings.BaiduMap;
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static BaiduMapProvider()
        {
            Instance = new BaiduMapProvider();
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

            //原来：http://q3.baidu.com/it/u=x=721;y=209;z=12;v=014;type=web&fm=44
            //更新：http://online1.map.bdimg.com/tile/?qt=tile&x=23144&y=6686&z=17&styles=pl

            //     http://online3.map.bdimg.com/onlinelabel/?qt=tile&x=769&y=250&z=12&styles=pl&udt=20150928&scaler=1
                 //http://online1.map.bdimg.com/onlinelabel/?qt=tile&x=774&y=249&z=12&styles=pl&udt=20150928&scaler=1	
               //  http://online1.map.bdimg.com/onlinelabel/?qt=tile&x={0}&y={1}&z={2}&styles=pl&udt=20150928&scaler=1
            string url = string.Format(UrlFormat, x, y, zoom);
            Console.WriteLine("url:" + url);
            return url;
        }


        static readonly string UrlFormat = "http://online1.map.bdimg.com/onlinelabel/?qt=tile&x={0}&y={1}&z={2}&styles=pl&udt=20150928&scaler=1";

    }
}
