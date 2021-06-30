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
        public Main()
        {
            InitializeComponent();
            Button_Connect_S1_Click(null, null); // AUTOMATIC CLICK REMOVE ON PROD


        }

        private static readonly Guid IntegrationId = new Guid("CD52BF80-A58B-4A35-BF30-159753159753");
        private const string IntegrationName = "MoveHardware";
        private const string Version = "1.0";
        private const string ManufacturerName = "SGIU";


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



        private ConfigApiClient _configApiClient;

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


            if (VideoOS.Platform.SDK.Environment.IsLoggedIn(uri))
            {
                _configApiClient = new ConfigApiClient();
                string _serverAddress = uri.ToString();
                int _serverPort = 80;
                bool _corporate = true;

                _configApiClient.ServerAddress = _serverAddress;
                _configApiClient.Serverport = _serverPort;
                _configApiClient.ServerType = _corporate
                                                  ? ConfigApiClient.ServerTypeEnum.Corporate
                                                  : ConfigApiClient.ServerTypeEnum.Arcus;
                _configApiClient.Initialize();
                if (_configApiClient.Connected)
                    toolStripStatusLabel_S1.Text = "Logged on";
                else
                    toolStripStatusLabel_S1.Text = "Error logging on";
                configAPI();
            }
        }

        private void configAPI()
        {
            var server = _configApiClient.GetItem("/");
            TreeNode serverTn = new TreeNode(server.DisplayName)
            {
                Tag = server,
                Checked = false,
            };

            treeView_S1.Nodes.Add(serverTn);
            serverTn.Nodes.AddRange(AddChildren("/"));
        }


        private TreeNode[] AddChildren(string node)
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
                        //   ImageIndex = UI.Icons.GetImageIndex(child.ItemType),
                        //   SelectedImageIndex = UI.Icons.GetImageIndex(child.ItemType)
                    };
                    //if (child.FQID.Kind != Kind.Folder && child.FQID.ObjectId != child.FQID.Kind)
                    //{
                    //    if (_treeNodeCache.ContainsKey(id) == false)
                    //        _treeNodeCache.Add(id, tn);
                    // }
                    children.Add(tn);
                    Console.WriteLine(child.Path);
                    tn.Nodes.AddRange(AddChildren(child.Path));

                }
            }
            return children.ToArray();

        }




        private void Button_Connect_S1_Click(object sender, EventArgs e)
        {


            // TODO: MULTIPLE LOGIN OPTIONS

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
            getCameraProperties(item);


        }

        private void treeView_S1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            DoAfterSelect(e.Node);
            Cursor.Current = Cursors.Default;
        }



        private CameraProperties getCameraProperties(ConfigurationItem _item)
        {

            if (_item.ItemType == "Hardware")
            {
                /// fill all hardware childs
                //          fillAllChilds(_item); /// THIS CAN BE DONE AFTER THE CAMERA IS ADDED 


                /* Compare with PS-TOOL
                string json = JsonSerializer.Serialize(_item);
                File.WriteAllText(@"C:\temp\test.json", json);
                Seems to have the same information. some empty variables are ignored in SDK version 
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

                CameraProperties cameraProperties = new CameraProperties() { Address = address, Name = name, Password = password, UserName = userName, DriverNumber = hardwareDriverId};
                return cameraProperties;
            }
            return null;

        }



        private void fillAllChilds(ConfigurationItem item)
        {
            FillChildren(item);
            foreach (var child in item.Children)
            {
                fillAllChilds(child);
            }
        }


        private void FillChildren(ConfigurationItem item)
        {
            if (!item.ChildrenFilled)
            {
                item.Children = _configApiClient.GetChildItems(item.Path);
                item.ChildrenFilled = true;
            }
            if (item.Children == null)
                item.Children = new ConfigurationItem[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // DO THE TRICK


            ConfigurationItem _item = treeView_S1.SelectedNode.Tag as ConfigurationItem;
            CameraProperties cameraProperties = getCameraProperties(_item);


            var _recordingServer = _configApiClient.GetItem("RecordingServer[9D1FE046-07B9-4906-ADCE-50418741711F]"); // TODO: TARGET RS --------------------------------------

            // Get Hardware Driver 
            ConfigurationItem[] hardwareDriverFolder = _configApiClient.GetChildItems(_recordingServer.Path + "/HardwareDriverFolder");

            var hardwarePath = (from item in hardwareDriverFolder where item.Properties[3].Value == cameraProperties.DriverNumber select item.Path).First();
             

            ConfigurationItem addHarwareInfo = _configApiClient.InvokeMethod(_recordingServer, "AddHardware");

            foreach (Property property in addHarwareInfo.Properties) {
                switch (property.Key)// TODO: Improve this ????
                {
                    case "HardwareAddress": property.Value = cameraProperties.Address; break;
                    case "UserName": property.Value = cameraProperties.UserName;break;
                    case "Password": property.Value = cameraProperties.Password; break;
                    case "HardwareDriverPath": property.Value = hardwarePath ; break;
                    // case "CustomData": property.Value = cameraProperties.UserName;break;

                    default:
                        break;
                }

            }


             ConfigurationItem addHarwareResult = _configApiClient.InvokeMethod(addHarwareInfo, addHarwareInfo.MethodIds[0]);

            String taskPath = addHarwareResult.Properties[6].Value; // TODO: Improve this  USING INDEX IS NOT GOOD


            var status  = _configApiClient.GetItem(taskPath);

            while (status.Properties.Length < 3)
            {
                status = _configApiClient.GetItem(taskPath);
                Console.WriteLine(status.Properties[0].Value); // TODO: Improve this  USING INDEX IS NOT GOOD
                Thread.Sleep(300);
                

            }
            Console.WriteLine(status.Properties[2].Value); // TODO: Improve this  USING INDEX IS NOT GOOD
            Console.WriteLine(status.Properties[3].Value);// TODO: Improve this  USING INDEX IS NOT GOOD
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
