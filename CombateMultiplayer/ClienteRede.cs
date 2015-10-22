using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace CombateMultiplayer
{
    class ClienteRede
    {
        private string TeclaPressionada;
        string Ip = "192.168.5.115";
        int Porto = 1138;

        private const int BUFFER_SIZE = 1024;

        Tanque Tanque;
        TcpClient client;


        public ClienteRede(Tanque t, string ip, int port)
        {
            Porto = port;
            Tanque = t;
            Ip = ip;
            client = new TcpClient(Ip, Porto);
        }

        public void roda()
        {

            var stream = client.GetStream();

            while (true)
            {
                if (TeclaPressionada != null)
                {
                    var bytesToSend = Encoding.ASCII.GetBytes(TeclaPressionada);

                    stream.Write(bytesToSend, 0, bytesToSend.Length);

                    TeclaPressionada = null;
                }
                else
                {
                    stream.Write(new byte[2], 0, 2);
                }
                var buffer = new byte[BUFFER_SIZE];

                var bytesReceived = stream.Read(buffer, 0, BUFFER_SIZE);
                Tanque.Botoes(Encoding.ASCII.GetString(buffer, 0, bytesReceived));




            }
        }

        public void recebeTecla(string tecla)
        {
            TeclaPressionada = tecla;
        }

    }
}