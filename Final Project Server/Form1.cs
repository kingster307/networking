using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Final_Project_Server
{
    public partial class Form1 : Form
    {
        Assembly _assembly;
        Stream _imageStream;
        IPAddress MyLocalIp;
        IPEndPoint myEP;
        Socket listeningSocket;
        int ClientCount, Accounts;
        string[,] Logins = new string[1, 4];//Username, Password, Signed on?, Display Name
        string[,] Clients = new string[1, 4];//Rows,Columns, 1 Row per Client, Username, DisplayName,I.P. Address or DC timestamp, Index in client socket array
        Socket[] ClientSocks = new Socket[1];
        string MainPath = FindMainPath();
        bool Running = false, lbReqUpdate = false;
 

        public Form1()
        {
            InitializeComponent();
        }

        private string Encrypt(string Input)
        {
            char[] pass = Input.ToCharArray();
            byte[] passbytes = new byte[Input.Length*2];
            int CharsUsed,BytesUsed;
            bool ENCComp;
            Encoder PassEncoder = new UnicodeEncoding().GetEncoder();
            PassEncoder.Convert(pass, 0, pass.Length, passbytes, 0, Input.Length*2, true, out CharsUsed, out BytesUsed, out ENCComp);
            for (int k = 0; k < passbytes.Length; k++)
            {
                passbytes[k] = byte.Parse((int.Parse(passbytes[k].ToString()) + (9 + (2*k))).ToString());
            }
            string passstring = "";
            for (int k = 0; k < passbytes.Length; k++) 
            { 
                passstring += passbytes[k].ToString()+'|'; 
            }
            return passstring+CharsUsed+'|'+BytesUsed;
        }

        private string Decrypt(string Input)
        {
            string[] pass = Input.Split('|');
            int CharsUsed,BytesUsed;
            CharsUsed = int.Parse(pass[pass.Length-2]);
            BytesUsed = int.Parse(pass[pass.Length-1]);
            char[] passchar = new char[CharsUsed];
            byte[] passbytes = new byte[BytesUsed];
            for (int k = 0; k < pass.Length-2; k++)
            {
                passbytes[k] = byte.Parse((int.Parse(pass[k]) - (9+(2*k))).ToString());
            }
            Decoder PassDecoder = new UnicodeEncoding().GetDecoder();
            int buffer;
            bool DECComp;
            PassDecoder.Convert(passbytes,0,BytesUsed,passchar,0,CharsUsed,true,out buffer, out buffer,out DECComp);
            string passstring = "";
            for (int k = 0; k < passchar.Length; k++) { passstring += passchar[k]; }
            return passstring;

        }

        private string ConvertIMGtoString(Bitmap Image)
        {
            string output;
            return output;
        }

        private static string GetLocalIPAddress()
        {

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private static string FindMainPath()
        {
            string path, name;
            path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            name = System.AppDomain.CurrentDomain.FriendlyName.Substring(0, System.AppDomain.CurrentDomain.FriendlyName.IndexOf('.'));
            while (!path.EndsWith("\\" + name))
            {
                path = System.IO.Directory.GetParent(path).ToString();
            }



            return path;
        }

        private string Recieve(Socket RecieveSocket)
        {
            byte[] buffer = new byte[1024];
            int iRx = RecieveSocket.Receive(buffer);
            char[] chars = new char[iRx];
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            System.String Recieved = new System.String(chars);
            return Recieved;
        }

        private void UpdateLoginsFile()
        {
            StreamWriter Writer = new StreamWriter(MainPath + "\\resources\\logins.bsf");
            for (int c = 0; c < Accounts ; c++)
            {
                Writer.WriteLine(Logins[c,0] + ";" + Encrypt(Logins[c,1]) + ";" + Logins[c,3]);
            }
            Writer.Close();
        }

        private void UpdateListbox()
        {
            listBox1.Items.Clear();
            int c;
            for (c = 0; c < Accounts; c++)
            {
                if (Logins[c, 2] != null)
                {
                    if (bool.Parse(Logins[c, 2]))
                    { listBox1.Items.Add(Logins[c, 3] + " is currently online"); }
                    else
                    { listBox1.Items.Add(Logins[c, 3] + " is currently offline"); }
                }
            }
            listBox1.Refresh();
            lbReqUpdate = false;
        }

        /*VALID MSGTYPES
         USR = User Message; 
            Subfields:
            Target Username
            Original Sender Username
         STS = Status Message; 
            Subfields:
                REQ: Request
                UPD: Update
            Messages:
                Connected
                Disconnected
                Login Success
                Login Fail
         SYS = System Message; 
            Subfields: 
                CAL: Call
                Messages: 
                    Request
                    Busy
                    Accept
                    Deny
                FTR: File Transfer
                Messages:
                    Request
                    Accept
                    Deny
                    File as string
                CLU: Client List Update
                Messages:
                    New Client List as String
         LOG: Login request
            Subfields:
                New: Create new account
                Old: Use created account
            Message: 
                Username;Password;DisplayName
         */

        private void Send(string MSGTYPE, string Subfield, string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(MSGTYPE + '\n' + Subfield + '\n' + Message + '\n' + System.DateTime.Now.ToString("hh:mm"));
            SenderSocket.Send(byData);
        }

        private string Login(bool CurrentAccount, string Username, string Password, Socket Sender, string DisplayName = "")
        {
            bool Sucess= false;
            string Emessage="Username not Found";
            int c=0;
            if (CurrentAccount)
            {
                for (c=0; c < Accounts; c++)
                {
                    if (Logins[c, 0] == Username)
                    {
                        if (Logins[c, 2] == true.ToString())
                        {
                            Emessage = "Already Signed in";
                            break;
                        }
                        if (Logins[c, 1] == Password)
                        {
                            Sucess = true;
                            break;
                        }
                        else 
                        { 
                            Emessage = "Incorrect password";
                            break;
                        }
                    }
                }
            }
            else
            {
                if (DisplayName != "")
                {
                    Sucess = true;
                    for (c=0; c < Accounts; c++)
                    {
                        if (Logins[c, 0] == Username)
                        {
                            Sucess = false;
                            Emessage = "Username taken";
                            break;
                        }
                        if (Logins[c, 3] == DisplayName)
                        {
                            Sucess = false;
                            Emessage = "Display Name taken";
                            break;
                        }
                    }
                }else { Emessage = "New Accounts must provide Display Name"; }
            }




            if (Sucess) 
            {
                Send("STS","UPD","Login Success",Sender);
                return "True;" + c; 
            }
            else 
            {
                Send("STS", "UPD", "Login Failure: " + Emessage, Sender);
                return Emessage; 
            }
        }

        private void HandleClient(object ClientsRowIndex)
        {
            int IndexinClients = (int)ClientsRowIndex;
            Socket Client = ClientSocks[ int.Parse(Clients[IndexinClients, 3])];
            string[] Recieved = new string[4];
            while (Client.Connected)
            {
                if (Client.Available != 0)
                {
                    Recieved = Recieve(Client).Split('\n');
                    if(Recieved[0] == "USR")
                    {
                        for(int c=0; c < Clients.GetUpperBound(0);c++)
                        {
                            if (Clients[c, 1] == Recieved[1])
                            {
                                //Found Target Username
                                Send("USR", Clients[IndexinClients, 0], Recieved[2], ClientSocks[int.Parse(Clients[c,3])]);
                            }

                        }
                    }
                    if(Recieved[0] == "STS")
                    {

                    }
                    if(Recieved[0] == "SYS")
                    {

                    }
                }
            }
            if (!Client.Connected)
            {
                Clients[ClientCount, 2] = DateTime.Now.ToString("G");
                for (int c = 0; c < Logins.GetUpperBound(0);c++ )
                {
                    if (Logins[c, 0] == Clients[IndexinClients, 0]) { Logins[c, 2] = false.ToString(); break; }
                }
                lbReqUpdate = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _assembly = Assembly.GetExecutingAssembly();
            //DEFUSRIMG
            _imageStream = _assembly.GetManifestResourceStream("DEFUSRIMG.BMP");
            

            ClientCount =0;
            Accounts=0;
            MyLocalIp = IPAddress.Parse(GetLocalIPAddress());
            if (!File.Exists(MainPath + "\\resources\\logins.bsf") || ("" == File.ReadAllText(MainPath + "\\resources\\logins.bsf", Encoding.ASCII)))
            {
                Bitmap DEFUSRIMG = new Bitmap(_imageStream);
                StreamWriter Initializer = new StreamWriter(MainPath + "\\resources\\logins.bsf");
                ImageConverter IC = new ImageConverter();
                string user, passstring, dn, usrimg;
                user = "Adam0Samuri";
                dn = "Ben";
                passstring = Encrypt("Networking18");
                usrimg = IC.ConvertToString(DEFUSRIMG);
                Initializer.Write(user+';'+passstring+';'+dn+"\r\n");
                Initializer.Close();
            }
            StreamReader Reader1 = new StreamReader(MainPath + "\\resources\\logins.bsf");
            int c = 0;
            string[] Readline = new string[3];
            //Load Logins into array
            while (Reader1.Peek() >-1)
            {
                Accounts++;
                //Resize Array
                string[,] LoginsBU = Logins;
                Logins = new string[Accounts + 1, 4];
                for (int j = 0; j < Accounts; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Logins[j, k] = LoginsBU[j, k];
                    }
                }

                Readline = Reader1.ReadLine().Split(';');
                Logins[c, 0] = Readline[0];
                Logins[c, 1] = Decrypt(Readline[1]);
                Logins[c, 2] = false.ToString();
                Logins[c, 3] = Readline[2];

                c++;
            }
            
            Reader1.Close();
            UpdateListbox();
            myEP = new IPEndPoint(MyLocalIp, 922);
            label1.Text = "LocalIP: " + MyLocalIp.ToString();
            listeningSocket = new Socket(MyLocalIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            /*
            string test = "Test1";
            test = Encrypt(test);
            test = Decrypt(test);
            lblTest.Text = test;
            lblTest.Visible = true;
            */

            lblTest.Visible = true;
            lblTest.Text = "Offline";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Running = true;
            timer1.Enabled = true;
            listeningSocket.Bind(myEP);
            listeningSocket.Listen(922);
            backgroundWorker1.RunWorkerAsync();
            lblTest.Text = "Running";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Running) { backgroundWorker1.RunWorkerAsync(); }
            if (!Running) { button1.Enabled = true; lblTest.Text = "Offline"; }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] Log = new string[2];
            if (Running)
            {
                Socket LoggingIn;
                LoggingIn = listeningSocket.Accept();
                Thread NewThread = new Thread(new ParameterizedThreadStart(HandleClient));

                //Login
                string[] Recieved = new string[4];
                Recieved = Recieve(LoggingIn).Split('\n');
                if (Recieved[0] == "LOG")
                {
                    if (Recieved[1] == "New")
                    {
                        string[] UAP = new string[3];
                        UAP = Recieved[2].Split(';');
                        Log = Login(false, UAP[0], Decrypt(UAP[1]), LoggingIn, UAP[2]).Split(';');
                        if (Log[0] == "True")
                        {
                            //Logged In
                            Clients[ClientCount, 0] = UAP[0];   //Username
                            Clients[ClientCount, 1] = UAP[2];   //DisplayName
                            Clients[ClientCount, 2] = LoggingIn.RemoteEndPoint.ToString(); // IP
                            Clients[ClientCount, 3] = ClientCount.ToString();       //Index in Socket Array
                            ClientSocks[ClientCount] = LoggingIn;

                            Logins[Accounts, 0] = UAP[0];
                            Logins[Accounts, 1] = Decrypt(UAP[1]);
                            Logins[Accounts, 2] = true.ToString();
                            Logins[Accounts, 3] = UAP[2];
                            
                            

                            ClientCount++;
                            Accounts++;

                            //Expand Client Array
                            string[,] ClientsBU = Clients;
                            Clients = new string[ClientCount+1,4];
                            for (int c = 0; c < ClientCount; c++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    Clients[c, k] = ClientsBU[c,k];
                                }
                            }

                            //Expand Login Array
                            string[,] LoginsBU = Logins;
                            Logins = new string[Accounts + 1, 4];
                            for (int c = 0; c < Accounts; c++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    Logins[c, k] = LoginsBU[c, k];
                                }
                            }

                            Array.Resize(ref ClientSocks,ClientCount+1);
                            UpdateLoginsFile();
                            lbReqUpdate = true;
                            NewThread.Start(ClientCount - 1);
                        }
                        else { LoggingIn.Close(); }
                    
                    }
                    else if (Recieved[1] == "Old")
                    {
                        string[] UAP = new string[2];
                        UAP = Recieved[2].Split(';');
                        Log = Login(true, UAP[0], Decrypt(UAP[1]), LoggingIn).Split(';');
                        if (Log[0] == "True")
                        {
                            //Logged In
                            Clients[ClientCount, 0] = UAP[0];   //Username
                            Clients[ClientCount, 1] = Logins[int.Parse(Log[1]),3];   //DisplayName
                            Clients[ClientCount, 2] = LoggingIn.RemoteEndPoint.ToString(); // IP
                            Clients[ClientCount, 3] = ClientCount.ToString();       //Index in Socket Array
                            ClientSocks[ClientCount] = LoggingIn;

                            Logins[int.Parse(Log[1]), 2] = true.ToString();

                            ClientCount++;

                            //Expand Client Array
                            string[,] ClientsBU = Clients;
                            Clients = new string[ClientCount + 2, 4];
                            for (int c = 0; c < ClientCount; c++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    Clients[c, k] = ClientsBU[c, k];
                                }
                            }

                            Array.Resize(ref ClientSocks, ClientCount + 1);
                            lbReqUpdate = true;
                            

                            NewThread.Start(ClientCount - 1);

                        }else{ LoggingIn.Close();}

                    }
                }    
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Running = false;
            lblTest.Text = "Stopping";
            backgroundWorker1.CancelAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lbReqUpdate) { UpdateListbox(); }
        }
    }
}
