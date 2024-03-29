﻿
namespace GMap.NET
{
   using System;
   using System.IO;

   /// <summary>
    /// 图像抽象代理
   /// </summary>
   public abstract class PureImageProxy
   {
      abstract public PureImage FromStream(Stream stream);
      abstract public bool Save(Stream stream, PureImage image);

      public PureImage FromArray(byte[] data)
      {
         MemoryStream m = new MemoryStream(data, 0, data.Length, false, true);
         var pi = FromStream(m);
         if(pi != null)
         {
            m.Position = 0;
            pi.Data = m;
         }
         else
         {
            m.Dispose();
         }
         m = null;

         return pi;
      }
   }

   /// <summary>
   /// 图像抽象
   /// </summary>
   public abstract class PureImage : IDisposable
   {
      public MemoryStream Data;

      internal bool IsParent;
      internal long Ix;
      internal long Xoff;
      internal long Yoff;

      #region IDisposable Members

      public abstract void Dispose();

      #endregion
   }
}
