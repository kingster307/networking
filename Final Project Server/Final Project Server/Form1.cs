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

namespace Final_Project_Server
{
    public partial class Form1 : Form
    {

        IPAddress MyLocalIp;
        IPEndPoint myEP;
        Socket listeningSocket;
        SocketAsyncEventArgs SockEventarg = new SocketAsyncEventArgs();
        int ClientCount, Accounts, ThreadCount=0;
        string[,] Logins = new string[1, 5];//Username, Password, Signed on?, Display Name, Pictures
        string[,] Clients = new string[1, 4];//Rows,Columns, 1 Row per Client, Username, DisplayName,I.P. Address or DC timestamp, Index in client socket array
        Socket[] ClientSocks = new Socket[1];
        StackObject[] HeldMSGs = new StackObject[1];
        StackObject FreeClientSockInd = new StackObject();
        
        string MainPath = FindMainPath();
        bool Running = false, lbReqUpdate = false, LogBool;
        Thread MSGChecker, THLogins;
        

 

        public Form1()
        {
            InitializeComponent();
        }

        public class StackObject
        {

            string[] items = new string[1];

            public void Push(string Input)
            {
                items[items.Length - 1] = Input;
                Array.Resize(ref items, items.Length + 1);
            }

            public string Pop()
            {
                if (items.Length > 1)
                {
                    string Out = items[items.Length - 2];
                    items[items.Length - 2] = null;
                    Array.Resize(ref items, items.Length - 1);
                    return Out;
                }
                else
                {
                    return null;
                }
            }
            public bool Contains(string s)
            {
                return items.Contains(s);
            }
            public int Count
            {
                get
                {
                    return items.Length - 1;
                }
            }
        }

        private void SockEventarg_Completed(object sender, EventArgs e)
        {
            LogBool = true;

        }

        private void CheckforIncoming()
        {
            while (ClientCount > 0)
            {
                for (int c = 0; c < ClientSocks.Length - 1; c++)
                {
                    try
                    {
                        if (ClientSocks[c].Available != 0)
                        {
                            for (int k = 0; k < Clients.GetUpperBound(0); k++)
                            {
                                if (int.Parse(Clients[k, 3]) == c)
                                {
                                    while (ThreadCount > 9) { }
                                    Thread NewThread = new Thread(new ParameterizedThreadStart(HandleIncoming));
                                    NewThread.Start(k);
                                    break;
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException) { }
                }
            }

        }

        private void HandleIncoming(object ClientsRowIndex)
        {
            int IndexinClients = (int)ClientsRowIndex, TargetLoginsIndex = 0, TargetClientIndex = 0;
            try
            {
                Socket Client = ClientSocks[int.Parse(Clients[IndexinClients, 3])];
                string[] Recieved, Splitter;
                string Sending = "";
                bool TargetOnline = false;
                Recieved = Recieve(Client).Split('\a');
                if (Recieved.Length == 4)
                {
                    for (int k = 0; k < Logins.GetUpperBound(0); k++)
                    {
                        if (Logins[k, 3] == Recieved[1])
                        {
                            //Target Account
                            TargetLoginsIndex = k;
                            if (bool.Parse(Logins[k, 2]))
                            {
                                //Online
                                TargetOnline = true;
                            }
                            else
                            {
                                //Offline
                                TargetOnline = false;
                            }
                            break;
                        }
                    }
                    for (int c = 0; c < Clients.GetUpperBound(0); c++)
                    {
                        if (Clients[c, 1] == Recieved[1])
                        {
                            //Found Target Username
                            TargetClientIndex = c;
                            break;
                        }
                    }

                    if (Recieved[0] == "USR")
                    {
                        Sending = "USR" + '\a' + Clients[IndexinClients, 1] + '\a' + Recieved[2] + '\a' + int.Parse(Clients[TargetClientIndex, 3]);
                    }
                    if (Recieved[0] == "STS")
                    {
                        if (Recieved[1] == "UPD")
                        {
                            if (Recieved[2] == "Offline")
                            {
                                for (int c = 0; c < Logins.GetUpperBound(0); c++)
                                {
                                    if (Logins[c, 3] == Clients[IndexinClients, 1])
                                    {
                                        ClientCount--;
                                        Logins[c, 2] = false.ToString();
                                        //Client Socks Handler
                                        Client.Close();
                                        ClientSocks[int.Parse(Clients[IndexinClients, 3])].Close();
                                        Array.Resize(ref ClientSocks, ClientSocks.Length - 1);
                                        ClientSocks[ClientSocks.Length - 1] = null;
                                        FreeClientSockInd.Push(Clients[IndexinClients, 3]);
                                        //Clients Handler
                                        Clients[IndexinClients, 0] = null;
                                        string[,] ClientsBU = Clients;
                                        Clients = new string[ClientCount + 1, 4];
                                        for (int k = 0, d = 0; k < Clients.GetUpperBound(0); k++, d++)
                                        {
                                            if (ClientsBU[d, 0] == null) { d++; }
                                            Clients[k, 0] = ClientsBU[d, 0];
                                        }
                                        break;
                                    }
                                }
                                ClientListUpdate();
                            }
                        }
                    }
                    if (Recieved[0] == "SYS")
                    {
                        if (Recieved[1] == "FTR")
                        {
                            Splitter = Recieved[2].Split('\v');
                            for (int k = 0; k < Logins.GetUpperBound(0); k++)
                            {
                                if (Logins[k, 3] == Splitter[0])
                                {
                                    //Target Account
                                    TargetLoginsIndex = k;
                                    if (bool.Parse(Logins[k, 2]))
                                    {
                                        //Online
                                        TargetOnline = true;
                                    }
                                    else
                                    {
                                        //Offline
                                        TargetOnline = false;
                                    }
                                    break;
                                }
                            }
                            for (int c = 0; c < Clients.GetUpperBound(0); c++)
                            {
                                if (Clients[c, 1] == Splitter[0])
                                {
                                    //Found Target Username
                                    Sending = "SYS" + '\a' + "FTR" + '\a' + Clients[IndexinClients, 1] + '\v' + Splitter[1] + '\v' + Splitter[2] + '\a' + int.Parse(Clients[c, 3]);
                                }
                                else
                                {
                                    Sending = "SYS" + '\a' + "FTR" + '\a' + Clients[IndexinClients, 1] + '\f' + Splitter[1] + '\f' + Splitter[2];
                                }
                            }
                        }
                    }
                    Splitter = Sending.Split('\a');
                    if (TargetOnline && Splitter.Length ==4)
                    {
                        Send(Splitter[0], Splitter[1], Splitter[2], ClientSocks[int.Parse(Splitter[3])]);
                    }
                    else if(Splitter.Length == 3) 
                    {
                        HeldMSGs[TargetLoginsIndex].Push(Splitter[0] + '\v' + Splitter[1] + '\v' + Splitter[2]);
                    }
                }
            }
            catch (ArgumentNullException) { }
        }
            

        private void HandleNewCons()
        {
            string[] Log = new string[2];
            while (Running)
            {
                SockEventarg = new SocketAsyncEventArgs();
                LogBool = false;
                Socket LoggingIn;    
                listeningSocket.AcceptAsync(SockEventarg);
                SockEventarg.Completed += SockEventarg_Completed;
                while ( Running && !LogBool) {}
                if (!Running) { break; }
                //Login
                LoggingIn = SockEventarg.AcceptSocket;
                string[] Recieved = new string[4];
                Recieved = Recieve(LoggingIn).Split('\a');
                if (Recieved[0] == "LOG")
                {
                    if (Recieved[1] == "New")
                    {
                        string[] UAP = new string[3];
                        UAP = Recieved[2].Split(';');
                        Log = Login(false, UAP[0], Decrypt(UAP[1]), LoggingIn, UAP[2], UAP[3]).Split(';');
                        if (Log[0] == "True")
                        {
                            //Logged In
                            Clients[ClientCount, 0] = UAP[0];   //Username
                            Clients[ClientCount, 1] = UAP[2];   //DisplayName
                            Clients[ClientCount, 2] = LoggingIn.RemoteEndPoint.ToString(); // IP
                            if (FreeClientSockInd.Count == 0) { Clients[ClientCount, 3] = ClientCount.ToString(); } //Index in Socket Array
                            else { Clients[ClientCount, 3] = FreeClientSockInd.Pop(); }
                            ClientSocks[int.Parse(Clients[ClientCount, 3])] = LoggingIn;

                            Logins[Accounts, 0] = UAP[0];
                            Logins[Accounts, 1] = Decrypt(UAP[1]);
                            Logins[Accounts, 2] = true.ToString();
                            Logins[Accounts, 3] = UAP[2];
                            Logins[Accounts, 4] = UAP[3];



                            ClientCount++;
                            Accounts++;

                            //Expand Client Array
                            string[,] ClientsBU = Clients;
                            Clients = new string[ClientCount + 1, 4];
                            for (int c = 0; c < ClientCount; c++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    Clients[c, k] = ClientsBU[c, k];
                                }
                            }

                            //Expand Login Array
                            string[,] LoginsBU = Logins;
                            Logins = new string[Accounts + 1, 5];
                            for (int c = 0; c < Accounts; c++)
                            {
                                for (int k = 0; k < 5; k++)
                                {
                                    Logins[c, k] = LoginsBU[c, k];
                                }
                            }

                            Array.Resize(ref ClientSocks, ClientCount + 1 + FreeClientSockInd.Count);
                            Array.Resize(ref HeldMSGs, Accounts);
                            HeldMSGs[HeldMSGs.Length - 1] = new StackObject();
                            UpdateLoginsFile();
                            ClientListUpdate();
                            lbReqUpdate = true;
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
                            Clients[ClientCount, 1] = Logins[int.Parse(Log[1]), 3];   //DisplayName
                            Clients[ClientCount, 2] = LoggingIn.RemoteEndPoint.ToString(); // IP
                            if (FreeClientSockInd.Count == 0) { Clients[ClientCount, 3] = ClientCount.ToString(); } //Index in Socket Array
                            else { Clients[ClientCount, 3] = FreeClientSockInd.Pop(); }
                            ClientSocks[int.Parse(Clients[ClientCount, 3])] = LoggingIn;

                            Logins[int.Parse(Log[1]), 2] = true.ToString();

                            ClientCount++;

                            //Expand Client Array
                            string[,] ClientsBU = Clients;
                            Clients = new string[ClientCount + 1, 4];
                            for (int c = 0; c < ClientCount; c++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    Clients[c, k] = ClientsBU[c, k];
                                }
                            }



                            Array.Resize(ref ClientSocks, ClientCount + 1 + FreeClientSockInd.Count);
                            
                            ClientListUpdate();
                            lbReqUpdate = true;

                            while (HeldMSGs[int.Parse(Log[1])].Count > 0)
                            {
                                string[] Split = HeldMSGs[int.Parse(Log[1])].Pop().Split('\v');
                                Send(Split[0],Split[1],Split[2],ClientSocks[ClientCount-1]);
                            }
                        }
                        else { LoggingIn.Close(); }
                    }
                }
            }
            SockEventarg.Dispose();
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

        private string IMGtoString(Bitmap Input)
        {
            byte[] IMGbyte;
            string IMGString="";
            ImageConverter IMGCon = new ImageConverter();
            IMGbyte = (byte[])IMGCon.ConvertTo(Input, typeof(byte[]));
            for (int k = 0; k < IMGbyte.Length; k++)
            {
                IMGString += IMGbyte[k].ToString() + '|';
            }
            IMGString.Substring(0,IMGString.Length-2);
            return IMGString;
        }

        private Bitmap StringtoIMG(string Input)
        {
            string[] IMGStringArray = Input.Split('|');
            byte[] IMGbyte = new byte[IMGStringArray.Length-1];
            Image image;
            for(int c=0;c<IMGbyte.Length;c++)
            {
                IMGbyte[c] = byte.Parse(IMGStringArray[c]);
            }
            using (var MemStream = new MemoryStream(IMGbyte, 0, IMGbyte.Length))
            {
                image = Image.FromStream(MemStream, true);
            }
            Bitmap bitmap = new Bitmap(image);
            return bitmap;
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
            if(RecieveSocket != null)
            {
            if (RecieveSocket.Connected)
            {
                string[] Splitter;
                string FullREC = "";
                byte[] buffer = new byte[RecieveSocket.Available];
                int iRx = RecieveSocket.Receive(buffer);
                char[] chars = new char[iRx];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                System.String Recieved = new System.String(chars);
                FullREC += Recieved;
                Splitter = FullREC.Split('\a');
                while (Splitter[Splitter.Length - 1] != "MSGEND")
                {
                    try
                    {
                        buffer = new byte[RecieveSocket.Available];
                        iRx = RecieveSocket.Receive(buffer);
                        chars = new char[iRx];
                        charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                        Recieved = new System.String(chars);
                        FullREC += Recieved;
                        Splitter = FullREC.Split('\a');
                    }
                    catch (ObjectDisposedException) { }
                    catch (SocketException) { }
                }
                return FullREC;
            }
                else { return "Socket not connected"; }
            }
            else { return "Socket not connected"; }
            /*if (Splitter.Length > 1 && Splitter[0] != "")
            { FullREC = "STS" + '\n' + "UPD" + '\n' + "Offline" + '\n' + "MSGEND"; }*/
            
        }

        private void ClientListUpdate()
        {
            string ClientList="";
            for (int c = 0; c < Logins.GetUpperBound(0); c++)
            {
                ClientList += Logins[c, 3] +';'+ Logins[c, 4] +';'+ Logins[c, 2] + '\r'; 
            }
            for(int c=0;c<ClientSocks.Length-1;c++)
            {
                if (!FreeClientSockInd.Contains(c.ToString()))
                {
                    Send("SYS", "CLU", ClientList, ClientSocks[c]);
                }
            }
        }

        private void UpdateLoginsFile()
        {
            StreamWriter Writer = new StreamWriter(MainPath + "\\resources\\logins.bsf");
            for (int c = 0; c < Accounts ; c++)
            {
                Writer.WriteLine(Logins[c,0] + ";" + Encrypt(Logins[c,1]) + ";" + Logins[c,3]+";"+Logins[c,4]);
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
         ** means not yet implemented
         USR = User Message; 
            Subfields:
            Target Username
            Original Sender Username
         STS = Status Message; 
            Subfields:
                **REQ: Request
                UPD: Update
            Messages:
                Connected
                Offline
                Login Success
                Login Fail
                Server Shutting Down
         SYS = System Message; 
            Subfields: 
              **CAL: Call
                Messages: 
                    Request
                    Busy
                    Accept
                    Deny
                FTR: File Transfer
                Messages:
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
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(MSGTYPE + '\a' + Subfield + '\a' + Message + '\a' + "MSGEND");
            /*if (byData.Length > 1024)
            {
                //MultiPart Message

                int c = 1;
                byte[] SendPart = new byte[1024];
                string headerstring = "";
                byte[] HeaderBytes;

                for (c = 1; ((c - 1) * 1024) < byData.Length; c++)
                {
                    headerstring = "P" + c.ToString() + '\v';
                    HeaderBytes = System.Text.Encoding.ASCII.GetBytes(headerstring);
                    for (int k = 0; k < HeaderBytes.Length; k++)
                    {
                        SendPart[k] = HeaderBytes[k];
                    }
                    for (int k = 0; k < (1024-HeaderBytes.Length); k++) 
                    { 
                        SendPart[k+HeaderBytes.Length] = byData[k]; 
                    }
                    SenderSocket.Send(SendPart);
                }
            }
            else {*/
            try { SenderSocket.Send(byData); }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }//}
            
        }

        private string Login(bool CurrentAccount, string Username, string Password, Socket Sender, string DisplayName = "", string Picture ="")
        {
            bool Sucess= false;
            string Emessage="";
            int c=0;
            if (CurrentAccount)
            {
                Emessage = "Username not Found";
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
                            Emessage = Logins[c, 3];
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
                if (DisplayName != ""&& Picture !="")
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
                }else if(DisplayName =="")
                { Emessage = "New Accounts must provide Display Name"; }
                else if(Picture =="")
                { Emessage = "New Accounts must provide Picture"; }
            }
            if (Sucess) 
            {
                Send("STS","UPD","Login Success;" + Emessage,Sender);
                return "True;" + c; 
            }
            else 
            {
                Send("STS", "UPD", "Login Failure: " + Emessage, Sender);
                return Emessage; 
            }
        }
        /*
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
                for (int c = 0; c < Clients.GetUpperBound(0); c++)
                {
                    if (Logins[c, 0] == Clients[IndexinClients,0])
                    {
                        Logins[c, 2] = false.ToString();
                        lbReqUpdate = true;
                    }
                }
            }
        }
        */
        private void Form1_Load(object sender, EventArgs e)
        {
            ClientCount =0;
            Accounts=0;
            MyLocalIp = IPAddress.Parse(GetLocalIPAddress());
            if (!File.Exists(MainPath + "\\resources\\logins.bsf") || ("" == File.ReadAllText(MainPath + "\\resources\\logins.bsf", Encoding.ASCII)))
            {
                Bitmap DEFUSRIMAGE = Properties.Resources.USRIMG;
                
                StreamWriter Initializer = new StreamWriter(MainPath + "\\resources\\logins.bsf");
                string user, passstring, dn, IMGString;
                user = Properties.Resources.DEFUSN;
                dn = Properties.Resources.DEFDN;
                passstring = Encrypt(Properties.Resources.DEFPASS);
                IMGString = IMGtoString(DEFUSRIMAGE);
                Initializer.Write(user+';'+passstring+';'+dn+';'+ IMGString +"\r\n");
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
                Logins = new string[Accounts + 1, 5];
                for (int j = 0; j < Accounts; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        Logins[j, k] = LoginsBU[j, k];
                    }
                }

                Readline = Reader1.ReadLine().Split(';');
                Logins[c, 0] = Readline[0];
                Logins[c, 1] = Decrypt(Readline[1]);
                Logins[c, 2] = false.ToString();
                Logins[c, 3] = Readline[2];
                Logins[c, 4] = Readline[3];

                c++;
            }
            
            Reader1.Close();
            UpdateListbox();
            myEP = new IPEndPoint(MyLocalIp, 922);
            label1.Text = "LocalIP: " + MyLocalIp.ToString();
            HeldMSGs = new StackObject[Accounts];
            for (int k = 0; k < HeldMSGs.Length; k++) { HeldMSGs[k] = new StackObject(); }

                /*
                string test = "Test1";
                test = Encrypt(test);
                test = Decrypt(test);
                lblTest.Text = test;
                lblTest.Visible = true;
                */

                lblTest.Visible = true;
            lblTest.Text = "Offline";

            //if (Logins[1, 4] != null) {pictureBox1.Image = StringtoIMG(Logins[1, 4]);}
            THLogins = new Thread(new ThreadStart(HandleNewCons));
            MSGChecker = new Thread(new ThreadStart(CheckforIncoming));

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (listeningSocket == null) { listeningSocket = new Socket(MyLocalIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp); }
            Running = true;
            listeningSocket.Bind(myEP);
            listeningSocket.Listen(922);
            //backgroundWorker1.RunWorkerAsync();
            THLogins.Start();
            timer1.Enabled = true;
            lblTest.Text = "Running";
        }
        /*
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
                        Log = Login(false, UAP[0], Decrypt(UAP[1]), LoggingIn, UAP[2],UAP[3]).Split(';');
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
                            Logins[Accounts, 4] = UAP[3];
                            
                            

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
                            Logins = new string[Accounts + 1, 5];
                            for (int c = 0; c < Accounts; c++)
                            {
                                for (int k = 0; k < 5; k++)
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
        */
        private void button2_Click(object sender, EventArgs e)
        {
            Running = false;
            lblTest.Text = "Stopping";
            THLogins.Abort();
            listeningSocket.Close();
            //backgroundWorker1.CancelAsync();
            for (int c = 0; c < Logins.GetUpperBound(0); c++)
            {
                Logins[c, 2] = false.ToString();
            }
                for (int c = 0; c < ClientSocks.Length - 1; c++)
                {

                    Send("STS", "UPD", "Server Shutting Down", ClientSocks[c]);
                    ClientCount--;
                    ClientSocks[c].Close();
                }
            //while (backgroundWorker1.IsBusy) { }
            listeningSocket = new Socket(MyLocalIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            THLogins = new Thread(new ThreadStart(HandleNewCons));
            lblTest.Text = "Offline";
            timer1.Enabled = false;
            Clients = new string[1, 4];//Rows,Columns, 1 Row per Client, Username, DisplayName,I.P. Address or DC timestamp, Index in client socket array
            ClientSocks = new Socket[1];
            UpdateListbox();
            button1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lbReqUpdate) { UpdateListbox(); }
            if (ClientCount > 0 && MSGChecker.ThreadState == System.Threading.ThreadState.Unstarted) 
            { 
                MSGChecker.Start(); 
            }
            if (ClientCount > 0 && MSGChecker.ThreadState == System.Threading.ThreadState.Stopped)
            {
                MSGChecker = new Thread(new ThreadStart(CheckforIncoming));
                MSGChecker.Start();
            }
        }
    }
}
