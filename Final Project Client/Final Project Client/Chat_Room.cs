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

namespace Final_Project_Client
{
    public partial class Chat_Room : Form
    {
        OpenFileDialog OFD = new OpenFileDialog();
        //need to get full file path of selected file
        string[][] Messages = new string[1][];
        StreamWriter FileSaver;
        bool running = true;
        public string filepath;
        int Selfindex, SelectedIndex, SelectedFormRow;
        string NewCL;
        bool FirstSelection = true, updCB, upCL;
        string[,] Clients = new string[1,4];//Displayname, Image, Status, Unread Count
        Label[] ClientLabels= new Label[1];//TAG == Form ROW INDEX, NAME == Clients ROW
        PictureBox[] ClientImgs = new PictureBox[1], Status = new PictureBox[1];

        public Chat_Room()
        {
            InitializeComponent();
        }



        private string IMGtoString(Bitmap Input)
        {
            if (Input != null)
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
            return null;
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

        private void pbSelectionChange(object sender, EventArgs e)
        {
            PictureBox Sender = (PictureBox)sender;
            int Row = int.Parse(Sender.Tag.ToString());
            if (SelectedIndex != int.Parse(Sender.Name.ToString()) || FirstSelection)
            {
                SelectionChange(SelectedFormRow, int.Parse(Sender.Tag.ToString()));
                FirstSelection = false;
            }
            
        }

        private void lblSelectionChange(object sender, EventArgs e)
        {
            Label Sender = (Label)sender;
            int Row = int.Parse(Sender.Tag.ToString());
            if (SelectedIndex != int.Parse(Sender.Name.ToString()) || FirstSelection)
            {
                SelectionChange(SelectedFormRow, int.Parse(Sender.Tag.ToString()));
                FirstSelection = false;
            }
        }

        private void SelectionChange(int OldRowIndex, int NewRowIndex)
        {
            SelectedIndex = int.Parse(ClientLabels[NewRowIndex].Name.ToString());
            SelectedFormRow = int.Parse(ClientLabels[NewRowIndex].Tag.ToString());
            Clients[int.Parse(ClientLabels[NewRowIndex].Name), 3] = "0";
            ClientLabels[OldRowIndex].BackColor = System.Drawing.Color.Transparent;
            ClientLabels[NewRowIndex].BackColor = System.Drawing.Color.Blue;
            Chat_Box_MessagingP.Visible = true;
            lblConvHeader.Text = Clients[SelectedIndex,0];
            pbConvHeader.Image = StringtoIMG(Clients[SelectedIndex, 1]);
            UpdateChatbox();
        }

        /*
        private string Recieve(Socket RecieveSocket)
        {
            byte[] buffer = new byte[RecieveSocket.Available];
            int iRx = RecieveSocket.Receive(buffer);
            char[] chars = new char[iRx];
            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            System.String Recieved = new System.String(chars);
            /*
            if (Recieved.StartsWith("P") && Recieved.Substring(1, 1) == "1")
            {
                //Multipart message
                string RecievedMultipart = "";
                string[] Splitter = Recieved.Split('\v');
                while (RecieveSocket.Available != 0)
                {
                    iRx = RecieveSocket.Receive(buffer);
                    chars = new char[iRx];
                    charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                    Recieved = new string(chars);
                    Splitter = Recieved.Split('\v');
                    RecievedMultipart += Splitter[1];
                }
                return RecievedMultipart;
            }
            else {*/ /*return Recieved; //}
            
        }
    */

        private void LogMessages()
        {
            StreamWriter MessageLogger = new StreamWriter(Form1.MainPath + "\\Logs\\" + Form1.Displayname + "Messages.log");
            for (int c = 0; c < Messages.Length; c++)
            {
                for (int k = 0; k < Messages[c].Length; k++)
                {
                    MessageLogger.Write(Messages[c][k] + '\f');
                }
                MessageLogger.Write("\r\n");
            }
            MessageLogger.Close();
        }

        private void UpdateChatbox()
        {
            string[] Splitter;
            Chat_Box_Tab_Lay_Pan.RowCount = 0;
            Chat_Box_Tab_Lay_Pan.Controls.Clear();
            Chat_Box_Tab_Lay_Pan.RowStyles.Clear();
            Chat_Box_Tab_Lay_Pan.RowCount = 1;
            Chat_Box_Tab_Lay_Pan.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

            for (int c = 0; c < ClientLabels.Length-1; c++)
            {
                if (int.Parse(ClientLabels[c].Name) != SelectedIndex)
                {
                    if (int.Parse(Clients[int.Parse(ClientLabels[c].Name), 3]) > 0)
                    {
                        //Unselected add notification
                        ClientLabels[c].Text = Clients[int.Parse(ClientLabels[c].Name), 0] + '\r' + '\n' + Clients[int.Parse(ClientLabels[c].Name), 3] + " Unread";
                        ClientLabels[c].BackColor = Color.DarkOrange;
                        ClientLabels[c].Height = ClientLabels[c].PreferredHeight;
                        System.Media.SystemSounds.Beep.Play();
                    }
                    else if (int.Parse(Clients[int.Parse(ClientLabels[c].Name), 3]) == 0)
                    {
                        //Unselected remove Notification
                        ClientLabels[c].Text = Clients[int.Parse(ClientLabels[c].Name), 0];
                        ClientLabels[c].BackColor = Color.Transparent;
                        ClientLabels[c].Height = ClientLabels[c].PreferredHeight;
                    }
                }
                else
                {
                    //Selected
                    ClientLabels[c].Text = Clients[int.Parse(ClientLabels[c].Name), 0];
                    ClientLabels[c].BackColor = Color.Blue;
                    ClientLabels[c].Height = ClientLabels[c].PreferredHeight;
                }
            }
            tlpClients.Refresh();
            for (int c = 0; c < Messages.Length - 1; c++)
                {
                    if (Messages[c][0] == Clients[SelectedIndex, 0])
                    {
                        //Found Message Array

                        if (Messages[c].Length > 1)
                        {
                            for (int k = 1; k < Messages[c].Length; k++)
                            {
                                Splitter = Messages[c][k].Split('\v');
                                if (Splitter[0] == "R")
                                {
                                    Label lbl = new Label();
                                    lbl.Text = Splitter[1] + '\r' + '\n' + Splitter[2];
                                    lbl.BorderStyle = BorderStyle.FixedSingle;
                                    lbl.BackColor = Color.Gray;
                                    lbl.AutoSize = true;
                                    lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);
                                    lbl.TextAlign = ContentAlignment.BottomRight;
                                    Chat_Box_Tab_Lay_Pan.RowStyles.Add(new RowStyle(SizeType.Absolute, (lbl.Height + 6)));
                                    Chat_Box_Tab_Lay_Pan.Controls.Add(lbl, 0, Chat_Box_Tab_Lay_Pan.RowCount - 1);
                                    Chat_Box_Tab_Lay_Pan.RowCount++;

                                }
                                if (Splitter[0] == "S")
                                {
                                    Label lbl = new Label();
                                    lbl.Text = Splitter[1] + '\r' + '\n' + Splitter[2];
                                    lbl.BorderStyle = BorderStyle.FixedSingle;
                                    lbl.BackColor = Color.OrangeRed;
                                    lbl.AutoSize = true;
                                    lbl.Anchor = (AnchorStyles.Right | AnchorStyles.Bottom);
                                    lbl.TextAlign = ContentAlignment.BottomRight;
                                    Chat_Box_Tab_Lay_Pan.RowStyles.Add(new RowStyle(SizeType.Absolute, (lbl.Height + 6)));
                                    Chat_Box_Tab_Lay_Pan.Controls.Add(lbl, 1, Chat_Box_Tab_Lay_Pan.RowCount - 1);
                                    Chat_Box_Tab_Lay_Pan.RowCount++;
                                }
                            }
                            break;
                        }
                    }
                }
        }

        private void Send(string MSGTYPE, string Subfield, string Message, Socket SenderSocket)
        {
            byte[] byData = System.Text.Encoding.ASCII.GetBytes(MSGTYPE + '\a' + Subfield + '\a' + Message + '\a' + "MSGEND");
            SenderSocket.Send(byData);
        }

        private void HandleIncoming()
        {
            
            string[] Recieved = new string[4];
            
            while(running)
            {
                if (Form1.Server.Available > 0)
                {
                    Recieved = Recieve(Form1.Server).Split('\a');
                    if (Recieved[0] == "USR")
                    {
                        for (int c = 0; c < Clients.GetUpperBound(0); c++)
                        {
                            if (Messages[c][0] == Recieved[1])
                            {
                                //Display Name match
                                Array.Resize(ref Messages[c], Messages[c].Length + 1);
                                Messages[c][Messages[c].Length - 1] = "R\v"+Recieved[2]+'\v'+DateTime.Now.ToString("hh:mm");
                                LogMessages();
                                if (c >= Selfindex)
                                {
                                    //Add unread notfications
                                    Clients[c + 1, 3] = (int.Parse(Clients[c + 1, 3]) + 1).ToString();
                                }
                                else
                                {
                                    //Add unread notfications
                                    Clients[c, 3] = (int.Parse(Clients[c, 3]) + 1).ToString();
                                }
                                updCB = true;
                                break;
                            }
                        }
                    }
                    if (Recieved[0] == "SYS")
                    {
                        if (Recieved[1] == "CLU")
                        {
                            NewCL = Recieved[2];
                            upCL = true;     
                        }
                        if (Recieved[1] == "FTR")
                        {
                            string[] Split = Recieved[2].Split('\f');
                            string Filename = Split[1];
                            for (int c = 0; c < Clients.GetUpperBound(0); c++)
                            {
                                if (Messages[c][0] == Split[0])
                                {
                                    Array.Resize(ref Messages[c], Messages[c].Length + 1);
                                    Messages[c][Messages[c].Length - 1] = "R\v" + Filename + '\v' + DateTime.Now.ToString("hh:mm");
                                    FileSaver = new StreamWriter(Form1.MainPath + "\\Recieved Files\\"+Filename);
                                    FileSaver.Write(Split[2]);
                                    FileSaver.Close();

                                    LogMessages();
                                    if (Split[0] == Clients[SelectedIndex, 0])
                                    {
                                        updCB = true;
                                    }
                                }
                            }

                        }
                    }
                    if (Recieved.Length == 3)
                    { MessageBox.Show(Recieved[2]); }
                }
            }

        }

        private void UpdateClientList(string ClientList)
        {
            int OCC =0, OrigL;
            string[] CLArray = ClientList.Split('\r'), ClientArray;
            tlpClients.Controls.Clear();
            tlpClients.RowCount = 0;
            ClientLabels = new Label[CLArray.Length-1];
            ClientImgs = new PictureBox[CLArray.Length - 1];
            Clients = new string[CLArray.Length-1, 4];
            OrigL = Messages.Length;
            Array.Resize(ref Messages, CLArray.Length-1);
            for (int c = OrigL-1; c < Messages.Length; c++)
            {
                Messages[c] = new string[1];
            }
                
            for (int c = 0; c < CLArray.Length-1; c++)
            {
                ClientArray = CLArray[c].Split(';');
                Clients[c, 0] = ClientArray[0];
                Clients[c, 1] = ClientArray[1];
                Clients[c, 2] = ClientArray[2];
                Clients[c, 3] = "0";
            }
            for (int c = 0; c <= Clients.GetUpperBound(0); c++)
                {
                    if (Form1.Displayname == Clients[c, 0])
                    {
                        Selfindex = c;
                    }
                    else 
                    {
                        if (bool.Parse(Clients[c, 2]))
                        {
                            //Online
                            ClientLabels[OCC] = new Label();
                        }
                        else 
                        { 
                            //Offline
                            ClientLabels[OCC] = new Label();
                            ClientLabels[OCC].ForeColor = System.Drawing.Color.LightGray;
                        }
                        ClientImgs[OCC] = new PictureBox();
                        ClientImgs[OCC].Image = StringtoIMG(Clients[c, 1]);
                        ClientImgs[OCC].SizeMode = PictureBoxSizeMode.StretchImage;
                        ClientImgs[OCC].Tag = OCC;
                        ClientImgs[OCC].Name = c.ToString();
                        ClientImgs[OCC].Click += pbSelectionChange;
                        ClientLabels[OCC].Text = Clients[c, 0];
                        ClientLabels[OCC].Tag = OCC;
                        ClientLabels[OCC].Name = c.ToString();
                        ClientLabels[OCC].Click += lblSelectionChange;

                    tlpClients.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
                    tlpClients.RowCount++;
                        tlpClients.Controls.Add(ClientImgs[OCC],0,tlpClients.RowCount);
                        tlpClients.Controls.Add(ClientLabels[OCC], 1, tlpClients.RowCount);

                        Messages[OCC][0] = Clients[c, 0];

                        OCC++;
                    }
                }
        }

        private void Chat_Room_Load(object sender, EventArgs e)
        {
            Messages[0] = new string[1];
            string[] Recieved = new string[4];
            Chat_Box_MessagingP.Visible = false;

            if (lblDisplayName.Text != "")
            {
                lblDisplayName.Text = Form1.Displayname;
                pbUserIMG.Image = Form1.USRIMG;
                Properties.Settings.Default.IMGString = IMGtoString(Form1.USRIMG);
            }
            Recieved = Recieve(Form1.Server).Split('\a');
            if (Recieved[0] == "SYS" && Recieved[1] == "CLU")
            {
                UpdateClientList(Recieved[2]);
                pbUserIMG.Image = StringtoIMG(Clients[Selfindex,1]);
                lblDisplayName.Text = Clients[Selfindex, 0];
            }
            if (File.Exists(Form1.MainPath + "\\Logs\\"+Form1.Displayname+"Messages.log"))
            {
                int c = 0;
                StreamReader MessageInit = new StreamReader(Form1.MainPath + "\\Logs\\" + Form1.Displayname + "Messages.log");
                while (MessageInit.Peek() > -1)
                {
                    Messages[c] = MessageInit.ReadLine().Split('\f');
                    c++;
                    Array.Resize(ref Messages, c+1);
                }
                Array.Resize(ref Messages, Messages.Length-1);
                for (int d = 0; d < Messages.Length; d++)
                {
                    Array.Resize(ref Messages[d], Messages[d].Length - 1);

                }
                MessageInit.Close();
            }
            Thread NewThread = new Thread(new ThreadStart(HandleIncoming));
            NewThread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string file;
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                filepath = OFD.FileName;
                if (File.Exists(filepath))
                { 
                    StreamReader Reader = new StreamReader(filepath);
                    file = Reader.ReadToEnd();
                    Send("SYS", "FTR", Clients[SelectedIndex, 0] + '\v'+ filepath.Substring(filepath.LastIndexOf('\\')+1) +'\v'+ file, Form1.Server);
                }
                for (int c = 0; c < Messages.Length; c++)
                {
                    if (Messages[c][0] == Clients[SelectedIndex, 0])
                    {
                        Array.Resize(ref Messages[c], Messages[c].Length + 1);
                        Messages[c][Messages[c].Length - 1] = "S\v" + "Sent: " + filepath.Substring(filepath.LastIndexOf('\\') + 1) + '\v' + DateTime.Now.ToString("hh:mm");
                        LogMessages();
                        UpdateChatbox();
                        break;
                    }
                }
              //  MessageBox.Show(filepath);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < Messages.Length; c++)
            {
                if (Messages[c][0] == Clients[SelectedIndex, 0])
                {
                    //Display Name match
                    Array.Resize(ref Messages[c], Messages[c].Length + 1);
                    Messages[c][Messages[c].Length - 1] = "S\v" + Chat_Box_UserinputTxt.Text +'\v'+ DateTime.Now.ToString("hh:mm");
                    LogMessages();
                    break;   
                }
            }
            Send("USR", Clients[SelectedIndex, 0], Chat_Box_UserinputTxt.Text, Form1.Server);
            Chat_Box_UserinputTxt.Text = "";
            UpdateChatbox();
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Chat_Room_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            Send("STS", "UPD", "Offline", Form1.Server);
            Form1.Server.Close();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updCB) { UpdateChatbox(); updCB = false; }
            if (upCL) { UpdateClientList(NewCL); upCL = false; }
        }

        private void Chat_Box_UserinputTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(sender,e);
            }
        }

        private void Chat_Box_ExitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
