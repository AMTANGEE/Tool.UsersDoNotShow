using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMTANGEE.Tools.UsersDoNotShow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<UserGroupObject> UsersGroupsObjects;

        public MainWindow()
        {
            InitializeComponent();

            var con = new AMTANGEE.Custom.ConnectionForm(true);

            if (con.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            try
            {
                AMTANGEE.DB.Open(con.ConnectionString);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Fehler beim Verbinden!\r\nSiehe Log für mehr Informationen.", 
                    "SQL Fehler", 
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            var users = new AMTANGEE.SDK.Users.Users(AMTANGEE.SDK.Global.OwnLocation, true, false, true);

            CbUser.ItemsSource = users;

            UsersGroupsObjects = new ObservableCollection<UserGroupObject>();

            foreach (var group in new AMTANGEE.SDK.Users.Groups())
                UsersGroupsObjects.Add(new UserGroupObject()
                {
                    IsUser = false,
                    GUID = group.Guid,
                    DisplayName = group.Name
                });
            foreach (var user in users)
                UsersGroupsObjects.Add(new UserGroupObject()
                {
                    IsUser = true,
                    GUID = user.Guid,
                    DisplayName = user.DisplayName
                });
        }

        private void CbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var cur in UsersGroupsObjects)
                cur.IsShown = true;

            var user = (AMTANGEE.SDK.Users.User)CbUser.SelectedItem;
            
            var ds = AMTANGEE.DB.Select("select USERGROUP from UsersDoNotShow where [USER] = '" + user.Guid + "' and LOCATION = '" + AMTANGEE.SDK.Global.OwnLocation.Guid + "'");

            if (ds != null)
                foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                    try {
                        UsersGroupsObjects.First(x => x.GUID == (Guid)row[0]).IsShown = false;
                    } catch { }

            DgUsersGroups.ItemsSource = null;
            DgUsersGroups.ItemsSource = UsersGroupsObjects;
        }
        
        private void BtnApplyToAll_Click(object sender, RoutedEventArgs e)
        {
            if(System.Windows.Forms.DialogResult.No == 
                System.Windows.Forms.MessageBox.Show("Wollen Sie die Änderung wirklich für alle Benutzer übernehmen?", 
                "Sind Sie sich sicher?", 
                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question))
                return;

            List<Guid> usersNotToShow = new List<Guid>();

            foreach (UserGroupObject item in DgUsersGroups.Items)
                if (!item.IsShown)
                    usersNotToShow.Add(item.GUID);

            ShowAllUser();

            foreach (var user in new AMTANGEE.SDK.Users.Users())
            {
                foreach (var cur in usersNotToShow)
                    HideUser(user.Guid, cur);

                AMTANGEE.SDK.Events.SendSDKEvent("REFRESHUSERSDONOTSHOW:{" + user.Guid.ToString().ToUpper() + "}");
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            List<Guid> usersNotToShow = new List<Guid>();

            foreach (UserGroupObject item in DgUsersGroups.Items)
                if (!item.IsShown)
                    usersNotToShow.Add(item.GUID);

            var user = (AMTANGEE.SDK.Users.User)CbUser.SelectedItem;
            
            ShowAllUser(user.Guid, false);

            foreach (var cur in usersNotToShow)
                HideUser(user.Guid, cur);
            
            AMTANGEE.SDK.Events.SendSDKEvent("REFRESHUSERSDONOTSHOW:{" + user.Guid.ToString().ToUpper() + "}");
        }

        public void HideUser(Guid userGuid, Guid userNotToShow)
        {
            AMTANGEE.DB.Exec("insert into UsersDoNotShow ([USER],USERGROUP,LOCATION) " +
                "values ('" + userGuid + "','" + userNotToShow + "','" + AMTANGEE.SDK.Global.OwnLocation.Guid + "')");
        }

        public void ShowAllUser(Guid userGuid = new Guid(), bool sendSdkEvent = true)
        {
            if(userGuid == new Guid())
            {
                AMTANGEE.DB.Exec("delete from UsersDoNotShow where LOCATION = '" + AMTANGEE.SDK.Global.OwnLocation.Guid + "'");
                if(sendSdkEvent)
                    foreach (var user in new AMTANGEE.SDK.Users.Users())
                        AMTANGEE.SDK.Events.SendSDKEvent("REFRESHUSERSDONOTSHOW:{" + user.Guid.ToString().ToUpper() + "}");
            }
            else
            {
                AMTANGEE.DB.Exec("delete from UsersDoNotShow where LOCATION = '" + AMTANGEE.SDK.Global.OwnLocation.Guid + "' and [USER] = '" + userGuid + "'");
                if(sendSdkEvent)
                    AMTANGEE.SDK.Events.SendSDKEvent("REFRESHUSERSDONOTSHOW:{" + userGuid.ToString().ToUpper() + "}");
            }
        }
    }
}
