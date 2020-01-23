using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Resources;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ClashLand.Core;
using ClashLand.Core.Networking;
using ClashLand.Packets.Messages.Server;
using System.Timers;
using ClashLand.Logic;
using ClashLand.Logic.Enums;
using ClashLand.Extensions;
using ClashLand.Logic.Structure.Slots.Items;

namespace ClashLandGUI
{
    public partial class ClashLandGUI2 : MaterialForm
    {
        public ClashLandGUI2()
        {
            InitializeComponent();
            var sm = MaterialSkinManager.Instance;
            sm.AddFormToManage(this);
            sm.Theme = MaterialSkinManager.Themes.DARK;
            sm.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue900, Primary.Grey500, Accent.Blue200, TextShade.WHITE);
        }

        public System.Timers.Timer T = new System.Timers.Timer();

        public int Count = 0;

        internal Device Device { get; set; }


        private void ClashLandGUI2_Load(object sender, EventArgs e)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            labelIP.Text = Convert.ToString(ipHostInfo.AddressList[0]);
            labelOnlinePlayers.Text = Convert.ToString(Gateway.NumberOfBuffersInUse.ToString());
            labelConnectedPlayers.Text = Utils.Padding(Players.Levels.Count.ToString(), 15);
            labelMemoryPlayers.Text = Utils.Padding(Players.Levels.Count.ToString(), 15);
            materialLabel14.Text = Convert.ToString(Resources.Clans.Seed.ToString());
            //materialLabel15.Text = Utils.Padding(ClashLand.Core.Resources.Clans.Seed.ToString());
            materialLabel16.Text = Utils.Padding(Players.Levels.Count.ToString(), 15);


            // CONFIG EDITOR
            //txtStartingGems.Text = ConfigurationManager.AppSettings["startingGems"];
            //txtStartingGold.Text = ConfigurationManager.AppSettings["startingGold"];
            //txtStartingElixir.Text = ConfigurationManager.AppSettings["startingElixir"];
            //txtStartingDarkElixir.Text = ConfigurationManager.AppSettings["startingDarkElixir"];
            //txtStartingTrophies.Text = ConfigurationManager.AppSettings["startingTrophies"];
            //txtStartingLevel.Text = ConfigurationManager.AppSettings["startingLevel"];
            txtUpdateURL.Text = ConfigurationManager.AppSettings["UpdateUrl"];
            //txtUsePatch.Text = ConfigurationManager.AppSettings["useCustomPatch"];
            txtPatchURL.Text = ConfigurationManager.AppSettings["PatchUrl"];
            txtDatabaseType.Text = ConfigurationManager.AppSettings["MysqlDatabase"];
            //txtPort.Text = ConfigurationManager.AppSettings["ServerPort"];
            //txtAdminMessage.Text = ConfigurationManager.AppSettings["AdminMessage"];
            //txtLogLevel.Text = ConfigurationManager.AppSettings["LogLevel"];
            txtClientVersion.Text = ConfigurationManager.AppSettings["ClientVersion"];

            //PLAYER MANAGER
            /*txtPlayerName.Enabled = false;
            txtPlayerScore.Enabled = false;
            txtPlayerGems.Enabled = false;
            txtTownHallLevel.Enabled = false;
            txtAllianceID.Enabled = false;
            txtPlayerLevel.Enabled = false;

            listView1.Items.Clear();
            int count = 0;
            foreach (var acc in ResourcesManager.m_vOnlinePlayers)
            {
                ListViewItem item = new ListViewItem(acc.Avatar.AvatarName);
                item.SubItems.Add(Convert.ToString(acc.Avatar.UserId));
                item.SubItems.Add(Convert.ToString(acc.Avatar.m_vAvatarLevel));
                item.SubItems.Add(Convert.ToString(acc.Avatar.GetScore()));
                item.SubItems.Add(Convert.ToString(acc.Avatar.AccountPrivileges));
                listView1.Items.Add(item);
                count++;
                if (count >= 100)
                {
                    break;
                }
            }*/

            //listView2.Items.Clear();
            //int count2 = 0;
            //foreach (var alliance in ResourcesManager.GetInMemoryAlliances())
            {
                //ListViewItem item = new ListViewItem(alliance.m_vAllianceName);
                //item.SubItems.Add(alliance.m_vAllianceId.ToString());
                //item.SubItems.Add(alliance.m_vAllianceLevel.ToString());
                //item.SubItems.Add(alliance.GetAllianceMembers().Count.ToString());
                //item.SubItems.Add(alliance.m_vScore.ToString());
                //listView2.Items.Add(item);
                //count2++;

                //if (count2 >= 100)
                {
                    //break;
                }
            }
        }

        /* MAIN TAB */

        //Restart Button 
        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
                System.Diagnostics.Process.Start("CLS.bat");
        }

            //Shutdown ClashLand Button
            private void materialRaisedButton12_Click(Object sender, EventArgs e)
            {
                foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                {
                    if (myProc.ProcessName == "CLS_v2018")
                    {
                        myProc.Kill();
                    }
                }

            }

            //Close Button
            private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            Close();
        }

        /* END MAIN TAB */

        /*CONFIG EDITOR TAB*/

        //Reset Button
        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            txtUpdateURL.Text = "http://clashology.ddns.net";
            txtPatchURL.Text = "";
            txtDatabaseType.Text = "mysql";
            txtAdminMessage.Text = "Welcome to ClashLand! Visit http://clashology.ddns.net for more info.";
            txtClientVersion.Text = "9.256/10.134 Mods";
        }

        //Save Changes Button
        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            var doc = new XmlDocument();
            var path = "config.cr";
            doc.Load(path);
            var ie = doc.SelectNodes("appSettings/add").GetEnumerator();

            while (ie.MoveNext())
            {
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingGems")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingGems.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingGold")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingGold.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingElixir")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingElixir.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingDarkElixir")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingDarkElixir.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingTrophies")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingTrophies.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "startingLevel")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtStartingLevel.Text;
                }
                if ((ie.Current as XmlNode).Attributes["key"].Value == "UpdateUrl")
                {
                    (ie.Current as XmlNode).Attributes["value"].Value = txtUpdateURL.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "useCustomPatch")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtUsePatch.Text;
                }
                if ((ie.Current as XmlNode).Attributes["key"].Value == "PatchUrl")
                {
                    (ie.Current as XmlNode).Attributes["value"].Value = txtPatchURL.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "maintenanceTimeleft")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtMintenance.Text;
                }
                if ((ie.Current as XmlNode).Attributes["key"].Value == "MysqlDatabase")
                {
                    (ie.Current as XmlNode).Attributes["value"].Value = txtDatabaseType.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "ServerPort")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtPort.Text;
                }
                if ((ie.Current as XmlNode).Attributes["key"].Value == "AdminMessage")
                {
                    (ie.Current as XmlNode).Attributes["value"].Value = txtAdminMessage.Text;
                }
                //if ((ie.Current as XmlNode).Attributes["key"].Value == "LogLevel")
                {
                    //(ie.Current as XmlNode).Attributes["value"].Value = txtLogLevel.Text;
                }
                if ((ie.Current as XmlNode).Attributes["key"].Value == "ClientVersion")
                {
                    (ie.Current as XmlNode).Attributes["value"].Value = txtClientVersion.Text;
                }
            }

            doc.Save(path);
            var title = "ClashLand Server Manager";
            var message = "Changes has been saved!";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* END OF CONFIG EDITOR TAB*/

        /* MAIL TAB*/

        //Send To Gobal Chat Button
        private void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            foreach (var onlineplayer in Device.m_vOnlinePlayers)
            {
                string test = onlineplayer.Avatar.Name;

                if (onlineplayer.Device == null)
                {

                }
                else
                {
                    var msg = new Global_Chat_Entry(onlineplayer.Device)
                    {
                        Identifier = 24715,
                        Name = textBox22.Text,
                        Message = textBox21.Text,
                        Message_Sender = onlineplayer.Device.Player.Avatar,
                        Bot = true,
                        Regex = true,
                        Sender = true
                    };

                    msg.Send();
                }
            }

            /*foreach (var _Device in Resources.GChat.Get_Chat(Device).Values.ToList())
            {
                new Global_Chat_Entry(_Device)
                {
                    Message = textBox21.Text,
                    Message_Sender = this.Device.Player.Avatar,
                    Bot = true,
                    Regex = true,
                    Sender = this.Device == _Device
                }.Send();
            }*/
        }

        //Send To Mailbox Button
        private void materialRaisedButton10_ClickDevice(object sender, EventArgs e)
        {
            //this.Device = Device;
            var AllianceID = this.Device.Player.Avatar.ClanId;

            if (AllianceID > 0)
            {
                var Clan = Resources.Clans.Get(AllianceID, false);
                if (Clan != null)
                {
                    var Mail = new Mail
                    {
                        Stream_Type = ClashLand.Logic.Enums.Avatar_Stream.CLAN_MAIL,
                        Sender_ID = 29,
                        Sender_Name = "ClashLand",
                        Sender_Level = this.Device.Player.Avatar.Level,
                        Sender_League = this.Device.Player.Avatar.League,
                        Alliance_ID = 0,
                        Message = textBox24.Text
                    };
                    foreach (Member Member in Clan.Members.Values)
                    {
                        Member.Player.Avatar.Inbox.Add(Mail);
                    }
                };
            }
        }

        /* END OF MAIL TAB*/

        private void materialRaisedButton17_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Name in the Game.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialRaisedButton18_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Time delay where the Message will be automatically sent. (In Seconds)", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialRaisedButton19_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Message will be shown in the Global Chat.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialRaisedButton20_Click(object sender, EventArgs e)
        {
            var Name = textBox1.Text;
            int Interval = Convert.ToInt32(((textBox2.Text + 0) + 0) + 0);
            var Message = textBox3.Text;

            if (Convert.ToInt32(Interval) < 1)
            {
                MessageBox.Show("The Interval can't be null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                foreach (var _Device in Devices._Devices.Values.ToList())
                {
                    
                    new Global_Chat_Entry(_Device)
                    {
                        Message = Message,
                        Bot = true,
                        Message_Sender = null,
                    }.Send();
                }

                Count++;
                materialLabel13.Text = Convert.ToString(Count);

                T.Interval = Interval;
                T.Elapsed += ((s, o) =>
                {
                    //foreach (Level onlinePlayer in ResourcesManager.m_vOnlinePlayers)
                    {
                        //var pm = new GlobalChatLineMessage(onlinePlayer.Client)
                        {
                            //Message = Message,
                            //HomeId = 0,
                            //CurrentHomeId = 0,
                            //LeagueId = 22,
                            //PlayerName = Name
                        };
                        //pm.Send();
                    }
                    Count++;
                    materialLabel13.Text = Convert.ToString(Count);
                });
                T.Start();
            }
        }

        private void materialRaisedButton25_Click(object sender, EventArgs e)
        {
        }
        private void materialRaisedButton19_Click_1(object sender, EventArgs e)
        {
            T.Stop();
            Count = 0;
            materialLabel13.Text = Convert.ToString(Count);
        }

        private void materialTabSelector1_Click(object sender, EventArgs e)
        {

        }

        private void txtDatabaseType_Click(object sender, EventArgs e)
        {

        }

        private void txtAdminMessage_Click(object sender, EventArgs e)
        {

        }

        private void txtUpdateURL_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            if (Constants.Maintenance != null)
            {
                Constants.Maintenance = null;
                Resources.Classes.Timers.LTimers[4].Stop();
                Resources.Classes.Timers.LTimers[5].Stop();
                Resources.Classes.Timers.LTimers.Remove(4);
                Resources.Classes.Timers.LTimers.Remove(5);
                Console.Clear();
                Console.WriteLine("# " + DateTime.Now.ToString("d") +
                                  " ---- Exited from Maintanance Mode---- " + DateTime.Now.ToString("T") +
                                  " #");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("# " + DateTime.Now.ToString("d") +
                                  " ---- Not in Maintanance Mode---- " + DateTime.Now.ToString("T") +
                                  " #");
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine("Please enter the duration of Maintenance (Minute): ");
            int i = 0;
            if (Int32.TryParse(Console.ReadLine(), out i))
            {
                Resources.Classes.Timers.Maintenance(i);
                Resources.Classes.Timers.LTimers[4].Start();
            }
        }
    }
}