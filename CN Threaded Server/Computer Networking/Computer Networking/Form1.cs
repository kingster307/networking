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

namespace Computer_Networking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        System.Net.IPAddress myAddress;
        bool RemoveIP = false, Running;
        string ClientIP, IPRemove;

        private void Form1_Load(object sender, EventArgs e)
        {
            myAddress = IPAddress.Parse(GetLocalIPAddress());
            lbllclIP.Text = "LocalIP: " + myAddress.ToString();
        }

        public static string GetLocalIPAddress()
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

        private void button1_Click(object sender, EventArgs e)
        {
            Running = true;
            backgroundWorker1.RunWorkerAsync();
            lblOutput.Visible = true;
            lblOutput.Text = "Waiting for Clients";
        }

        private void HandleClient(object Client)
        {
            Socket CLSock  = (Socket) Client;
            Send(k.Text,CLSock);
            Form2 Chatbox = new Form2();
            Chatbox.Chatbox(CLSock);
            Application.Run(Chatbox);
        }

        private void Send(string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(Message);
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Running)
            {
                IPEndPoint myEP = new IPEndPoint(myAddress, 922);
                Socket listeningSocket = new Socket(myAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(myEP);
                listeningSocket.Listen(922);
                Socket acceptedSocket = listeningSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleClient));
                thread.Start(acceptedSocket);
                ClientIP = acceptedSocket.RemoteEndPoint.ToString();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (lstClients.Items.Contains(ClientIP) == false) { lstClients.Items.Add(ClientIP); }
            if (Running){ backgroundWorker1.RunWorkerAsync();}
            if (RemoveIP) { lstClients.Items.Remove(RemoveIP); }
        }

        public void Disconnected(string IP)
        {
            RemoveIP = true;
            IPRemove = IP;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Running = false;
        }

       

     
    }
}
