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
using System.Drawing;
using System.IO;

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

        private bool _busy = false;                                  // Axiliary boolean to safe check treeView items 

        // Join the dark theme 
        readonly Color TEXTBACKCOLOR = System.Drawing.ColorTranslator.FromHtml("#252526");
        readonly Color BACKCOLOR = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
        readonly Color INFOCOLOR = System.Drawing.ColorTranslator.FromHtml("#1E7AD4");
        readonly Color MESSAGECOLOR = System.Drawing.ColorTranslator.FromHtml("#86A95A");
        readonly Color DEBUGCOLOR = System.Drawing.ColorTranslator.FromHtml("#DCDCAA");
        readonly Color ERRORCOLOR = System.Drawing.ColorTranslator.FromHtml("#B0572C");

        public Main()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.textBox_Console.BackColor = TEXTBACKCOLOR;
            this.BackColor = BACKCOLOR;
            this.groupBox1.BackColor = TEXTBACKCOLOR;
            this.groupBox2.BackColor = TEXTBACKCOLOR;

            VideoOS.Platform.SDK.Environment.Initialize();

            _configApiClient1 = new ConfigApiClient();
            _configApiClient2 = new ConfigApiClient();

            UI.Icons.Init();                                    // TreeView Decorator
            treeView_S1.ImageList = UI.Icons.IconListBlack;
            treeView_S2.ImageList = UI.Icons.IconListBlack;

            numericUpDown_MaxDegreeOfParallelism.Value = 10;

            /*
            // FAST DEBUG
            textBox_Address_S1.Text = "10.1.0.192";
            textBox_Domain__S1.Text = "MEX-LAB";
            textBox_User_S1.Text = "SGIU";
            textBox_Password_S1.Text = "Milestone1$";


            textBox_Address_S2.Text = "172.19.190.152";
            textBox_Domain_S2.Text = ".";
            textBox_User_S2.Text = "Administrator";
            textBox_Password_S2.Text = "Milestone1$";
            
            //Button_Connect_S1_Click(null, null); 
            //Button_Connect_S2_Click(null, null); 
            */
        }

        /// <summary>
        /// Login into the a Management Server (C-CODE)
        /// </summary>
        /// <param name="uri">Server URI</param>
        /// <param name="nc">Credentials</param>
        private void Login(Uri uri, NetworkCredential nc, ConfigApiClient _configApiClient, Label toolStripStatusLabel, Button button_Connect)
        {
            WriteInConsole("Connecting to: " + uri + ".", LogType.message);
            VideoOS.Platform.SDK.Environment.AddServer(false, uri, nc, true);                    // Add the server to the environment 
            try
            {
                VideoOS.Platform.SDK.Environment.Login(uri, IntegrationId, IntegrationName, Version, ManufacturerName);     // attempt to login 
            }
            catch (ServerNotFoundMIPException snfe)
            {
                WriteInConsole("Server not found: " + snfe.Message + ".", LogType.error);
            }
            catch (InvalidCredentialsMIPException ice)
            {
                WriteInConsole("Invalid credentials: " + ice.ToString() + ".", LogType.error);
            }
            catch (Exception e)
            {
                WriteInConsole("Other error connecting to: " + e.ToString() + ".", LogType.error);
            }


            string _serverAddress = uri.ToString();                           // server URI
            int _serverPort = 80;                                             // Server port - TODO: Harcoded port 
            bool _corporate = true;                                           // c-code - TODO: Harcoded type

            _configApiClient.ServerAddress = _serverAddress;                  // set API Client
            _configApiClient.Serverport = _serverPort;
            _configApiClient.ServerType = _corporate
                                              ? ConfigApiClient.ServerTypeEnum.Corporate
                                              : ConfigApiClient.ServerTypeEnum.Arcus;

            try
            {
                _configApiClient.Initialize();                                    // Initialize API

            }
            catch (Exception ex)
            {
                WriteInConsole("API Error: " + ex, LogType.error);
            }
            WriteInConsole("Initializing API: " + _configApiClient.Token + ".", LogType.debug);
            WriteInConsole("Initializing API: " + _configApiClient.Connected + ".", LogType.info);

            if (_configApiClient.Connected)
            {
                WriteInConsole("Initializing API: " + _configApiClient.ServerAddress + ".", LogType.info);

                toolStripStatusLabel.Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabel.Text = "Logged on";                    // If connected change status label 
                });
                button_Connect.Invoke((MethodInvoker)delegate
                {
                    button_Connect.Text = "Disconnect";
                });
                WriteInConsole("Connection to : " + uri + " established.", LogType.message);
            }
            else
            {
                toolStripStatusLabel.Text = "Error logging on";             // If not connected change status label
                WriteInConsole("Connection to : " + uri + " failed.", LogType.error);

            }


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
                Uri uri = new Uri("http://" + textBox_Address_S1.Text);                                                 // Fetch URI
                String user = textBox_User_S1.Text;                                                             // Fetch user 
                String pass = textBox_Password_S1.Text;                                                         // Fetch pass 
                String domain = textBox_Domain__S1.Text;                                                        // Fetch domain 
                NetworkCredential nc = new NetworkCredential(user, pass, domain);                               // Build credentials

                Task taskA = new Task(() =>
                {
                    Login(uri, nc, _configApiClient1, toolStripStatusLabel_S1, Button_Connect_S1);                  // Call Login method
                    PopulateTreeView(_configApiClient1, treeView_S1);                                               // Show items 
                });
                taskA.Start();
            }
        }

        /// <summary>
        /// Push Connect Destiny Server Button Action 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_S2_Click(object sender, EventArgs e)
        {

            if (ValidateServerInputTexts(textBox_Address_S2.Text, textBox_User_S2.Text, textBox_Password_S2.Text, textBox_Domain_S2.Text))
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

                    Task taskA = new Task(() =>
                    {
                        Login(uri, nc, _configApiClient2, toolStripStatusLabel_S2, Button_Connect_S2);                  // Call Login method
                    });
                    taskA.Start();

                    taskA.ContinueWith(task =>
                    {
                        PopulateTreeView(_configApiClient2, treeView_S2);                                               // Show items 
                    });

                }
            }
        }

        private bool ValidateServerInputTexts(string address, string user, string pass, string domain)
        {
            bool valid = true;

            valid = !string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass) && !string.IsNullOrEmpty(domain);
            if (valid == false) WriteInConsole("Complete all the values", LogType.error);
            return valid;
        }

        /// <summary>
        /// Populate the TreeView using the API client 
        /// </summary>
        /// <param name="_configApiClient"></param>
        /// <param name="treeView"></param>
        private void PopulateTreeView(ConfigApiClient _configApiClient, TreeView treeView)
        {

            var server = _configApiClient.GetItem("/");                                             // Start from the root
            WriteInConsole(server.DisplayName + ": Loading Configuration... ", LogType.info);

            TreeNode serverTn = new TreeNode(server.DisplayName)                                    // Create a new node 
            {
                Tag = server,               // Store the ConfurationItem in the tag 
                Checked = false             // Is not checked 
            };

            treeView.Invoke((MethodInvoker)delegate
            {
                treeView.Nodes.Clear();
                treeView.Nodes.Add(serverTn);                                                           // Add the root node 
                serverTn.Nodes.AddRange(AddChildren("/", _configApiClient));                            // Add root children
            });

            WriteInConsole(server.DisplayName + ": Configuration Loaded. ", LogType.info);
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
            Parallel.ForEach(_configApiClient.GetChildItems(node), child =>
            {

                if (_showItemsType.Contains(child.ItemType))                                        //  Apply Filter 
                {
                    TreeNode tn = new TreeNode(child.DisplayName)                                   //  Create a TreeNode 
                    {
                        Tag = child,                                                                // Store the ConfigurationItem in the tag
                        Checked = false,                                                            // not checked 
                        ImageIndex = UI.Icons.GetImageIndex(child.ItemType),                        // Decorator Icon
                        SelectedImageIndex = UI.Icons.GetImageIndex(child.ItemType)                 // Decorator Selected Icon 
                    };
                    children.Add(tn);
                    //  Add node to childen list 
                    tn.Nodes.AddRange(AddChildren(child.Path, _configApiClient));                   //  Recursive call with the child as parent 
                }
            });
            return children.ToArray();                                                              // Return the children list
        }



        /// <summary>
        /// Get the needed information to add a camera to a the server 
        /// Information to gather: Name, Address, UserName, HardwareDriverId, Password 
        /// </summary>
        /// <param name="_item">ConfigurationItem</param>
        /// <param name="_configApiClient">Milestone API</param>
        /// <returns></returns>
        private HardwareProperties GetHardwareProperties(ConfigurationItem _item, ConfigApiClient _configApiClient)
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

            HardwareProperties cameraProperties = new HardwareProperties()
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
            ((Button)sender).Enabled = false;


            if (treeView_S2.SelectedNode != null)
            {
                ConfigurationItem _dest_recordingServer = treeView_S2.SelectedNode.Tag as ConfigurationItem;                // Get Destination Recording Server

                if (_dest_recordingServer.ItemType == ItemTypes.RecordingServer)                                                    // Check the items 
                {
                    IList<ConfigurationItem> checkedNodes = GetCheckedNodes(treeView_S1.Nodes, ItemTypes.Hardware);                                                                     // Fill checkedNodes

                    WriteInConsole("Add " + checkedNodes.Count + " Hardware to " + _dest_recordingServer.DisplayName + " Recording Server", LogType.message);  // Show in the console the total number of hardware to be moved 

                    Cursor.Current = Cursors.WaitCursor;                            // Change cursor to wait 
                    DateTime startTime = DateTime.Now;                              // Save start time 


                    var listOfTasks = new List<Task>();
                    int i = 0;
                    foreach (ConfigurationItem checkedNode in checkedNodes)
                    {
                        listOfTasks.Add(new Task(() => MoveHardwareToRecordingServer(checkedNode, _dest_recordingServer, checkedNodes.Count, ++i)));
                    }

                    int _maxDegreeOfParallelism = (int)numericUpDown_MaxDegreeOfParallelism.Value;

                    Task tasks = StartAndWaitAllThrottledAsync(listOfTasks, _maxDegreeOfParallelism, -1).ContinueWith(result =>
                    {
                        WriteInConsole("Adding Hardware Complete", LogType.message);
                        PopulateTreeView(_configApiClient2, treeView_S2);               // Refresh treeView})

                    });
                    Cursor.Current = Cursors.Default;                               // Restore cursor
                }
                else WriteInConsole("Please select the destination Recording Server", LogType.error);
            }
            else WriteInConsole("Please select the destination Recording Server", LogType.error);

            ((Button)sender).Enabled = true;
        }



        /// <summary>
        /// Get checked nodes aux methods 
        /// </summary>
        /// <param name="nodes"></param>
        private void GetCheckedNodesAux(TreeNodeCollection nodes, String itemType, ref IList<ConfigurationItem> checkedNodes)
        {
            foreach (TreeNode node in nodes)                                            // For each node
            {
                if (node.Checked)                                                       // Is the node is checked 
                {
                    ConfigurationItem _confItem = node.Tag as ConfigurationItem;        // Get the ConfigurationItem from the node 
                    if (_confItem.ItemType == itemType)                                 // Is the ConfigurationItem is Hardware 
                    {
                        checkedNodes.Add(_confItem);                                    // Add the Item to a the checknodes list 
                    }
                }
                if (node.Nodes.Count != 0)                                              // If node has childs 
                    GetCheckedNodesAux(node.Nodes, itemType, ref checkedNodes);         // Recursive call
            }
        }

        /// <summary>
        /// Get all the the  from the checked nodes 
        /// </summary>
        /// <param name="nodes"></param>
        private IList<ConfigurationItem> GetCheckedNodes(TreeNodeCollection nodes, String itemType)
        {
            IList<ConfigurationItem> checkedNodes = checkedNodes = new List<ConfigurationItem>();
            GetCheckedNodesAux(nodes, itemType, ref checkedNodes);
            return checkedNodes;
        }



        /// <summary>
        /// Given a Hardware and a Recording Server. Add the hardware to the new server an copy all the settings. 
        /// </summary>
        /// <param name="_src"></param>
        /// <param name="_dest_recordingServer"></param>
        private void MoveHardwareToRecordingServer(ConfigurationItem _src, ConfigurationItem _dest_recordingServer, int count, int v)
        {
            HardwareProperties hardwareProperties = GetHardwareProperties(_src, _configApiClient1);                                   // Get src hardware basic properties 

            ConfigurationItem[] hardwareDriverFolder = _configApiClient2.GetChildItems(_dest_recordingServer.Path + "/HardwareDriverFolder");                   // Get dest RS drivers
            var hardwarePath = (from item in hardwareDriverFolder where Array.Find(item.Properties, ele => ele.Key == "Number").Value == hardwareProperties.DriverNumber select item.Path).First();    // Find the driver on the dest server 

            WriteInConsole(hardwareProperties.Name + ": Adding Hardware. (" + v + "/" + count + ")", LogType.message);

            ConfigurationItem addHarwareInfo = _configApiClient2.InvokeMethod(_dest_recordingServer, "AddHardware");            // Invoke Addhardware method on dest server 

            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareAddress").Value = hardwareProperties.Address;        // set harware address
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "UserName").Value = hardwareProperties.UserName;              // set user 
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "Password").Value = hardwareProperties.Password;              // set password
            Array.Find(addHarwareInfo.Properties, ele => ele.Key == "HardwareDriverPath").Value = hardwarePath;                 // set driver 

            ConfigurationItem addHarwareResult = _configApiClient2.InvokeMethod(addHarwareInfo, "AddHardware");   // Star AddHarware method 

            String taskPath = Array.Find(addHarwareResult.Properties, ele => ele.Key == "Path").Value;      // Get thask path 


            ConfigurationItem status = _configApiClient2.GetItem(taskPath);       // Get the task status 

            String _state = Array.Find(status.Properties, ele => ele.Key == "State").Value; // Get state

            while (!(_state == "Success" || _state == "Error"))
            {
                status = _configApiClient2.GetItem(taskPath);                   // Get the task status 
                Thread.Sleep(300);                                              // <- Wait 300ms
                _state = Array.Find(status.Properties, ele => ele.Key == "State").Value; // Get the new state
            }

            if (_state == "Error")
            {
                WriteInConsole("Error Adding Hardware (" + v + "/" + count + ") " + hardwareProperties.Name + ". " + Array.Find(status.Properties, ele => ele.Key == "ErrorText").Value, LogType.error);
            }

            else
            {
                {
                    WriteInConsole(hardwareProperties.Name + ": Getting Hardware Configurations.", LogType.debug);
                    FillAllChilds(_src, _configApiClient1);                                             // Fill all source childs 
                    ConfigurationItem _dest = _configApiClient2.GetItem(status.Properties[1].Value);    // Get the added Hardware

                    WriteInConsole(hardwareProperties.Name + ": Adding Streams .", LogType.info);

                    MatchStreams(_src, _dest);                                                          // Add streams if necesary 

                    WriteInConsole(hardwareProperties.Name + ": Preparing destination.", LogType.info);
                    FillAllChilds(_dest, _configApiClient2);                                            // Fill dest childs. 

                    WriteInConsole(hardwareProperties.Name + ": Coping configuration.", LogType.info);

                    CopyConfigurationItem(_src, _dest);                                                 // Copy configuration from src to dest
                    WriteInConsole(hardwareProperties.Name + ": Saving configuration.", LogType.info);
                    SetAllChilds(_dest, _configApiClient2);                                             // Save the configuration to dest server
                    WriteInConsole(hardwareProperties.Name + ": Success. (" + v + " / " + count + ")", LogType.message);
                }


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
        private void TreeView_S1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_busy) return;                           // If already iterating the tree, do nothing 
            _busy = true;                                // Start iterating 
            try
            {
                CheckNodes(e.Node, e.Node.Checked);     // Call aux method with parent node
            }
            finally
            {
                _busy = false;                           // iteration end 
            }
        }

        /// <summary>
        /// Auxiliary method to check node childs
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void CheckNodes(TreeNode node, bool check)
        {
            foreach (TreeNode child in node.Nodes)      // For each child of the node 
            {
                child.Checked = check;                  // set check 
                CheckNodes(child, check);               // recursive call to the childs 
            }
        }





        private void AddRoleToServer(ConfigurationItem _role, ConfigurationItem _dest_roleFolder)
        {

            try                                                                                             // try to add the role to the dest server
            {
                WriteInConsole(_role.DisplayName + ": Adding Role.", LogType.message);

                ConfigurationItem result = _configApiClient2.InvokeMethod(_dest_roleFolder, "AddRole");     // Invoke AddRole Method

                //Fill the basic role information with the src role 
                Array.Find(result.Properties, ele => ele.Key == "Name").Value = Array.Find(_role.Properties, ele => ele.Key == "Name").Value;
                Array.Find(result.Properties, ele => ele.Key == "Description").Value = Array.Find(_role.Properties, ele => ele.Key == "Description").Value;
                Array.Find(result.Properties, ele => ele.Key == "DualAuthorizationRequired").Value = Array.Find(_role.Properties, ele => ele.Key == "DualAuthorizationRequired").Value;
                Array.Find(result.Properties, ele => ele.Key == "MakeUsersAnonymousDuringPTZSession").Value = Array.Find(_role.Properties, ele => ele.Key == "MakeUsersAnonymousDuringPTZSession").Value;
                Array.Find(result.Properties, ele => ele.Key == "AllowMobileClientLogOn").Value = Array.Find(_role.Properties, ele => ele.Key == "AllowMobileClientLogOn").Value;
                Array.Find(result.Properties, ele => ele.Key == "AllowSmartClientLogOn").Value = Array.Find(_role.Properties, ele => ele.Key == "AllowSmartClientLogOn").Value;
                Array.Find(result.Properties, ele => ele.Key == "AllowWebClientLogOn").Value = Array.Find(_role.Properties, ele => ele.Key == "AllowWebClientLogOn").Value;

                ConfigurationItem InvokeResult = _configApiClient2.InvokeMethod(result, "AddRole");        // Call invoke method with the properties 
                WriteInConsole(_role.DisplayName + ": " + Array.Find(InvokeResult.Properties, ele => ele.Key == "State").Value, LogType.info);
                _dest_roleFolder.ChildrenFilled = false;                                                   // Children have changed 
            }
            catch (Exception exp)
            {

                WriteInConsole(_role.DisplayName + ": Error Adding Role. " + exp.Message, LogType.error);
            }
        }




        /// <summary>z
        /// Click on Move Roles Buttom 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MoveRoles_Click(object sender, EventArgs e)
        {

            IList<ConfigurationItem> rolesNodes = GetCheckedNodes(treeView_S1.Nodes, "Role");                       //  Get List of checked Roles 

            WriteInConsole("Move " + rolesNodes.Count + " roles.", LogType.message);

            ConfigurationItem _dest_Server = treeView_S2.TopNode.Tag as ConfigurationItem;                          //  Get destination server root
            FillChildren(_dest_Server, _configApiClient2);                                                          //  Fill childrens 
            ConfigurationItem _dest_roleFolder = Array.Find(_dest_Server.Children, ele => ele.ItemType == "RoleFolder"); //  Get dest Role Folder
            FillChildren(_dest_roleFolder, _configApiClient2);                                                           //  Fill Role Folder childrens

            foreach (ConfigurationItem _role in rolesNodes)                                                         // For each cheched role
            {
                FillChildren(_role, _configApiClient1);                    // Fill current role children 

                if (Array.Find(_role.Properties, ele => ele.Key == "RoleType").Value != "Adminstrative")            // If the role is not Adminstrative -- Misspell in the API -> Report ?
                {
                    AddRoleToServer(_role, _dest_roleFolder);
                    CopyPermissionsToDestRole(_role); // Async call for each hardware
                    SetOveralpermissions(_role); // Async call for each Security Name Space
                }
                AddUsersToRole(_role);

            }
        }

        private void AddUsersToRole(ConfigurationItem _role)
        {
            try
            {
                WriteInConsole(_role.DisplayName + ": Add users to role", LogType.info);

                ConfigurationItem _userFolder = Array.Find(_role.Children, ele => ele.ItemType == "UserFolder");                                    // Get user folder from source role 
                FillChildren(_userFolder, _configApiClient1);                                                                                       // Fill Children 

                ConfigurationItem _role_s2 = Find_S1_Role_in_S2(_role);                                                                             // Find Role en dest server 
                FillChildren(_role_s2, _configApiClient2);                                                                                          // Fill Children 

                ConfigurationItem _userFolder_s2 = Array.Find(_role_s2.Children, ele => ele.ItemType == "UserFolder");                              // Get user folder from dest role

                foreach (ConfigurationItem _user in _userFolder.Children)                                                                           // For each user in userFolder 
                {
                    WriteInConsole(_role.DisplayName + ": Adding " + _user.DisplayName + " users to role", LogType.info);
                    // If the user is BasicUser
                    if (Array.Find(_user.Properties, ele => ele.Key == "IdentityType").Value == "BasicUser")
                    {
                        ConfigurationItem _user_s2 = Find_S1_User_in_S2(_user);                                                                         // Find the user in dest server 
                        ConfigurationItem result = _configApiClient2.InvokeMethod(_userFolder_s2, "AddRoleMember");                                     // Invoke AddRoleMember in dest server, UserFolder
                        Array.Find(result.Properties, ele => ele.Key == "Sid").Value = Array.Find(_user_s2.Properties, ele => ele.Key == "Id").Value;   // Set as Sid the Id of the basicUser 
                        ConfigurationItem result2 = _configApiClient2.InvokeMethod(result, "AddRoleMember");                                            // Invoke method again to save. 
                        WriteInConsole(_role.DisplayName + ": Adding " + _user.DisplayName + " users to role: " + Array.Find(result2.Properties, ele => ele.Key == "State").Value, LogType.message); // Show result

                    }
                    // If Windows User Just add the ID and the method will do the rest 
                    else
                    {
                        ConfigurationItem result = _configApiClient2.InvokeMethod(_userFolder_s2, "AddRoleMember");                                                                     // Invoke Method in dest server
                        Array.Find(result.Properties, ele => ele.Key == "Sid").Value = Array.Find(_user.Properties, ele => ele.Key == "Sid").Value;                                     // Set the Sid from the src user
                        ConfigurationItem result2 = _configApiClient2.InvokeMethod(result, "AddRoleMember");                                                                            // Call method again to execute 
                        WriteInConsole(_role.DisplayName + ": Adding " + _user.DisplayName + " users to role: " + Array.Find(result2.Properties, ele => ele.Key == "State").Value, LogType.message); // Show result

                    }
                }
            }
            catch (Exception exp)
            {
                WriteInConsole(_role.DisplayName + ": " + exp.Message, LogType.error); // Show 
            }
        }

        private void SetOveralpermissions(ConfigurationItem _role)
        {
            try
            {
                WriteInConsole(_role.DisplayName + ": Setting Overall permissions to role", LogType.message);                                   // Set Overall permissions to the role 

                ConfigurationItem _role_s2 = Find_S1_Role_in_S2(_role);                                                                         // Find Role in dest server 
                FillChildren(_role_s2, _configApiClient2);                                                                                      // Fill role childrens 

                ConfigurationItem _result_s1 = _configApiClient1.InvokeMethod(_role, "ChangeOverallSecurityPermissions");                       // Invoke ChangeOverallSecurityPermissions in S1
                ConfigurationItem _result_s2 = _configApiClient2.InvokeMethod(_role_s2, "ChangeOverallSecurityPermissions");                    // Invoke ChangeOverallSecurityPermissions in S2


                var listOfTasks = new List<Task>();
                foreach (ValueTypeInfo vti in _result_s1.Properties[0].ValueTypeInfos)                                                          // Iterate all the SecurityNamespace avilables 
                {
                    listOfTasks.Add(new Task(() =>
                    {
                        WriteInConsole(_role.DisplayName + ": Setting " + vti.Name + "Role Overall permissions", LogType.debug);                                      // Show result - Success? No errors, but not confiration from API
                        _result_s1.Properties[0].Value = vti.Value;                                                                                 // Set the SecurityNamespace to the S1 invoke method 
                        _result_s2.Properties[0].Value = vti.Value;                                                                                 // Set the SecurityNamespace to the S2 invoke method 

                        ConfigurationItem _result_2_s1 = _configApiClient1.InvokeMethod(_result_s1, "ChangeOverallSecurityPermissions");            // Invoke Method to get the permissions in S1
                        ConfigurationItem _result_2_s2 = _configApiClient2.InvokeMethod(_result_s2, "ChangeOverallSecurityPermissions");            // Invoke Method to get the permissions in S2

                        _result_2_s2.Properties = _result_2_s1.Properties;                                                                          // Copy the permissions from S2 to S1 

                        ConfigurationItem _result_3_s2 = _configApiClient2.InvokeMethod(_result_2_s2, "ChangeOverallSecurityPermissions");          // Invoke method again to save the changes in S2

                    }));
                }

                int _maxDegreeOfParallelism = (int)numericUpDown_MaxDegreeOfParallelism.Value;

                Task tasks = StartAndWaitAllThrottledAsync(listOfTasks, _maxDegreeOfParallelism, -1).ContinueWith(result =>
                {
                    WriteInConsole(_role.DisplayName + ": Setting Overall permissions to role. Complete", LogType.message);                                   // Set Overall permissions to the role 
                                                                                                                                                              //   RefreshDestTreeView();               // Refresh treeView

                });
            }
            catch (Exception exp)
            {
                WriteInConsole(_role.DisplayName + ": Error Setting Role Overall permissions : " + exp.Message, LogType.error);                                  // Show error 
            }
        }


        /// <summary>
        /// Find the passed user in the destination server 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private ConfigurationItem Find_S1_User_in_S2(ConfigurationItem user)
        {
            String _userType = Array.Find(user.Properties, ele => ele.Key == "IdentityType").Value;
            String _userName = Array.Find(user.Properties, ele => ele.Key == "AccountName").Value;
            String _domain = Array.Find(user.Properties, ele => ele.Key == "Domain").Value;

            ConfigurationItem _user = FindBasicUser(_userName, _configApiClient2);

            return _user;
        }


        /// <summary>
        /// Given a BasicUser name, and a APIclient returns the user ConfigurationItem from the BasicUserFolder 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="configApiClient"></param>
        /// <returns>ConfigurationItem BasicUser</returns>
        private ConfigurationItem FindBasicUser(string userName, ConfigApiClient configApiClient)
        {
            ConfigurationItem _server = configApiClient.GetItem("/"); //treeView_S2.TopNode.Tag as ConfigurationItem;
            FillChildren(_server, configApiClient);

            ConfigurationItem _BasicUserFolder = Array.Find(_server.Children, ele => ele.ItemType == ItemTypes.BasicUserFolder);
            _BasicUserFolder.ChildrenFilled = false;
            FillChildren(_BasicUserFolder, _configApiClient2);

            ConfigurationItem _basicUser = Array.Find(_BasicUserFolder.Children, ele => ele.DisplayName == userName);
            if (_basicUser == null) throw new Exception("BasicUser " + userName + " was not found in " + _server.DisplayName);
            else return _basicUser;

        }

        /// <summary>
        /// Finde a Role in the destination server
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private ConfigurationItem Find_S1_Role_in_S2(ConfigurationItem role)
        {
            String _srcRoleName = Array.Find(role.Properties, ele => ele.Key == "Name").Value;

            ConfigurationItem _role = FindRoleByName(_srcRoleName, _configApiClient2);

            return _role;
        }

        /// <summary>
        /// Given a Role name, and a APIclient returns the user ConfigurationItem from the Role Folder
        /// </summary>
        /// <param name="srcRoleName"></param>
        /// <param name="configApiClient"></param>
        /// <returns>ConfigurationItem Role</returns>
        private ConfigurationItem FindRoleByName(string srcRoleName, ConfigApiClient configApiClient)
        {
            ConfigurationItem _server = configApiClient.GetItem("/");
            FillChildren(_server, configApiClient);

            ConfigurationItem _roleFolder = Array.Find(_server.Children, ele => ele.ItemType == ItemTypes.RoleFolder);
            _roleFolder.ChildrenFilled = false;
            FillChildren(_roleFolder, configApiClient);

            foreach (ConfigurationItem _role in _roleFolder.Children)
            {
                String _roleName = Array.Find(_role.Properties, ele => ele.Key == "Name").Value;

                if (String.Equals(_roleName, srcRoleName))
                {
                    return _role;
                }
            }
            throw new Exception("Role " + srcRoleName + " was not found in " + _server.DisplayName);
        }

        /// <summary>
        /// Set the permision a role has on the device 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="device"></param>
        /// <param name="permissions"></param>
        private void SetDevicepermissions(ConfigurationItem role, ConfigurationItem device, Property[] permissions)
        {
            ConfigurationItem result = _configApiClient2.InvokeMethod(device, "ChangeSecurityPermissions");

            Array.Find(result.Properties, ele => ele.Key == "UserPath").Value = role.Path;
            ConfigurationItem InvokeResult = _configApiClient2.InvokeMethod(result, "ChangeSecurityPermissions");

            permissions[0] = InvokeResult.Properties[0];
            InvokeResult.Properties = permissions;

            ConfigurationItem InvokeResult2 = _configApiClient2.InvokeMethod(InvokeResult, "ChangeSecurityPermissions");
        }


        /// <summary>
        /// Find a device in the destination Server
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private ConfigurationItem Find_S1_Device_in_S2(ConfigurationItem device)
        {
            ConfigurationItem hardware = GetDeviceHardware(device, _configApiClient1);

            String _srcAddress = Array.Find(hardware.Properties, ele => ele.Key == "Address").Value;
            String _srcName = Array.Find(device.Properties, ele => ele.Key == "Name").Value;

            ConfigurationItem _device = FindDeviceByAddress(_srcAddress, _srcName, _configApiClient2);

            return _device;
        }



        /// <summary>
        /// Given a Hardware IP address, a Device Name and API client return the CofigurationItem of the Device 
        /// </summary>
        /// <param name="srcAddress"></param>
        /// <param name="srcName"></param>
        /// <param name="_configApiClient"></param>
        /// <returns></returns>
        private ConfigurationItem FindDeviceByAddress(String srcAddress, String srcDeviceName, ConfigApiClient configApiClient)
        {
            ConfigurationItem _server = configApiClient.GetItem("/");
            FillChildren(_server, configApiClient);

            ConfigurationItem _recordingFolder = Array.Find(_server.Children, ele => ele.ItemType == "RecordingServerFolder");
            FillChildren(_recordingFolder, configApiClient);

            foreach (ConfigurationItem _recordingServer in _recordingFolder.Children)
            {
                FillChildren(_recordingServer, configApiClient);
                ConfigurationItem _hardwareFolder = Array.Find(_recordingServer.Children, ele => ele.ItemType == "HardwareFolder");
                _hardwareFolder.ChildrenFilled = false;
                FillChildren(_hardwareFolder, configApiClient);

                foreach (ConfigurationItem _hardware in _hardwareFolder.Children)
                {
                    FillChildren(_hardware, configApiClient);
                    String _hardwareAddress = Array.Find(_hardware.Properties, ele => ele.Key == "Address").Value;

                    if (String.Equals(_hardwareAddress, srcAddress))
                    {
                        ConfigurationItem[] _devicesFolders = Array.FindAll(_hardware.Children, ele => devicesFolders.Contains(ele.ItemType));

                        foreach (ConfigurationItem _devicesFolder in _devicesFolders)
                        {
                            FillChildren(_devicesFolder, configApiClient);
                            foreach (ConfigurationItem _device in _devicesFolder.Children)
                            {
                                FillChildren(_device, configApiClient);

                                string _cameraName = Array.Find(_device.Properties, ele => ele.Key == "Name").Value;

                                if (String.Equals(_cameraName, srcDeviceName))
                                {
                                    return _device;
                                }
                            }

                        }
                        //throw new Exception("Device " + srcDeviceName + " was not found in the Hardware " + _hardware.DisplayName + " in the server " + _server.DisplayName);
                        return null;
                    }
                }
            }
            //throw new Exception("Hardware with IP " + srcAddress + " was not found in the server " + _server.DisplayName);
            return null;

        }


        private ConfigurationItem GetDeviceHardware(ConfigurationItem device, ConfigApiClient _configApiClient)
        {
            ConfigurationItem _deviceFolder = _configApiClient.GetItem(device.ParentPath);
            ConfigurationItem _hardware = _configApiClient.GetItem(_deviceFolder.ParentPath);
            return _hardware;
        }


        private Property[] GetRolePermissions(ConfigurationItem role, ConfigurationItem camera)
        {
            ConfigurationItem result = _configApiClient1.InvokeMethod(camera, "ChangeSecurityPermissions");
            Array.Find(result.Properties, ele => ele.Key == "UserPath").Value = role.Path;
            ConfigurationItem InvokeResult = _configApiClient1.InvokeMethod(result, "ChangeSecurityPermissions");
            return InvokeResult.Properties;
        }




        private void button_MoveBasicUsers_Click(object sender, EventArgs e)
        {
            /// PLEASE PLEASE PLEASE REFACTOR THIS 
            /// 
            IList<ConfigurationItem> _src_BasicUser = GetCheckedNodes(treeView_S1.Nodes, "BasicUser");

            ConfigurationItem _dest_Server = treeView_S2.TopNode.Tag as ConfigurationItem;
            FillChildren(_dest_Server, _configApiClient2);

            ConfigurationItem _dest_BasicUserFolder = Array.Find(_dest_Server.Children, ele => ele.ItemType == "BasicUserFolder");
            FillChildren(_dest_BasicUserFolder, _configApiClient2);

            WriteInConsole("Moving " + _src_BasicUser.Count + " basic user(s).", LogType.message);

            foreach (ConfigurationItem _basicUser in _src_BasicUser)
            {
                WriteInConsole(_basicUser.DisplayName + ": Moving basic user.", LogType.info);

                FillAllChilds(_basicUser, _configApiClient1);

                ConfigurationItem result = _configApiClient2.InvokeMethod(_dest_BasicUserFolder, "AddBasicUser");
                Array.Find(result.Properties, ele => ele.Key == "Name").Value = Array.Find(_basicUser.Properties, ele => ele.Key == "Name").Value;
                Array.Find(result.Properties, ele => ele.Key == "Description").Value = Array.Find(_basicUser.Properties, ele => ele.Key == "Description").Value;
                Array.Find(result.Properties, ele => ele.Key == "CanChangePassword").Value = Array.Find(_basicUser.Properties, ele => ele.Key == "CanChangePassword").Value;
                Array.Find(result.Properties, ele => ele.Key == "ForcePasswordChange").Value = "True"; // I could find the password on the src server, a new one is needed       //Array.Find(_basicUser.Properties, ele => ele.Key == "ForcePasswordChange").Value;
                Array.Find(result.Properties, ele => ele.Key == "Password").Value = "Abcd12345!!";
                Array.Find(result.Properties, ele => ele.Key == "Status").Value = Array.Find(_basicUser.Properties, ele => ele.Key == "Status").Value;

                try
                {
                    ConfigurationItem InvokeResult = _configApiClient2.InvokeMethod(result, "AddBasicUser");
                    WriteInConsole(_basicUser + ": " + Array.Find(InvokeResult.Properties, ele => ele.Key == "State").Value, LogType.message);
                }
                catch (Exception ex)
                {
                    WriteInConsole(_basicUser + ": " + ex.Message, LogType.error);
                }
            }

            WriteInConsole("Moving " + _src_BasicUser.Count + " basic user(s) Complete.", LogType.message);


            PopulateTreeView(_configApiClient2, treeView_S2);               // Refresh treeView})

        }


        private void button_MoveGroups_Click(object sender, EventArgs e)
        {
            WriteInConsole("Move Groups", LogType.message);

            List<Task> listOfTasks = new List<Task>();

            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.CameraGroup), ItemTypes.CameraGroupFolder);
            }));
            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.MicrophoneGroup), ItemTypes.MicrophoneGroupFolder);
            }));
            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.SpeakerGroup), ItemTypes.SpeakerGroupFolder);
            }));
            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.MetadataGroup), ItemTypes.MetadataGroupFolder);
            }));
            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.InputEventGroup), ItemTypes.InputEventGroupFolder);
            }));
            listOfTasks.Add(new Task(() =>
            {
                MoveGroup(GetCheckedNodes(treeView_S1.Nodes, ItemTypes.OutputGroup), ItemTypes.OutputGroupFolder);
            }));


            int _maxDegreeOfParallelism = (int)numericUpDown_MaxDegreeOfParallelism.Value;

            Task tasks = StartAndWaitAllThrottledAsync(listOfTasks, _maxDegreeOfParallelism, -1).ContinueWith(result =>
            {
                WriteInConsole("Moving Groups Complete", LogType.message);
                PopulateTreeView(_configApiClient2, treeView_S2);               // Refresh treeView})


            });

        }

        private void MoveGroup(IList<ConfigurationItem> groups, String type)
        {
            if (groups.Count > 0)
            {
                WriteInConsole("Moving " + groups.Count + " " + groups[0].ItemType, LogType.message);
                foreach (ConfigurationItem group in groups)
                {
                    //ConfigurationItem _dest_Server = treeView_S2.TopNode.Tag as ConfigurationItem;
                    ConfigurationItem _dest_Server = _configApiClient2.GetItem("/");

                    FillChildren(_dest_Server, _configApiClient2);
                    ConfigurationItem groupFolder = Array.Find(_dest_Server.Children, ele => ele.ItemType == type);

                    try
                    {
                        WriteInConsole(group.DisplayName + ": Creating Group", LogType.info);
                        CreateGroup(groupFolder, group);
                    }
                    catch (Exception ex)
                    {
                        WriteInConsole(group.DisplayName + ": " + ex.Message, LogType.error);
                    }

                    try
                    {
                        WriteInConsole(group.DisplayName + ": Populate Group", LogType.info);
                        PopulateGroup(group, type, type.Replace("Group", ""));
                    }
                    catch (Exception ex)
                    {
                        WriteInConsole(group.DisplayName + ": " + ex.Message, LogType.error);
                    }
                }
                WriteInConsole("Moving " + groups.Count + " " + groups[0].ItemType + " Complete", LogType.message);
            }
        }

        private void PopulateGroup(ConfigurationItem group, String typeGroupFolder, string typeFolder)
        {

            FillChildren(group, _configApiClient1);
            ConfigurationItem _device_Folder_S1 = Array.Find(group.Children, ele => ele.ItemType == typeFolder);
            FillChildren(_device_Folder_S1, _configApiClient1);


            //ConfigurationItem s2 = treeView_S2.TopNode.Tag as ConfigurationItem;
            ConfigurationItem s2 = _configApiClient2.GetItem("/");
            FillChildren(s2, _configApiClient2);
            ConfigurationItem _groupFolder = Array.Find(s2.Children, ele => ele.ItemType == typeGroupFolder);
            _groupFolder.ChildrenFilled = false;
            FillChildren(_groupFolder, _configApiClient2);

            ConfigurationItem _deviceGroupFolder = Array.Find(_groupFolder.Children, ele => ele.DisplayName == group.DisplayName);
            if (_deviceGroupFolder != null)
            {
                FillChildren(_deviceGroupFolder, _configApiClient2);

                ConfigurationItem _deviceFolder = Array.Find(_deviceGroupFolder.Children, ele => ele.ItemType == typeFolder);

                foreach (ConfigurationItem _device_in_group_s1 in _device_Folder_S1.Children)
                {
                    ConfigurationItem _hardware_in_s1 = GetDeviceHardware(_device_in_group_s1, _configApiClient1);
                    String name_in_s1 = Array.Find(_device_in_group_s1.Properties, ele => ele.Key == "Name").Value;
                    String address_in_s1 = Array.Find(_hardware_in_s1.Properties, ele => ele.Key == "Address").Value;
                    ConfigurationItem device_in_s2 = FindDeviceByAddress(address_in_s1, name_in_s1, _configApiClient2);

                    if (device_in_s2 != null)
                    {
                        try
                        {
                            ConfigurationItem result = _configApiClient2.InvokeMethod(_deviceFolder, "AddDeviceGroupMember");
                            Array.Find(result.Properties, ele => ele.Key == "ItemSelection").Value = device_in_s2.Path;
                            ConfigurationItem result2 = _configApiClient2.InvokeMethod(result, "AddDeviceGroupMember");
                            WriteInConsole(group.DisplayName + ": Device " + _device_in_group_s1.DisplayName + " added to " + _deviceFolder.DisplayName, LogType.info);
                        }
                        catch (Exception ex)
                        {
                            WriteInConsole(group.DisplayName + ": Device " + _device_in_group_s1.DisplayName + " - " + ex.Message, LogType.error);
                        }

                    }
                    else
                        WriteInConsole(group.DisplayName + ": " + name_in_s1 + " was not found", LogType.error);

                }
            }
        }

        private void CreateGroup(ConfigurationItem groupFolder, ConfigurationItem group)
        {
            ConfigurationItem result = _configApiClient2.InvokeMethod(groupFolder, "AddDeviceGroup");
            Array.Find(result.Properties, ele => ele.Key == "GroupName").Value = Array.Find(group.Properties, ele => ele.Key == "Name").Value;
            Array.Find(result.Properties, ele => ele.Key == "GroupDescription").Value = Array.Find(group.Properties, ele => ele.Key == "Description").Value;
            ConfigurationItem InvokeResult = _configApiClient2.InvokeMethod(result, "AddDeviceGroup");
            // return InvokeResult.Properties;
        }


        private void CopyPermissionsToDestRole(ConfigurationItem _role)
        {
            try                                                                                          // Scan all the camaras and copy the permissions to the dest role 
            {
                WriteInConsole(_role.DisplayName + ": Set Role permissions to role", LogType.message);
                ConfigurationItem _role_s2 = Find_S1_Role_in_S2(_role);
                ConfigurationItem _src_Server = treeView_S1.TopNode.Tag as ConfigurationItem;                                                   // Get Src Server 

                WriteInConsole(_role.DisplayName + ": Granting Permissions", LogType.info);

                FillChildren(_src_Server, _configApiClient1);                                                                                   // Fill Src Server clildrens
                ConfigurationItem _recordingServerFolder = Array.Find(_src_Server.Children, ele => ele.ItemType == "RecordingServerFolder");    // Get Src Recording Server Folder
                FillChildren(_recordingServerFolder, _configApiClient1);                                                                        // Fill Src Recording Server Folder

                foreach (ConfigurationItem _recodingServer in _recordingServerFolder.Children)                                                  // For each Recording Server in the folder
                {
                    FillChildren(_recodingServer, _configApiClient1);                                                                           // Fill the Recording Server Children 
                    ConfigurationItem _hardwareFolder = Array.Find(_recodingServer.Children, ele => ele.ItemType == "HardwareFolder");          // Get Hardware Folder
                    FillChildren(_hardwareFolder, _configApiClient1);                                                                           // Fill the Hardware Folder 

                    WriteInConsole(_role.DisplayName + ": Set Hardware Permissions.  Started", LogType.message);
                    var listOfTasks = new List<Task>();
                    foreach (ConfigurationItem _hardware in _hardwareFolder.Children)
                    {
                        listOfTasks.Add(new Task(() => SetRolePermisionOnHardware(_hardware, _role, _role_s2)));
                    }

                    int _maxDegreeOfParallelism = (int)numericUpDown_MaxDegreeOfParallelism.Value;

                    Task tasks = StartAndWaitAllThrottledAsync(listOfTasks, _maxDegreeOfParallelism, -1).ContinueWith(result =>
                    {
                        WriteInConsole(_role.DisplayName + ": Setting Hardware Permissions.  Complete", LogType.message);
                    });
                }
                WriteInConsole(_role.DisplayName + ": Success Setting Role Permissions : ", LogType.message);

            }
            catch (Exception exp)
            {
                WriteInConsole(_role.DisplayName + ": Error Setting Role Permissions : " + exp.Message, LogType.error);
            }
        }

        private void SetRolePermisionOnHardware(ConfigurationItem _hardware, ConfigurationItem _role, ConfigurationItem _role_s2)
        {
            WriteInConsole(_role.DisplayName + ": Setting permission on Hardware: " + _hardware.DisplayName, LogType.info);

            FillChildren(_hardware, _configApiClient1);                                                                             // Fill the Hardware Folder
            ConfigurationItem[] _devicesFolders = Array.FindAll(_hardware.Children, ele => devicesFolders.Contains(ele.ItemType));  // Get the List of Devices Folders (Cams/Mics/In/Out/Meta)

            foreach (ConfigurationItem _deviceFolder in _devicesFolders)                                                            // For each Device Folder
            {
                FillChildren(_deviceFolder, _configApiClient1);                                                                     // Fill the Device Folder Children 
                foreach (ConfigurationItem _device in _deviceFolder.Children)
                {
                    WriteInConsole(_role.DisplayName + ": Setting permissions on device: " + _device.DisplayName, LogType.debug);
                    Property[] _permissions = GetRolePermissions(_role, _device);                                                    // Get the permissions on the Src server for the role
                    ConfigurationItem _device_s2 = Find_S1_Device_in_S2(_device);                                                   // Find the Device in the Dest Server
                    if (_device_s2 != null) SetDevicepermissions(_role_s2, _device_s2, _permissions);                                                         // Set the permision
                    else
                    {
                        WriteInConsole(_role.DisplayName + ": " + _device.DisplayName + " Device not found", LogType.error);
                    }
                }
            }

        }

        enum LogType
        {
            debug,
            message,
            info,
            error,
        }



        //static bool debug = true;
        /// <summary>
        /// Auxiliar method to write in the console
        /// </summary>
        /// <param name="text"></param>
        private void WriteInConsole(string text, LogType type)
        {

            // if (type != LogType.debug)

            {

                textBox_Console.Invoke((MethodInvoker)delegate
                {
                    Color _color;
                    switch (type)
                    {
                        case LogType.debug:
                            _color = DEBUGCOLOR;
                            break;
                        case LogType.message:
                            _color = MESSAGECOLOR;
                            break;
                        case LogType.info:
                            _color = INFOCOLOR;
                            break;
                        case LogType.error:
                            _color = ERRORCOLOR;
                            break;
                        default:
                            _color = Color.White;
                            break;
                    }



                    textBox_Console.SelectionStart = textBox_Console.TextLength;
                    textBox_Console.SelectionLength = 0;

                    textBox_Console.SelectionColor = _color;
                    textBox_Console.AppendText(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ": " + text + Environment.NewLine);
                    textBox_Console.SelectionColor = textBox_Console.ForeColor;

                    textBox_Console.SelectionStart = textBox_Console.TextLength;
                    textBox_Console.ScrollToCaret();
                });
            }
        }

        private void treeView_S1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                PopulateTreeView(_configApiClient1, treeView_S1);
                e.Handled = true;
            }

        }

        private void treeView_S2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                PopulateTreeView(_configApiClient2, treeView_S2);
                e.Handled = true;
            }
        }

        private void button_SaveLogs_Click(object sender, EventArgs e)
        {
            String _path = @"c:\ProgramData\Milestone\MigrationToolV2";
            System.IO.Directory.CreateDirectory(_path);


            string s1 = (_configApiClient1 != null) ? _configApiClient1.GetItem("/").DisplayName : "ServerA";
            string s2 = (_configApiClient2 != null) ? _configApiClient2.GetItem("/").DisplayName : "ServerB";



            _path += "\\" + s1 + " - " + s2 + DateTime.Now.ToString(" (MMddyy-HHmmss)") + ".log";

            string _text = textBox_Console.Text;
            File.WriteAllText(_path, _text);

        }


        /// <summary>
        /// Devices Folders Filter
        /// </summary>
        private static Collection<String> devicesFolders = new Collection<string>()
                                                        {
                                                            ItemTypes.CameraFolder,
                                                            ItemTypes.MicrophoneFolder,
                                                            ItemTypes.OutputFolder,
                                                            ItemTypes.InputEventFolder,
                                                            ItemTypes.MetadataFolder
                                                        };

        /// <summary>
        /// ViewTree Filter 
        /// </summary>
        private static Collection<String> _showItemsType = new Collection<string>()
                                                        {
                                                           ItemTypes.System,
                                                           ItemTypes.RecordingServerFolder,
                                                           ItemTypes.RecordingServer,
                                                           ItemTypes.HardwareFolder,
                                                           ItemTypes.Hardware,

                                                           ItemTypes.Role,
                                                           ItemTypes.RoleFolder,


                                                           ItemTypes.CameraGroup,
                                                           ItemTypes.CameraGroupFolder,

                                                           ItemTypes.MicrophoneGroup,
                                                           ItemTypes.MicrophoneGroupFolder,

                                                           ItemTypes.SpeakerGroup,
                                                           ItemTypes.SpeakerGroupFolder,

                                                           ItemTypes.MetadataGroup,
                                                           ItemTypes.MetadataGroupFolder,

                                                           ItemTypes.InputEventGroup,
                                                           ItemTypes.InputEventGroupFolder,

                                                           ItemTypes.OutputGroup,
                                                           ItemTypes.OutputGroupFolder,


                                                           ItemTypes.UserFolder,
                                                           ItemTypes.User,

                                                           ItemTypes.BasicUserFolder,
                                                           ItemTypes.BasicUser,

                                                        };


        /// <summary>
        /// Thread execution control, limit the parallelism 
        /// </summary>
        /// <param name="tasksToRun"></param>
        /// <param name="maxTasksToRunInParallel"></param>
        /// <param name="timeoutInMilliseconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAndWaitAllThrottledAsync(IEnumerable<Task> tasksToRun, int maxTasksToRunInParallel, int timeoutInMilliseconds, CancellationToken cancellationToken = new CancellationToken())
        {
            List<Task> tasks = tasksToRun.ToList(); // Convert to a list of tasks so that we don't enumerate over it multiple times needlessly.
            using (var throttler = new SemaphoreSlim(maxTasksToRunInParallel))
            {
                var postTaskTasks = new List<Task>();

                // Have each task notify the throttler when it completes so that it decrements the number of tasks currently running.
                tasks.ForEach(t => postTaskTasks.Add(t.ContinueWith(tsk => throttler.Release())));

                // Start running each task.
                foreach (var task in tasks)
                {
                    // Increment the number of tasks currently running and wait if too many are running.
                    await throttler.WaitAsync(timeoutInMilliseconds, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                    task.Start();
                }

                // Wait for all of the provided tasks to complete.
                // We wait on the list of "post" tasks instead of the original tasks, otherwise there is a potential race condition where the throttlers using block is exited before some Tasks have had their "post" action completed, which references the throttler, resulting in an exception due to accessing a disposed object.
                await Task.WhenAll(postTaskTasks.ToArray());
            }
        }
    }


    /// <summary>
    /// Camera Properties class 
    /// </summary>
    class HardwareProperties
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DriverNumber { get; set; }
    }

}

