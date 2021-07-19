using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using VideoOS.Platform.SDK.Platform;
using VideoOS.ConfigurationAPI;
using System.Collections.ObjectModel;

using System.IO;
using System.Text.Json;
using System.Threading;

namespace MoveHardware
{
    public partial class Main : Form
    {

        /*
         * Integration description
         */
        private static readonly Guid IntegrationId = new Guid("CD52BF80-A58B-4A35-BF30-159753159753");
        private const string IntegrationName = "MoveHardware";
        private const string Version = "1.0";
        private const string ManufacturerName = "SGIU";

        /*
         * API clients 
         */

        private ConfigApiClient _configApiClient1;
        private ConfigApiClient _configApiClient2;


        public Main()
        {
            InitializeComponent();
            Button_Connect_S1_Click(null, null); // AUTOMATIC CLICK REMOVE ON PROD
            Button_Connect_S2_Click(null, null); // AUTOMATIC CLICK REMOVE ON PROD
           
        }

        internal static Collection<String> _showItemsType = new Collection<string>()
                                                       {
                                                           ItemTypes.System,
                                                           ItemTypes.RecordingServer,
                                                           ItemTypes.RecordingServerFolder,
//                                                      ItemTypes.HardwareDriverFolder,
//                                                      ItemTypes.HardwareDriver,
	                                                       ItemTypes.Hardware,
                                                           ItemTypes.HardwareFolder,
	             /*                                          ItemTypes.CameraFolder,
	                                                        ItemTypes.InputEventFolder,
	                                                       ItemTypes.OutputFolder,
	                                                       ItemTypes.MicrophoneFolder,
	                                                       ItemTypes.SpeakerFolder,
	                                                       ItemTypes.MetadataFolder,
	                                                       ItemTypes.Camera,
	                                                       ItemTypes.InputEvent,
	                                                       ItemTypes.Output,
	                                                       ItemTypes.Microphone,
	                                                       ItemTypes.Speaker,
	                                                       ItemTypes.Metadata,
                                                           ItemTypes.CameraGroup,
                                                           ItemTypes.CameraGroupFolder,
                                                           ItemTypes.MetadataGroup,
                                                           ItemTypes.MetadataGroupFolder,
                                                           ItemTypes.MicrophoneGroup,
                                                           ItemTypes.MicrophoneGroupFolder,
                                                           ItemTypes.SpeakerGroup,
                                                           ItemTypes.SpeakerGroupFolder,
                                                           ItemTypes.InputEventGroup,
                                                           ItemTypes.InputEventGroupFolder,
                                                           ItemTypes.OutputGroup,
                                                           ItemTypes.OutputGroupFolder,
                                                        ItemTypes.BasicUserFolder,
														   ItemTypes.BasicUser,
                                                           ItemTypes.Role,
                                                           ItemTypes.RoleFolder,
                                                           ItemTypes.StorageFolder,
                                                           ItemTypes.Storage,
                                                           ItemTypes.LayoutFolder,
                                                           ItemTypes.LayoutGroup,
                                                           ItemTypes.LayoutGroupFolder,
                                                           ItemTypes.VideoWall,
                                                           ItemTypes.VideoWallFolder,
                                                           ItemTypes.Monitor,
                                                           ItemTypes.MonitorFolder,
                                                           ItemTypes.VideoWallPreset,
                                                           ItemTypes.VideoWallPresetFolder,
                                                           ItemTypes.MonitorPresetFolder,
                                                           ItemTypes.UserDefinedEventFolder,
                                                           ItemTypes.UserDefinedEvent,
                                                           ItemTypes.AnalyticsEventFolder,
                                                           ItemTypes.AnalyticsEvent,
                                                           ItemTypes.GenericEventFolder,
                                                           ItemTypes.GenericEventDataSourceFolder,
                                                           ItemTypes.GisMapLocation,
                                                           ItemTypes.GisMapLocationFolder,
                                                           ItemTypes.TimeProfile,
                                                           ItemTypes.TimeProfileFolder,
                                                           ItemTypes.MIPKind,
                                                           ItemTypes.MIPKindFolder,
                                                           ItemTypes.MIPItem,
                                                           ItemTypes.MIPItemFolder,
                                                           ItemTypes.AlarmDefinition,
                                                           ItemTypes.AlarmDefinitionFolder,
                                                           ItemTypes.LprMatchList,
                                                           ItemTypes.LprMatchListFolder,
                                                           ItemTypes.SaveSearches,
                                                           ItemTypes.FindSaveSearches,
                                                           ItemTypes.SaveSearchesFolder,
                                                           ItemTypes.Rule,
                                                           ItemTypes.RuleFolder,
                                                           ItemTypes.AccessControlSystem,
                                                           ItemTypes.AccessControlSystemFolder,
                                                           ItemTypes.AccessControlUnit,
                                                           ItemTypes.AccessControlUnitFolder,
                                                           ItemTypes.Site,
                                                           ItemTypes.SiteFolder,
                                                           ItemTypes.LicenseInformationFolder,
                                                           ItemTypes.LicenseInstalledProductFolder,
                                                           ItemTypes.LicenseOverviewAllFolder,
                                                           ItemTypes.LicenseDetailFolder,
                                                           ItemTypes.LicenseInformation,
                                                           ItemTypes.LicenseInstalledProduct,
                                                           ItemTypes.LicenseOverviewAll,
                                                           ItemTypes.LicenseDetail,
                                                           ItemTypes.BasicOwnerInformationFolder,
                                                           ItemTypes.BasicOwnerInformation,
       */                                                    
        };


        private void Login(Uri uri, NetworkCredential nc)
        {
            VideoOS.Platform.SDK.Environment.Initialize();

            VideoOS.Platform.SDK.Environment.AddServer(uri, nc);
            try
            {
                VideoOS.Platform.SDK.Environment.Login(uri, IntegrationId, IntegrationName, Version, ManufacturerName);
            }
            catch (ServerNotFoundMIPException snfe)
            {
                MessageBox.Show(snfe.Message);

                // Report  "Server not found: "
            }
            catch (InvalidCredentialsMIPException ice)
            {
                MessageBox.Show(ice.ToString());

                // Report  "Invalid credentials" 
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                // Report  "Other error connecting to: " + uri.DnsSafeHost;
            }

            _configApiClient1 = new ConfigApiClient();
            string _serverAddress = uri.ToString();
            int _serverPort = 80;
            bool _corporate = true;

            _configApiClient1.ServerAddress = _serverAddress;
            _configApiClient1.Serverport = _serverPort;
            _configApiClient1.ServerType = _corporate
                                              ? ConfigApiClient.ServerTypeEnum.Corporate
                                              : ConfigApiClient.ServerTypeEnum.Arcus;
            _configApiClient1.Initialize();
            if (_configApiClient1.Connected)
                toolStripStatusLabel_S1.Text = "Logged on";
            else
                toolStripStatusLabel_S1.Text = "Error logging on";
            configAPI(_configApiClient1, treeView_S1);
        }

        private void configAPI(ConfigApiClient _configApiClient, TreeView treeView)
        {
            var server = _configApiClient.GetItem("/");
            TreeNode serverTn = new TreeNode(server.DisplayName)
            {
                Tag = server,
                Checked = false,
            };

            treeView.Nodes.Add(serverTn);
            serverTn.Nodes.AddRange(AddChildren("/", _configApiClient));
        }


        private TreeNode[] AddChildren(string node, ConfigApiClient _configApiClient)
        {
            List<TreeNode> children = new List<TreeNode>();
            foreach (ConfigurationItem child in _configApiClient.GetChildItems(node))
            {
                if (_showItemsType.Contains(child.ItemType))
                {
                    //            Guid id = child.FQID.ObjectId != Guid.Empty ? child.FQID.ObjectId : child.FQID.ServerId.Id;

                    TreeNode tn = new TreeNode(child.DisplayName)
                    {
                        Tag = child,
                        Checked = false,
                           ImageIndex = UI.Icons.GetImageIndex(child.ItemType),
                           SelectedImageIndex = UI.Icons.GetImageIndex(child.ItemType)
                    };
                    //if (child.FQID.Kind != Kind.Folder && child.FQID.ObjectId != child.FQID.Kind)
                    //{
                    //    if (_treeNodeCache.ContainsKey(id) == false)
                    //        _treeNodeCache.Add(id, tn);
                    // }
                    children.Add(tn);
                    Console.WriteLine(child.Path);
                    tn.Nodes.AddRange(AddChildren(child.Path, _configApiClient));

                }
            }
            return children.ToArray();

        }




        private void Button_Connect_S1_Click(object sender, EventArgs e)
        {


            // TODO: basic user 

            //Uri uri = new Uri("http://10.3.32.18"1);
            Uri uri = new Uri("http://" + textBox_S1.Text);

            // This will reuse the Windows credentials you are logged in with
            //  NetworkCredential nc = System.Net.CredentialCache.DefaultNetworkCredentials;


            // You need this to apply "basic" credentials.
            // Below, do AddServer(uri, cc) instead of AddServer(uri, nc)
            //           CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(uri, "Test", "Milestone1$", "Basic");

            // Alternatively, the BuildCredentialCache can also build credential for Windows login
            // CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(uri, "MEX-LAB\\SGIU", "Milestone1$", "Negotiate"); 


            String user = textBox_User_S1.Text;
            String pass = textBox_Password_S1.Text;
            String domain = textBox_Domain__S1.Text;
            // This will use specific Windows credentials
            NetworkCredential nc = new NetworkCredential(user, pass, domain);

            Login(uri, nc);

        }

        private void treeView_S1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView_S1.SelectedNode = e.Node;
            }
        }

        private void DoAfterSelect(TreeNode node)
        {
            ConfigurationItem item = node.Tag as ConfigurationItem;

            getCameraProperties(item, _configApiClient1);


        }

        private void DoAfterSelect_S2(TreeNode node)
        {
            ConfigurationItem item = node.Tag as ConfigurationItem;

            getCameraProperties(item, _configApiClient2);


        }

        private void treeView_S1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            DoAfterSelect(e.Node);
            Cursor.Current = Cursors.Default;
        }



        private CameraProperties getCameraProperties(ConfigurationItem _item, ConfigApiClient _configApiClient)
        {

            if (_item.ItemType == "Hardware")
            {

                /* Compare with PS-TOOL
                string json = JsonSerializer.Serialize(_item);
                File.WriteAllText(@"C:\temp\test.json", json);
                Seems to have the same information. empty variables are ignored in SDK version 
                 */

                Console.WriteLine(_item.ItemType);

                // Get Properties 
                string id = (from item in _item.Properties where item.Key == "Id" select item.Value).First();
                Console.WriteLine(id);

                string name = (from item in _item.Properties where item.Key == "Name" select item.Value).First();
                Console.WriteLine(name);

                string description = (from item in _item.Properties where item.Key == "Description" select item.Value).First();
                Console.WriteLine(description);

                string address = (from item in _item.Properties where item.Key == "Address" select item.Value).First();
                Console.WriteLine(address);

                string userName = (from item in _item.Properties where item.Key == "UserName" select item.Value).First();
                Console.WriteLine(userName);

                string model = (from item in _item.Properties where item.Key == "Model" select item.Value).First();
                Console.WriteLine(model);

                string hardwareDriverPath = (from item in _item.Properties where item.Key == "HardwareDriverPath" select item.Value).First();
                Console.WriteLine(hardwareDriverPath);

                // Get Password 
                ConfigurationItem result = _configApiClient.InvokeMethod(_item, "ReadPasswordHardware");
                ConfigurationItem password_result = _configApiClient.InvokeMethod(result, result.MethodIds[0]);
                string password = (from item in password_result.Properties where item.Key == "Password" select item.Value).First();
                Console.WriteLine(password);

                // Get Recording Server 
                var recordingServer = _configApiClient.GetItem(_item.ParentPath.Split('/')[0]);
                string recordingServerId = (from item in recordingServer.Properties where item.Key == "Id" select item.Value).First();
                Console.WriteLine(recordingServerId);

                // Get Hardware Driver 
                ConfigurationItem[] hardwareDriverFolder = _configApiClient.GetChildItems(_item.ParentPath.Split('/')[0] + "/HardwareDriverFolder");
                var hardwareProp = (from item in hardwareDriverFolder where item.Path == hardwareDriverPath select item.Properties).First();
                string hardwareDriverId = (from item in hardwareProp where item.Key == "Number" select item.Value).First();
                Console.WriteLine(hardwareDriverId);

                CameraProperties cameraProperties = new CameraProperties() { Address = address, Name = name, Password = password, UserName = userName, DriverNumber = hardwareDriverId };
                return cameraProperties;
            }
            return null;

        }



        private void fillAllChilds(ConfigurationItem item, ConfigApiClient _configApiClient)
        {
            FillChildren(item, _configApiClient);
            foreach (var child in item.Children)
            {
                fillAllChilds(child, _configApiClient);
            }
        }


        private void FillChildren(ConfigurationItem item, ConfigApiClient _configApiClient)
        {
            if (!item.ChildrenFilled)
            {
                item.Children = _configApiClient.GetChildItems(item.Path);
                item.ChildrenFilled = true;
            }
            if (item.Children == null)
                item.Children = new ConfigurationItem[0];
        }

        private void button_Move_Selection_Click(object sender, EventArgs e)
        {

            /// PLEASE PLEASE PLEASE REFACTOR THIS, 

            ConfigurationItem _item = treeView_S1.SelectedNode.Tag as ConfigurationItem;
            CameraProperties cameraProperties = getCameraProperties(_item, _configApiClient1);

            ConfigurationItem _recordingServer = treeView_S2.SelectedNode.Tag as ConfigurationItem;

            // Get Hardware Driver 
            ConfigurationItem[] hardwareDriverFolder = _configApiClient2.GetChildItems(_recordingServer.Path + "/HardwareDriverFolder");

            var hardwarePath = (from item in hardwareDriverFolder where item.Properties[3].Value == cameraProperties.DriverNumber select item.Path).First();

            ConfigurationItem addHarwareInfo = _configApiClient2.InvokeMethod(_recordingServer, "AddHardware");




            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareAddress").Value = cameraProperties.Address;
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "UserName").Value = cameraProperties.UserName;
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "Password").Value = cameraProperties.Password;
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareDriverPath").Value = hardwarePath;

            /*   foreach (Property property in addHarwareInfo.Properties)
               {
                   switch (property.Key)// TODO: Improve this ????
                   {
                       case "HardwareAddress": property.Value = cameraProperties.Address; break;
                       case "UserName": property.Value = cameraProperties.UserName; break;
                       case "Password": property.Value = cameraProperties.Password; break;
                       case "HardwareDriverPath": property.Value = hardwarePath; break;
                       default:
                           break;
                   }

               }
            */

            ConfigurationItem addHarwareResult = _configApiClient2.InvokeMethod(addHarwareInfo, addHarwareInfo.MethodIds[0]);

            String taskPath = addHarwareResult.Properties[6].Value; // TODO: Improve this  USING INDEX IS NOT GOOD


            var status = _configApiClient2.GetItem(taskPath);

            while (status.Properties.Length < 3)
            {
                status = _configApiClient2.GetItem(taskPath);
                Console.WriteLine(status.Properties[0].Value); // TODO: Improve this  USING INDEX IS NOT GOOD
                Thread.Sleep(300);


            }
            Console.WriteLine(status.Properties[2].Value); // TODO: Improve this  USING INDEX IS NOT GOOD --> Success message 

            // No errors, we are on the track 

            // We have the hardware on the new RS, no names or configs yet .

            /// If the hardware has been added, set the childs. 

            fillAllChilds(_item, _configApiClient1);  // Fill source childs 

            String justAddedHardware = status.Properties[1].Value;
            ConfigurationItem destH = _configApiClient2.GetItem(justAddedHardware);
            fillAllChilds(destH, _configApiClient2);  // Fill dest childs. 

            MatchStreams(_item, destH); // Add stream if necesary 

            destH = _configApiClient2.GetItem(justAddedHardware);
            fillAllChilds(destH, _configApiClient2);  // Fill dest childs. 

            CopyConfigurationItem(_item, destH);

            // _configApiClient2.SetItem(destH);
            SetAllChilds(destH, _configApiClient2);


        }

        private void MatchStreams(ConfigurationItem src, ConfigurationItem dest)
        {
            ConfigurationItem _cameras_S1 = Array.Find(src.Children, ele => ele.ItemType == "CameraFolder");
            ConfigurationItem _cameras_S2 = Array.Find(dest.Children, ele => ele.ItemType == "CameraFolder");

            for (int i = 0; i < _cameras_S1.Children.Length; i++)
            {
                ConfigurationItem _camera_S1 = _cameras_S1.Children[i];
                ConfigurationItem _streamsFolder_S1 = Array.Find(_camera_S1.Children, ele => ele.ItemType == "StreamFolder");
                ConfigurationItem _streams_S1 = Array.Find(_streamsFolder_S1.Children, ele => ele.ItemType == "Stream");

                ConfigurationItem _camera_S2 = _cameras_S2.Children[i];
                ConfigurationItem _streamsFolder_S2 = Array.Find(_camera_S2.Children, ele => ele.ItemType == "StreamFolder");
                ConfigurationItem _streams_S2 = Array.Find(_streamsFolder_S2.Children, ele => ele.ItemType == "Stream");

                if (_streams_S1.Children.Length > 1)
                {
                    for (int j = 1; j < _streams_S1.Children.Length; j++)
                    {
                        AddStream(_streams_S2);

                    }

                }

            }


        }

        private void AddStream(ConfigurationItem streams_S2)
        {
            ConfigurationItem addStreamInfo = _configApiClient2.InvokeMethod(streams_S2, "AddStream");

            ConfigurationItem addStreamResult = _configApiClient2.InvokeMethod(addStreamInfo, addStreamInfo.MethodIds[0]);

            String taskPath = addStreamResult.Properties[0].Value; // TODO: Improve this  USING INDEX IS NOT GOOD
            Console.WriteLine(taskPath);

        }



        private void CopyConfigurationItem(ConfigurationItem src, ConfigurationItem dest)
        {

            dest.DisplayName = src.DisplayName;

            if (src.EnableProperty != null)
                dest.EnableProperty.Enabled = src.EnableProperty.Enabled;

            if (src.Properties != null)
            {
                for (int i = 0; i < src.Properties.Length; i++)
                {
                    if (src.Properties[i].IsSettable)
                    {
                        dest.Properties[i].Value = src.Properties[i].Value;
                        Console.WriteLine(src.Properties[i].DisplayName + ", " + src.Properties[i].Value + ", Added");
                    }
                    else
                    {
                        Console.WriteLine(src.Properties[i].DisplayName + ", " + src.Properties[i].Value + ", Not Added");
                    }

                }
            }


            if (src.Children != null)
            {
                for (int i = 0; i < src.Children.Length; i++)
                {
                    CopyConfigurationItem(src.Children[i], dest.Children[i]);
                }
            }
        }




        private void SetAllChilds(ConfigurationItem item, ConfigApiClient _configApiClient)
        {

            try
            {

                if (!item.ItemType.Contains("Folder") && !(item.ItemType == "Stream" && !(item.ItemCategory == "Item")) && !(item.ItemType == "ClientSettings"))  // ignore Stream - !Items no settings there, 
                                                                                                                                                                  // Anything with the word "Folder", just the structure
                                                                                                                                                                  // Client Settings will have many paths and IDs, mean problems
                {
                    Console.WriteLine("Try to set -> Item Type: " + item.ItemType, ", Category: " + item.ItemCategory, ", Display Name: " + item.DisplayName);
                    _configApiClient2.SetItem(item);
                }

            }
            catch (Exception r)
            {

                Console.WriteLine("Error setting item: " + item + "error: " + r);
            }

            foreach (var child in item.Children)
            {
                SetAllChilds(child, _configApiClient);
            }

        }

        //----


        private void Button_Connect_S2_Click(object sender, EventArgs e)
        {
            Uri uri = new Uri("http://" + textBox_Address_S2.Text);
            String user = textBox_User_S2.Text;
            String pass = textBox_Password_S2.Text;
            String domain = textBox_Domain_S2.Text;
            NetworkCredential nc = new NetworkCredential(user, pass, domain);
            VideoOS.Platform.SDK.Environment.AddServer(uri, nc);

            try
            {
                VideoOS.Platform.SDK.Environment.Login(uri, IntegrationId, IntegrationName, Version, ManufacturerName);
            }
            catch (ServerNotFoundMIPException snfe)
            {
                MessageBox.Show(snfe.Message);

                // Report  "Server not found: "
            }
            catch (InvalidCredentialsMIPException ice)
            {
                MessageBox.Show(ice.ToString());

                // Report  "Invalid credentials" 
            }
            catch (Exception ed)
            {
                MessageBox.Show(ed.Message);
                // Report  "Other error connecting to: " + uri.DnsSafeHost;
            }


            // no errors, good news 

            _configApiClient2 = new ConfigApiClient();
            string _serverAddress = uri.ToString();
            int _serverPort = 80;
            bool _corporate = true;

            _configApiClient2.ServerAddress = _serverAddress;
            _configApiClient2.Serverport = _serverPort;
            _configApiClient2.ServerType = _corporate
                                              ? ConfigApiClient.ServerTypeEnum.Corporate
                                              : ConfigApiClient.ServerTypeEnum.Arcus;
            _configApiClient2.Initialize();
            if (_configApiClient2.Connected)
                label_Status_S2.Text = "Logged on";
            else
                label_Status_S2.Text = "Error logging on";
            configAPI(_configApiClient2, treeView_S2);

        }

        // Refactor this duplicate code with treeview_s1 - LOW PRIORITY
        private void treeView_S2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            DoAfterSelect_S2(e.Node);
            Cursor.Current = Cursors.Default;
        }

    }

    public static class InvokeInfoProperty
    {
        public const string Progress = "Progress";
        public const string Path = "Path";
        public const string ErrorCode = "ErrorCode";
        public const string ErrorText = "ErrorText";
        public const string State = "State";
    }

    class CameraProperties
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DriverNumber { get; set; }
    }

}
