using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using VideoOS.Platform.SDK.Platform;
using VideoOS.ConfigurationAPI;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace MoveHardware
{
    public partial class Main : Form
    {

        /*
         * Integration info
         */
        private static readonly Guid IntegrationId = new Guid("CD52BF80-A58B-4A35-BF30-159753159753");
        private const string IntegrationName = "MoveHardware";
        private const string Version = "1.0";
        private const string ManufacturerName = "SGIU";

        private ConfigApiClient _configApiClient1;          // Api client for src server
        private ConfigApiClient _configApiClient2;          // Api client for dest server

        private IList<ConfigurationItem> checkedNodes;      // List of checked Nodes
        bool busy = false;                                  // Axiliary boolean to safe check treeView items 
        
        String tasksResultCache = "";                        // Until i find a way to safe call console write method 

        public Main()
        {
            InitializeComponent();

            VideoOS.Platform.SDK.Environment.Initialize();

            _configApiClient1 = new ConfigApiClient();
            _configApiClient2 = new ConfigApiClient();

            UI.Icons.Init();                                    // TreeView Decorator
            treeView_S1.ImageList = UI.Icons.IconListBlack;
            treeView_S2.ImageList = UI.Icons.IconListBlack;

            Button_Connect_S1_Click(null, null); // AUTOMATIC CLICK - REMOVE ON PROD
            Button_Connect_S2_Click(null, null); // AUTOMATIC CLICK - REMOVE ON PROD
        }

        /// <summary>
        /// ViewTree Filter 
        /// </summary>
        internal static Collection<String> _showItemsType = new Collection<string>()
                                                        {
                                                           ItemTypes.System,
                                                           ItemTypes.RecordingServerFolder,
                                                           ItemTypes.RecordingServer,
                                                           ItemTypes.HardwareFolder,
                                                           ItemTypes.Hardware,
                                                        };


        /// <summary>
        /// Login into the a Management Server (C-CODE)
        /// </summary>
        /// <param name="uri">Server URI</param>
        /// <param name="nc">Credentials</param>
        private void Login(Uri uri, NetworkCredential nc, ConfigApiClient _configApiClient, Label toolStripStatusLabel, Button button_Connect)
        {
            VideoOS.Platform.SDK.Environment.AddServer(uri, nc);                                                            // Add the server to the environment 
            try
            {
                VideoOS.Platform.SDK.Environment.Login(uri, IntegrationId, IntegrationName, Version, ManufacturerName);     // attempt to login 
            }
            catch (ServerNotFoundMIPException snfe)
            {
                MessageBox.Show(snfe.Message);                                                                              // Report  "Server not found: "
            }
            catch (InvalidCredentialsMIPException ice)
            {
                MessageBox.Show(ice.ToString());                                                                            // Report  "Invalid credentials" 
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);                                                                                 // Report  "Other error connecting to: " + uri.DnsSafeHost;
            }


            string _serverAddress = uri.ToString();                           // server URI
            int _serverPort = 80;                                             // Server port - TODO: Harcoded port 
            bool _corporate = true;                                           // c-code - TODO: Harcoded type

            _configApiClient.ServerAddress = _serverAddress;                  // set API Client
            _configApiClient.Serverport = _serverPort;
            _configApiClient.ServerType = _corporate
                                              ? ConfigApiClient.ServerTypeEnum.Corporate
                                              : ConfigApiClient.ServerTypeEnum.Arcus;
            _configApiClient.Initialize();                                    // Initialize API

            if (_configApiClient.Connected)
            {
                toolStripStatusLabel.Text = "Logged on";                    // If connected change status label 
                button_Connect.Text = "Disconnect";
            }
            else
                toolStripStatusLabel.Text = "Error logging on";             // If not connected change status label 

        }


        /// <summary>
        /// Push Connect Source Server Button Action 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_S1_Click(object sender, EventArgs e)
        {
            if (_configApiClient1.Connected)                                                                    // If connected Disconect 
            {
                VideoOS.Platform.SDK.Environment.RemoveServer(new Uri(_configApiClient1.ServerAddress));        // Remove server
                _configApiClient2.Close();                                                                      // Close API (REVIEW THIS) 
                Button_Connect_S1.Text = "Connect";                                                             // Change button text 
            }
            else
            {
                Uri uri = new Uri("http://" + textBox_S1.Text);                                                 // Fetch URI
                String user = textBox_User_S1.Text;                                                             // Fetch user 
                String pass = textBox_Password_S1.Text;                                                         // Fetch pass 
                String domain = textBox_Domain__S1.Text;                                                        // Fetch domain 
                NetworkCredential nc = new NetworkCredential(user, pass, domain);                               // Build credentials

                Login(uri, nc, _configApiClient1, toolStripStatusLabel_S1, Button_Connect_S1);                  // Call Login method
                PopulateTreeView(_configApiClient1, treeView_S1);                                               // Show items 
            }
        }

        /// <summary>
        /// Push Connect Destiny Server Button Action 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_S2_Click(object sender, EventArgs e)
        {
            if (_configApiClient2.Connected)                                                                    // If connected Disconect 
            {
                VideoOS.Platform.SDK.Environment.RemoveServer(new Uri(_configApiClient2.ServerAddress));        // Remove server
                _configApiClient2.Close();                                                                      // Close API (REVIEW THIS) 
                Button_Connect_S2.Text = "Connect";                                                             // Change button text 
            }
            else
            {
                Uri uri = new Uri("http://" + textBox_Address_S2.Text);                                         // Fetch URI
                String user = textBox_User_S2.Text;                                                             // Fetch user 
                String pass = textBox_Password_S2.Text;                                                         // Fetch pass 
                String domain = textBox_Domain_S2.Text;                                                         // Fetch domain 
                NetworkCredential nc = new NetworkCredential(user, pass, domain);                               // Build credentials

                Login(uri, nc, _configApiClient2, toolStripStatusLabel_S2, Button_Connect_S2);                  // Call Login method
                PopulateTreeView(_configApiClient2, treeView_S2);                                               // Show items 
            }
        }

        /// <summary>
        /// Populate the TreeView using the API client 
        /// </summary>
        /// <param name="_configApiClient"></param>
        /// <param name="treeView"></param>
        private void PopulateTreeView(ConfigApiClient _configApiClient, TreeView treeView)
        {
            var server = _configApiClient.GetItem("/");                                             // Start from the root
            TreeNode serverTn = new TreeNode(server.DisplayName)                                    // Create a new node 
            {
                Tag = server,               // Store the ConfurationItem in the tag 
                Checked = false             // Is not checked 
            };
            treeView.Nodes.Add(serverTn);                                                           // Add the root node 
            serverTn.Nodes.AddRange(AddChildren("/", _configApiClient));                            // Add root children
        }

        /// <summary>
        /// Auxiliary Method to Populate Tree View 
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="_configApiClient">Milestone API</param>
        /// <returns></returns>
        private TreeNode[] AddChildren(string node, ConfigApiClient _configApiClient)
        {
            List<TreeNode> children = new List<TreeNode>();                                         //  Create an empty list of TreeNode 

            foreach (ConfigurationItem child in _configApiClient.GetChildItems(node))               //  For each clild of the parent node 
            {
                if (_showItemsType.Contains(child.ItemType))                                        //  Apply Filter 
                {
                    TreeNode tn = new TreeNode(child.DisplayName)                                   //  Create a TreeNode 
                    {
                        Tag = child,                                                    // Store the ConfigurationItem in the tag
                        Checked = false,                                                // Is not checked 
                        ImageIndex = UI.Icons.GetImageIndex(child.ItemType),            // Icon
                        SelectedImageIndex = UI.Icons.GetImageIndex(child.ItemType)     // Selected Icon 
                    };
                    children.Add(tn);                                                               //  Add node to childen list 
                    tn.Nodes.AddRange(AddChildren(child.Path, _configApiClient));                   //  Recursive call with the child as parent 
                }
            }

            return children.ToArray();                                                              // Return the children list
        }



        /// <summary>
        /// Get the needed information to add a camera to a the server 
        /// Information to gather: Name, Address, UserName, HardwareDriverId, Password 
        /// </summary>
        /// <param name="_item">ConfigurationItem</param>
        /// <param name="_configApiClient">Milestone API</param>
        /// <returns></returns>
        private CameraProperties GetCameraProperties(ConfigurationItem _item, ConfigApiClient _configApiClient)
        {

            // Get Properties 
            string name = (from item in _item.Properties where item.Key == "Name" select item.Value).First();                                   // Find the hardware name 
            string address = (from item in _item.Properties where item.Key == "Address" select item.Value).First();                             // Find the hardware IP address
            string userName = (from item in _item.Properties where item.Key == "UserName" select item.Value).First();                           // Find the hardware Username  
            string hardwareDriverPath = (from item in _item.Properties where item.Key == "HardwareDriverPath" select item.Value).First();       // Find the hardware HardwareDriverPath

            // Get Password 
            ConfigurationItem result = _configApiClient.InvokeMethod(_item, "ReadPasswordHardware");                                            // Invoke API method to recover password
            ConfigurationItem password_result = _configApiClient.InvokeMethod(result, result.MethodIds[0]);                                     // Execute API method to recover password
            string password = (from item in password_result.Properties where item.Key == "Password" select item.Value).First();                 // Get password 

            // Get Hardware Driver 
            ConfigurationItem[] hardwareDriverFolder = _configApiClient.GetChildItems(_item.ParentPath.Split('/')[0] + "/HardwareDriverFolder");// Fetch DriversFolder 
            var hardwareProp = (from item in hardwareDriverFolder where item.Path == hardwareDriverPath select item.Properties).First();        // Find Driver
            string hardwareDriverId = (from item in hardwareProp where item.Key == "Number" select item.Value).First();                         // Get Driver ID 

            CameraProperties cameraProperties = new CameraProperties()
            {                                                                                                                                   // Build the camera Properties Object 
                Address = address,
                Name = name,
                Password = password,
                UserName = userName,
                DriverNumber = hardwareDriverId
            };

            return cameraProperties;                                                                                                            // Return Camera Properties
        }


        /// <summary>
        /// Fill all the childs from a parent item 
        /// </summary>
        /// <param name="item">Parent ConfigurationItem</param>
        /// <param name="_configApiClient">Milesotone API</param>
        private void FillAllChilds(ConfigurationItem item, ConfigApiClient _configApiClient)
        {
            FillChildren(item, _configApiClient);                                                                   // Call aux method to get the children using the API
            foreach (var child in item.Children)                                                                    // For each child
            {
                FillAllChilds(child, _configApiClient);                                                             // Recurcive call
            }
        }

        /// <summary>
        /// Auxiliar methot to fill childs from a parent item 
        /// </summary>
        /// <param name="item">Parent ConfigurationItem</param>
        /// <param name="_configApiClient">Milesotone API</param>
        private void FillChildren(ConfigurationItem item, ConfigApiClient _configApiClient)
        {
            if (!item.ChildrenFilled)                                                                               //  If children was already filled continue 
            {
                item.Children = _configApiClient.GetChildItems(item.Path);                                          //  If not get the children with an API call
                item.ChildrenFilled = true;                                                                         //  Filled flag 
            }
            if (item.Children == null)                                                                              //  If children is null
                item.Children = new ConfigurationItem[0];                                                           //  Create a new object 
        }

        /// <summary>
        /// Press Button Move Selected Hardware to a new RS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Move_Selection_Click(object sender, EventArgs e)
        {
            if (treeView_S2.SelectedNode != null)
            {
                ConfigurationItem _dest_recordingServer = treeView_S2.SelectedNode.Tag as ConfigurationItem;                // Get Dest RS 
                if (_dest_recordingServer.ItemType == "RecordingServer")                                                    // Check the items 
                {
                    checkedNodes = new List<ConfigurationItem>();                                                           // Wipe checkedNodes
                    GetCheckedNodes(treeView_S1.Nodes);                                                                     // Fill checkedNodes

                    WriteInConsole("Moving " + checkedNodes.Count + "  Hardware to " + _dest_recordingServer.DisplayName);  // Show in the console the total number of hardware to be moved 

                    Cursor.Current = Cursors.WaitCursor;                            // Change cursor to wait 
                    WriteInConsole("Start time: " + DateTime.Now.ToString());       // Show start time 
                    DateTime startTime = DateTime.Now;                              // Save start time 

                    Parallel.ForEach(checkedNodes, new ParallelOptions { MaxDegreeOfParallelism = (int)numericUpDown_MaxDegreeOfParallelism.Value },  // Call in parallel the tasks to perform the migration
                     _src =>
                     {
                         MoveHardwareToRecordingServer(_src, _dest_recordingServer);    // Move the src hardware to dest RS
                 }
                     );

                    DateTime endTime = DateTime.Now;                                // Save the end time
                    WriteInConsole(DateTime.Now.ToString());                        // Show end time

                    WriteInConsole((endTime - startTime).ToString());               // Show engaged time

                    WriteInConsole(tasksResultCache);                               // Show results 
                    tasksResultCache = "";                                          // Wipe Cache

                    Cursor.Current = Cursors.Default;                               // Restore cursor
                }
                else MessageBox.Show("Please select a Recording Server for destination");   // the detination server has not been selected 
            }
        }



        /// <summary>
        /// Get all the the hardware from the checked nodes 
        /// </summary>
        /// <param name="nodes"></param>
        private void GetCheckedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)                                            // For each node
            {
                if (node.Checked)                                                       // Is the node is checked 
                {
                    ConfigurationItem _confItem = node.Tag as ConfigurationItem;        // Get the ConfigurationItem from the node 
                    if (_confItem.ItemType == "Hardware")                               // Is the ConfigurationItem is Hardware 
                    {
                        checkedNodes.Add(_confItem);                                    // Add the Item to a the checknodes list 
                    }
                }
                if (node.Nodes.Count != 0)                                              // If node has childs 
                    GetCheckedNodes(node.Nodes);                                        // Recursive call
            }
        }



        /// <summary>
        /// Given a Hardware and a Recording Server. Add the hardware to the new server an copy all the settings. 
        /// </summary>
        /// <param name="_src"></param>
        /// <param name="_dest_recordingServer"></param>
        private void MoveHardwareToRecordingServer(ConfigurationItem _src, ConfigurationItem _dest_recordingServer)
        {
            CameraProperties cameraProperties = GetCameraProperties(_src, _configApiClient1);                                   // Get src hardware basic properties 

            ConfigurationItem[] hardwareDriverFolder = _configApiClient2.GetChildItems(_dest_recordingServer.Path + "/HardwareDriverFolder");                   // Get dest RS drivers
            var hardwarePath = (from item in hardwareDriverFolder where item.Properties[3].Value == cameraProperties.DriverNumber select item.Path).First();    // Find the driver on the dest server 

            ConfigurationItem addHarwareInfo = _configApiClient2.InvokeMethod(_dest_recordingServer, "AddHardware");            // Invoke Addhardware method on dest server 

            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareAddress").Value = cameraProperties.Address;        // set harware address
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "UserName").Value = cameraProperties.UserName;              // set user 
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "Password").Value = cameraProperties.Password;              // set password
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareDriverPath").Value = hardwarePath;                 // set driver 

            ConfigurationItem addHarwareResult = _configApiClient2.InvokeMethod(addHarwareInfo, addHarwareInfo.MethodIds[0]);   // Star Addharware method 

            String taskPath = addHarwareResult.Properties[6].Value; // TODO: Improve this USING INDEX IS NOT GOOD

            var status = _configApiClient2.GetItem(taskPath);       // Get the task status 

            while (status.Properties.Length < 3)                    // TODO: check error or success?
            {
                status = _configApiClient2.GetItem(taskPath);       // Get the task status 
                Thread.Sleep(300);   // <- Wait to success or error 
            }

            Console.WriteLine("Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - " + status.Properties[2].Value);          // TODO: Improve this  USING INDEX IS NOT GOOD --> Success message 
            //  WriteInConsole("Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - " + status.Properties[2].Value );

            tasksResultCache += "Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - " + status.Properties[2].Value + Environment.NewLine;

            if (status.Properties[2].Value == "Success")                                            // If the hardware was successfuly added, set configurations
            {

                FillAllChilds(_src, _configApiClient1);                                             // Fill all source childs 

                ConfigurationItem _dest = _configApiClient2.GetItem(status.Properties[1].Value);    // Get the added Hardware

                MatchStreams(_src, _dest);                                                          // Add streams if necesary 

                FillAllChilds(_dest, _configApiClient2);                                            // Fill dest childs. 

                CopyConfigurationItem(_src, _dest);                                                 // Copy configuration from src to dest

                SetAllChilds(_dest, _configApiClient2);                                             // Save the configuration to dest server

                tasksResultCache += "Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - Configuration copied" + Environment.NewLine;

                Console.WriteLine("Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - Configuration copied");
                //       WriteInConsole("Camera: " + cameraProperties.Name + " - IP: " + cameraProperties.Address + " - Configuration copied");
            }

        }

        /// <summary>
        ///  Match the number of camera streams
        ///  manually transverse dest the childs 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        private void MatchStreams(ConfigurationItem src, ConfigurationItem dest)
        {

            FillChildren(dest, _configApiClient2);                                                                                  // Fill dest hardware children 
            ConfigurationItem _cameras_S1 = Array.Find(src.Children, ele => ele.ItemType == "CameraFolder");                        // Get src cameraFolder 
            ConfigurationItem _cameras_S2 = Array.Find(dest.Children, ele => ele.ItemType == "CameraFolder");                       // Get dest cameraFolder 
            FillChildren(_cameras_S2, _configApiClient2);                                                                           // Fill dest Cameras 

            for (int i = 0; i < _cameras_S1.Children.Length; i++)                                                                   // For each camera 
            {   
                ConfigurationItem _camera_S1 = _cameras_S1.Children[i];                                                             // Get selected camera children 
                ConfigurationItem _streamsFolder_S1 = Array.Find(_camera_S1.Children, ele => ele.ItemType == "StreamFolder");       // Get camera StreamFolder
                ConfigurationItem _streams_S1 = Array.Find(_streamsFolder_S1.Children, ele => ele.ItemType == "Stream");            // Get Streams

                if (_streams_S1.Children.Length > 1)                                                                                // If the camera has more than 1 stream 
                {
                    ConfigurationItem _camera_S2 = _cameras_S2.Children[i];                                                         // Get the selected stream 
                    FillChildren(_camera_S2, _configApiClient2);                                                                    // Fill dest camera children 
                    ConfigurationItem _streamsFolder_S2 = Array.Find(_camera_S2.Children, ele => ele.ItemType == "StreamFolder");   // Get dest dest StreamFolder 
                    _streamsFolder_S2.Children = _configApiClient2.GetChildItems(_streamsFolder_S2.Path);                           // Fill dest camera streams children 
                    ConfigurationItem _streams_S2 = Array.Find(_streamsFolder_S2.Children, ele => ele.ItemType == "Stream");        // Get the selected stream 
                    _streams_S2.Children = _configApiClient2.GetChildItems(_streams_S2.Path);                                       // Fill dest camera stream config 
                    for (int j = 1; j < _streams_S1.Children.Length; j++)                                                           // For each stream, starting from the second 
                    {
                        ConfigurationItem addStreamInfo = _configApiClient2.InvokeMethod(_streams_S2, "AddStream");                     // Invoke AddStream Method
                        ConfigurationItem addStreamResult = _configApiClient2.InvokeMethod(addStreamInfo, addStreamInfo.MethodIds[0]);  // Execute AddStream 
                        String result = addStreamResult.Properties[0].Value; // TODO: Improve this  USING INDEX IS NOT GOOD             // Get Result
                        Console.WriteLine(result);
                    }
                }
            }
        }


        /// <summary>
        /// Copy the properties from the src ConfigurationItem to the dest ConfigurationItem
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        private void CopyConfigurationItem(ConfigurationItem src, ConfigurationItem dest)
        {
            dest.DisplayName = src.DisplayName;                                                 // Copy tha display name 
            if (src.EnableProperty != null)                                                     // If Enable Property is not null
                dest.EnableProperty.Enabled = src.EnableProperty.Enabled;                       // Copy the Enabled status
            if (src.Properties != null)                                                         // If properties is not null 
            {
                for (int i = 0; i < src.Properties.Length; i++)                                 // Iterate over the property array 
                {
                    if (src.Properties[i].IsSettable)                                           // If the properties is settable
                    {
                        dest.Properties[i].Value = src.Properties[i].Value;                     // Copy the value 
                    }
                }
            }

            if (src.Children != null)                                                           // Is the ConfigurationItem has children 
            {
                for (int i = 0; i < src.Children.Length; i++)                                   // Iterate over the chils
                {
                    CopyConfigurationItem(src.Children[i], dest.Children[i]);                   // Recursive call
                }
            }
        }




        /// <summary>
        /// Save the ConfigurationItems to the server 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_configApiClient"></param>
        private void SetAllChilds(ConfigurationItem item, ConfigApiClient _configApiClient)
        {

            try
            {

                if (!item.ItemType.Contains("Folder") &&                                    // Anything with the word "Folder" is just the tree structure, ignore 
                    !(item.ItemType == "Stream" && !(item.ItemCategory == "Item")) &&       // ignore Stream - !Items no settings there, 
                    !(item.ItemType == "ClientSettings"))                                   // Client Settings will have many paths and IDs, mean problems -> review if necesary 
                {
                    _configApiClient2.SetItem(item);                                        // Call API to store values 
                }

            }
            catch (Exception r)
            {
                Console.WriteLine("Error setting item: " + item + "error: " + r);           // Any error? 
            }

            foreach (var child in item.Children)
            {
                SetAllChilds(child, _configApiClient);                                      // Recursive call 
            }
        }


        /// <summary>
        /// Check / uncheck childs if a parent was checked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_S1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (busy) return;                           // If already iterating the tree, do nothing 
            busy = true;                                // Start iterating 
            try
            {
                checkNodes(e.Node, e.Node.Checked);     // Call aux method with parent 
            }
            finally
            {
                busy = false;                           // iteration end 
            }
        }

        /// <summary>
        /// Auxiliary method to check node childs
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void checkNodes(TreeNode node, bool check)
        {
            foreach (TreeNode child in node.Nodes)      // For child of the node node 
            {
                child.Checked = check;                  // set check 
                checkNodes(child, check);               // recursive call to the childs 
            }
        }


        /// <summary>
        /// Auxiliar method to write in the console
        /// </summary>
        /// <param name="text"></param>
        private void WriteInConsole(string text)
        {
         /*   if (InvokeRequired)
            {
                this.Invoke(new Action<string>(WriteInConsole), new object[] { text });
                return;
            }*/
            textBox_Console.Text += text + Environment.NewLine;  
        }
    }
  
    /// <summary>
    /// Camera Properties class 
    /// </summary>
    class CameraProperties
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DriverNumber { get; set; }
    }

}
