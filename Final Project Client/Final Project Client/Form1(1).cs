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

namespace Final_Project_Client
{
    public partial class Form1 : Form
    {

        IPAddress MyLocalIp;
        IPEndPoint myEP, ServerEp;
        Socket Server;

        public Form1()
        {
            InitializeComponent();
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

        private void Send(string MSGTYPE, string Subfield, string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(MSGTYPE + '\n' + Subfield + '\n' + Message + '\n' + System.DateTime.Now.ToString("hh:mm"));
            SenderSocket.Send(byData);
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
                if (Final_Project_Client.Properties.Settings.Default.User != "") { textBox1.Text = Final_Project_Client.Properties.Settings.Default.User; }
                if (Final_Project_Client.Properties.Settings.Default.Pass != "") { textBox2.Text = Decrypt(Final_Project_Client.Properties.Settings.Default.Pass); }
                comboBox1.Text = Final_Project_Client.Properties.Settings.Default.Server;
                rbExisting.Checked = true;
                textBox3.Enabled = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "") || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "") || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "") || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] CB = new string[2];
            string[] Recieved = new string[4];
            CB = comboBox1.Text.Split(':');
            ServerEp = new IPEndPoint(IPAddress.Parse(CB[0]),int.Parse(CB[1]));
            Server = new Socket(MyLocalIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Server.Connect(ServerEp);
            if(radioButton1.Checked)
            { Send("LOG", "New", textBox1.Text + ';' + Encrypt(textBox2.Text) + ';' + textBox3.Text, Server); }
            if(rbExisting.Checked)
            { Send("LOG", "Old", textBox1.Text + ';' + Encrypt(textBox2.Text), Server); }
            Recieved = Recieve(Server).Split('\n');
            if (Recieved[0] == "STS" && Recieved[1] == "UPD")
            {
                if (Recieved[2] == "Login Success")
                {
                    //Login Success
                    Final_Project_Client.Properties.Settings.Default.Server = comboBox1.Text;
                    if (checkBox1.Checked) { Final_Project_Client.Properties.Settings.Default.User = textBox1.Text; }
                    if (checkBox2.Checked) { Final_Project_Client.Properties.Settings.Default.Pass = Encrypt(textBox2.Text); }
                    Final_Project_Client.Properties.Settings.Default.PreviousLogin = true;
                }
                else
                {
                    Server.Close();
                    MessageBox.Show(Recieved[2]);
                }

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && comboBox1.Text != "") || (textBox1.Text != "" && textBox2.Text != "" && textBox3.Enabled == false && comboBox1.Text != ""))
            { button1.Enabled = true; }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
            }
            else { textBox3.Enabled = false; }
        }
    }
}
