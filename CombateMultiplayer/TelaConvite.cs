using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombateMultiplayer
{
    public partial class TelaConvite : Form
    {
        TelaInicial telainicial;
        string name;
        string IP;

        public TelaConvite(string str,string ip)
        {
            InitializeComponent();
            name = str;
            IP = ip;
            label1.Text = String.Format("{0} mijou no seu quintal,\nte chamou de mariquinha\ne peidou na cara da sua avó.\nVai deixar barato?",name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnviaMsg04();
            Form f = new TelaDeJogo(1, IP,1138);
            f.Show();
        }

        private void EnviaMsg04()
        {
            byte[] mensage = new byte[1024];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), 20152);

            Socket UDPEnvia = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string msg = name + "|"+"1138";


            mensage = Encoding.ASCII.GetBytes("04" + string.Format("{0:000}", msg.Length + 5) + msg);
            UDPEnvia.SendTo(mensage, SocketFlags.None, remoteEndPoint);

        }
    }
}
