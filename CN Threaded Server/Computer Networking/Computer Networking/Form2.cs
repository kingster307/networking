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
    public partial class Form2 : Form
    {
        Socket Client;
        string RecievedMessage;
        bool Connected = true;

        public Form2()
        {
            InitializeComponent();
        }

        public void Chatbox(object Socket)
        {
            Client = (Socket) Socket;
            byte[] buffer = new byte[1024];
            int iRx = Client.Receive(buffer);
            char[] chars = new char[iRx];
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            System.String Recieved = new System.String(chars);
            listBox1.Items.Add(Recieved + " connected."+ "(" + Client.RemoteEndPoint.ToString() + ")");
            this.Text = "Chat with: "+ Recieved;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "") { Send(textBox1.Text, Client); }
            textBox1.Text = "";
        }

        private void Send(string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(Message);
            SenderSocket.Send(byData);
            listBox1.Items.Add(Message + " Sent @ " +  System.DateTime.Now.ToString("hh:mm"));
            listBox1.Refresh();
        }

        private string Recieve(object objIn)
        {
            Socket RecieveSocket = (Socket)objIn;
            if (RecieveSocket.Connected)
            {
                byte[] buffer = new byte[1024];
                int iRx = RecieveSocket.Receive(buffer);
                char[] chars = new char[iRx];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                System.String Recieved = new System.String(chars);
                RecievedMessage = Recieved + " Recieved @ " + System.DateTime.Now.ToString("hh:mm");
            }
            else 
            { 
                RecievedMessage = "Disconnected @ " + System.DateTime.Now.ToString("hh:mm");
                Connected = false;
            }
            return RecievedMessage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Recieve(Client);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RecievedMessage != listBox1.Items[listBox1.Items.Count - 1].ToString())
            {
                listBox1.Items.Add(RecievedMessage);
                listBox1.Refresh();
            }
            else
            {
                RecievedMessage = "Disconnected @ " + System.DateTime.Now.ToString("hh:mm");
                Connected = false;
            }
            if (Connected) { backgroundWorker1.RunWorkerAsync(); }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void Disconnected()
        {
            var principalForm = Application.OpenForms.OfType<Form1>().Single();
            principalForm.Disconnected(Client.RemoteEndPoint.ToString());
            RecievedMessage = "Disconnected @ " + System.DateTime.Now.ToString("hh:mm");
            Connected = false;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnected();
        }
    }
}
