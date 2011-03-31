using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace WindowsDock.Core
{
    public class IconHelper
    {
        public static ImageSource GetIcon(string filename)
        {
            System.Drawing.Icon extractedIcon = System.Drawing.Icon.ExtractAssociatedIcon(filename);
            ImageSource imgs;

            using (System.Drawing.Icon i = System.Drawing.Icon.FromHandle(extractedIcon.ToBitmap().GetHicon()))
            {
                imgs = Imaging.CreateBitmapSourceFromHIcon(i.Handle, new Int32Rect(0, 0, 32, 32), BitmapSizeOptions.FromEmptyOptions());
            }

            return imgs;
        }
    }
}
