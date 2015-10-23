using System;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace CombateMultiplayer
{
    class GerenciadorDeRede
    {
        private string TeclaPressionada;
        private const int BUFFER_SIZE = 1024;
        Tanque Tanque =null;
        int Port = 1138;
        string Ip;

        Queue<Byte[]> FilaDeMensagens;

        TcpListener listener = null;

        TcpClient client = null;
        NetworkStream stream = null;

        bool rodando = true;

        public GerenciadorDeRede(Tanque tanque,int port,string ip,bool isClient)
        {
            Port = port;
            Tanque = tanque;

            if (isClient)
            {
                Ip = ip;
                client = new TcpClient(Ip, Port);
            }
            else {
                try
                {
                    listener = new TcpListener(IPAddress.Any, Port);
                    listener.Start();
                }
                catch (SocketException e)
                {
                    Environment.Exit(e.ErrorCode);
                }
            }
            FilaDeMensagens = new Queue<byte[]>();
        }

        public void inicia() {


            client = listener.AcceptTcpClient();


           Comunica1();

            
        }


        void writeAntiquado()
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
        }

        void write()
        {
            if (FilaDeMensagens.Count > 0)
            {
                while (FilaDeMensagens.Count > 0)
                {
                    byte[] msg = FilaDeMensagens.Dequeue();
                    stream.Write(msg, 0, msg.Length);
                    
                }
            }
            else
            {
                stream.Write(new byte[2], 0, 2);
            }
        }

        void pegaTeclas() {
            if (TeclaPressionada != null)
            {

                Tanque outro = Tanque.Jogo.outroTanque(Tanque);
                switch (TeclaPressionada)
                {
                    case "Left":
                        {
                            EnviaMensagem11((outro.Position.X - outro.Velocidade), outro.Position.Y, 0);
                             break;
                        }
                    case "Up":
                        {
                            EnviaMensagem11(outro.Position.X , outro.Position.Y-outro.Velocidade, 1);
                            break;
                        }
                    case "Right":{


                        EnviaMensagem11((outro.Position.X + outro.Velocidade), outro.Position.Y, 2);
                            break;
                    }
                    case "Down":
                        {
                            EnviaMensagem11(outro.Position.X, outro.Position.Y + outro.Velocidade, 3);
                            break;
                        }
                    default:
                        break;
                }
                TeclaPressionada = null;
            }
        
        
        }


        void readAntiquado(ref Byte[] byteStream)
        {

            int bytesReceived = stream.Read(byteStream, 0, BUFFER_SIZE);
            Tanque.Botoes(Encoding.ASCII.GetString(byteStream, 0, bytesReceived));

        }


        void read(ref Byte[] byteStream)
        {

            int bytesReceived = stream.Read(byteStream, 0, BUFFER_SIZE);
            ProcessData(Encoding.ASCII.GetString(byteStream, 0, bytesReceived));

        }


        void ProcessData(string cadeia)
        {
            if (cadeia.Length > 4)
            {
                int codigo, tamanho;
                codigo = int.Parse(cadeia[0].ToString() + cadeia[1].ToString());
                tamanho = int.Parse(cadeia[2].ToString() + cadeia[3].ToString() + cadeia[4].ToString());
                char[] msg = new char[tamanho - 5];
                cadeia.CopyTo(5, msg, 0, tamanho - 5);

                switch (codigo)
                {
                    case 10:
                        {
                            RecebimentoMensagem10(new String(msg));

                            break;
                        }
                    case 11:
                        {
                            RecebimentoMensagem11(new String(msg));

                            break;
                        }

                }
            }
        }

        private void RecebimentoMensagem10(string p)
        {
            Tanque.Jogo.StartGame();
        }

        private void RecebimentoMensagem11(string str)
        {
            string[] strings = str.Split(new Char[] { '|' });
            float posX, posY;
                int dir;
            posX = float.Parse(strings[0]);
            posY = float.Parse(strings[1]);
            dir = int.Parse(strings[2]);


            Tanque.moveTo(posX, posY,dir);
            EnviaMensagem12(posX,posY,dir);
        }

        public void EnviaMensagem10()
        {
            byte[] byteMsg = Encoding.ASCII.GetBytes("10005");

            FilaDeMensagens.Enqueue(byteMsg);

        }

        private void EnviaMensagem11(float posX, float posY,int dir) {
            string msg = (posX + "|" + posY + "|" + dir);
            byte[] byteMsg = Encoding.ASCII.GetBytes("11" + string.Format("{0:000}", msg.Length+5) + msg);

            FilaDeMensagens.Enqueue(byteMsg);
        
        }

        private void EnviaMensagem12(float posX, float posY, int dir)
        {
            string msg = (posX + "|" + posY + "|" + dir);
            byte[] byteMsg = Encoding.ASCII.GetBytes("12" + string.Format("{0:000}", msg.Length + 5) + msg);

            FilaDeMensagens.Enqueue(byteMsg);

        }

        void Comunica1(){            

                try
                {
                    stream = client.GetStream();

                    while (true)
                    {

                        Byte[] byteStream = new byte[BUFFER_SIZE];

                        read(ref byteStream);
                        pegaTeclas();
                        write();
                       
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    stream.Close();
                    client.Close();
                }
        
        

        }

        public void Comunica2()
        {

            try
            {
                stream = client.GetStream();

                while (true)
                {

                    Byte[] byteStream = new byte[BUFFER_SIZE];

                    pegaTeclas();
                    write();
                    read(ref byteStream);


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                stream.Close();
                client.Close();
            }



        }

        public void recebeTecla(string tecla)
        {
            TeclaPressionada = tecla;
        }
    }
}


