
namespace GMap.NET
{
   /// <summary>
   /// 枚举 鼠标放大的类型
   /// </summary>
   public enum MouseWheelZoomType
   {
      /// <summary>
       /// 放大地图当前鼠标的位置，并使其地图中心
      /// </summary>
      MousePositionAndCenter,

      /// <summary>
      /// 放大到当前鼠标的位置，但不会将它映射中心，谷歌/冰风格
      /// zooms to current mouse position, but doesn't make it map center,
      /// google/bing style ;}
      /// </summary>
      MousePositionWithoutCenter,

      /// <summary>
      /// 放大地图当前视图中心
      /// </summary>
      ViewCenter,        
   }
}
