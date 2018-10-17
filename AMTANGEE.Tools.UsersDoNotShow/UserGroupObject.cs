using System;

namespace UsersDoNotShow
{
    public class UserGroupObject
    {
        public bool IsUser { get; set; }
        public bool IsShown { get; set; }
        public System.Windows.Media.Imaging.BitmapImage UserGroupIcon => IsUser ? Global.UserImage : Global.UserGroupImage;
        public Guid GUID { get; set; }
        public string DisplayName { get; set; }
    }
}
