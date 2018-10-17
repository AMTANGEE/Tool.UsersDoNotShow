using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace UsersDoNotShow
{
    public class Global
    {
        private static BitmapImage userImage;
        public static BitmapImage UserImage
        {
            get
            {
                if(userImage == null)
                {
                    userImage = new BitmapImage();
                    userImage.BeginInit();
                    userImage.UriSource = new Uri("pack://application:,,/Resources/User.png");
                    userImage.EndInit();
                }

                return userImage;
            }
        }
        private static BitmapImage userGroupImage;
        public static BitmapImage UserGroupImage
        {
            get
            {
                if (userGroupImage == null)
                {
                    userGroupImage = new BitmapImage();
                    userGroupImage.BeginInit();
                    userGroupImage.UriSource = new Uri("pack://application:,,/Resources/UserGroup.png");
                    userGroupImage.EndInit();
                }

                return userGroupImage;
            }
        }
    }
}
