using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Proj_Network_Client
{
    public partial class Chat_Screen_Client : Form
    {
        public Chat_Screen_Client()
        {
            InitializeComponent();
        }

        private void Chat_Screen_Client_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
//user must select whom they wish to talk to
    //after selection convopanel becomes enabeled 
        //talk away


//still need to figure out how to upload and send files
//either select one or multiple//only multiple if time