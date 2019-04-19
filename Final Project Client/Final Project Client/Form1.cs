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
using System.IO;

namespace Final_Project_Client
{
    public partial class Form1 : Form
    {

        IPAddress MyLocalIp;
        IPEndPoint myEP, ServerEp;
        public static Bitmap USRIMG;
        public static Socket Server;
        public static string Username, EncPass, Displayname;
        OpenFileDialog UIP = new OpenFileDialog();
        public static string MainPath = FindMainPath();


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

        public Form1()
        {
            InitializeComponent();
        }

        private string IMGtoString(Bitmap Input)
        {
            byte[] IMGbyte;
            string IMGString = "";
            ImageConverter IMGCon = new ImageConverter();
            IMGbyte = (byte[])IMGCon.ConvertTo(Input, typeof(byte[]));
            for (int k = 0; k < IMGbyte.Length; k++)
            {
                IMGString += IMGbyte[k].ToString() + '|';
            }
            IMGString.Substring(0, IMGString.Length - 2);
            return IMGString;
        }

        private Bitmap StringtoIMG(string Input)
        {
            string[] IMGStringArray = Input.Split('|');
            byte[] IMGbyte = new byte[IMGStringArray.Length - 1];
            Image image;
            for (int c = 0; c < IMGbyte.Length; c++)
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

        private string Encrypt(string Input)
        {
            char[] pass = Input.ToCharArray();
            byte[] passbytes = new byte[Input.Length * 2];
            int CharsUsed, BytesUsed;
            bool ENCComp;
            Encoder PassEncoder = new UnicodeEncoding().GetEncoder();
            PassEncoder.Convert(pass, 0, pass.Length, passbytes, 0, Input.Length * 2, true, out CharsUsed, out BytesUsed, out ENCComp);
            for (int k = 0; k < passbytes.Length; k++)
            {
                passbytes[k] = byte.Parse((int.Parse(passbytes[k].ToString()) + (9 + (2 * k))).ToString());
            }
            string passstring = "";
            for (int k = 0; k < passbytes.Length; k++)
            {
                passstring += passbytes[k].ToString() + '|';
            }
            return passstring + CharsUsed + '|' + BytesUsed;
        }

        private string Decrypt(string Input)
        {
            string[] pass = Input.Split('|');
            int CharsUsed, BytesUsed;
            CharsUsed = int.Parse(pass[pass.Length - 2]);
            BytesUsed = int.Parse(pass[pass.Length - 1]);
            char[] passchar = new char[CharsUsed];
            byte[] passbytes = new byte[BytesUsed];
            for (int k = 0; k < pass.Length - 2; k++)
            {
                passbytes[k] = byte.Parse((int.Parse(pass[k]) - (9 + (2 * k))).ToString());
            }
            Decoder PassDecoder = new UnicodeEncoding().GetDecoder();
            int buffer;
            bool DECComp;
            PassDecoder.Convert(passbytes, 0, BytesUsed, passchar, 0, CharsUsed, true, out buffer, out buffer, out DECComp);
            string passstring = "";
            for (int k = 0; k < passchar.Length; k++) { passstring += passchar[k]; }
            return passstring;

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
 LOG = Login request
    Subfields:
        New: Create new account
        Old: Use created account
    Message: 
        Username;Password;DisplayName
 */
        /*
        private void SendImage(Socket SenderSocket)
        {
            byte[] imgbytes = 

        }
        */
        private void Send(string MSGTYPE, string Subfield, string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(MSGTYPE + '\a' + Subfield + '\a' + Message + '\a' + "MSGEND");
            SenderSocket.Send(byData);
        }

        private string Recieve(Socket RecieveSocket)
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
                buffer = new byte[RecieveSocket.Available];
                iRx = RecieveSocket.Receive(buffer);
                chars = new char[iRx];
                charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                Recieved = new System.String(chars);
                FullREC += Recieved;
                Splitter = FullREC.Split('\a');
            }
            return FullREC;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            MyLocalIp = IPAddress.Parse(GetLocalIPAddress());
            myEP = new IPEndPoint(MyLocalIp, 922);
            comboBox1.Items.Add(myEP);
            if (Final_Project_Client.Properties.Settings.Default.PreviousLogin == true)
            {
                
                if (Final_Project_Client.Properties.Settings.Default.User != "") { textBox1.Text = Final_Project_Client.Properties.Settings.Default.User; checkBox1.Checked = true; }
                if (Final_Project_Client.Properties.Settings.Default.Pass != "") { textBox2.Text = Decrypt(Final_Project_Client.Properties.Settings.Default.Pass); checkBox2.Checked = true; }
                //pictureBox1.Image = StringtoIMG(Properties.Settings.Default.IMGString);
                comboBox1.Text = Final_Project_Client.Properties.Settings.Default.Server;
                rbExisting.Checked = true;
                textBox3.Enabled = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] CB = new string[2];
            string[] Recieved = new string[4];
            Bitmap Image;
            CB = comboBox1.Text.Split(':');
            ServerEp = new IPEndPoint(IPAddress.Parse(CB[0]),int.Parse(CB[1]));
            Server = new Socket(MyLocalIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try { Server.Connect(ServerEp); }
            catch (SocketException d) 
            {
                if (d.ErrorCode == 10061)
                {
                    MessageBox.Show("Server not running at specified address");
                }
                else 
                {
                    MessageBox.Show(d.ToString());
                }
            }
            

            Username = textBox1.Text;
            EncPass = Encrypt(textBox2.Text);
            Displayname = textBox3.Text;

            if (Server.Connected)
            {
                if (radioButton1.Checked)
                {
                    Image = new Bitmap(pictureBox1.Image);
                    USRIMG = Image;
                    Send("LOG", "New", Username + ';' + EncPass + ';' + Displayname + ';' + IMGtoString(USRIMG), Server);
                    Recieved = Recieve(Server).Split('\a');
                    if (Recieved[0] == "STS" && Recieved[1] == "UPD")
                    {
                        string[] Splitter = Recieved[2].Split(';');
                        if (Splitter[0] == "Login Success")
                        {
                            //Login Success
                            Final_Project_Client.Properties.Settings.Default.Server = comboBox1.Text;
                            if (radioButton1.Checked)
                            {
                                Properties.Settings.Default.IMGString = IMGtoString(USRIMG);
                                StreamWriter Logger1 = new StreamWriter(MainPath + "\\Logs\\SentIMGString.log");
                                Logger1.WriteLine(IMGtoString(USRIMG));
                            }
                            if (checkBox1.Checked)
                            {
                                Final_Project_Client.Properties.Settings.Default.User = textBox1.Text;
                            }
                            if (checkBox2.Checked)
                            {
                                Final_Project_Client.Properties.Settings.Default.Pass = Encrypt(textBox2.Text);
                            }
                            Final_Project_Client.Properties.Settings.Default.PreviousLogin = true;

                            //USRIMG.Save(MainPath + "\\settings\\USRIMAGE.BMP");
                            Properties.Settings.Default.Save();
                            Form CR = new Chat_Room();
                            CR.Show();
                            this.Hide();
                        }
                        else
                        {
                            Server.Close();
                            MessageBox.Show(Recieved[2]);
                        }
                    }
                }
                if (rbExisting.Checked)
                {
                    Send("LOG", "Old", Username + ';' + EncPass, Server);
                    Recieved = Recieve(Server).Split('\a');
                    if (Recieved[0] == "STS" && Recieved[1] == "UPD")
                    {
                        string[] Splitter = Recieved[2].Split(';');
                        if (Splitter[0] == "Login Success")
                        {
                            //Login Success
                            Displayname = Splitter[1];
                            Final_Project_Client.Properties.Settings.Default.Server = comboBox1.Text;
                            if (radioButton1.Checked) { Properties.Settings.Default.IMGString = IMGtoString(USRIMG); }
                            if (checkBox1.Checked)
                            {
                                Final_Project_Client.Properties.Settings.Default.User = textBox1.Text;
                            }
                            if (checkBox2.Checked)
                            {
                                Final_Project_Client.Properties.Settings.Default.Pass = Encrypt(textBox2.Text);
                            }
                            Final_Project_Client.Properties.Settings.Default.PreviousLogin = true;

                            //USRIMG.Save(MainPath + "\\settings\\USRIMAGE.BMP");
                            Properties.Settings.Default.Save();
                            Form CR = new Chat_Room();
                            CR.Show();
                            this.Hide(); 
                            
                        }
                        else
                        {
                            Server.Close();
                            MessageBox.Show(Recieved[2]);
                        }
                    }
                }
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
            }
            else { textBox3.Enabled = false; }

            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool IMGWarning = false;

            UIP.Multiselect = false;
            UIP.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            UIP.ShowDialog();
            
            if (File.Exists(UIP.FileName))
            {
                USRIMG = new Bitmap(UIP.FileName);
                int IMGHeight = USRIMG.Height, IMGWidth = USRIMG.Width;
                if (IMGHeight != IMGWidth || IMGHeight > 128 || IMGHeight < 64)
                {
                    IMGWarning = true;
                    if (IMGHeight > 128 && IMGWidth > 128) { USRIMG = new Bitmap(USRIMG, new Size(128, 128)); }
                    else if (IMGHeight <= 128 && IMGHeight >= 64 && IMGWidth != IMGHeight) { USRIMG = new Bitmap(USRIMG, new Size(IMGHeight, IMGHeight)); }
                    
                }
                pictureBox1.Image = USRIMG;
                pictureBox1.Refresh();
            }
            if (IMGWarning) { MessageBox.Show("Image automatically resized, recommended size is between 64*64 and 128*128, with 1:1 aspect ratio"); }
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "" && pictureBox1.Image != null) || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

    }
}
