using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace CombateMultiplayer
{
    class GerenciadorDeRede
    {
        private string TeclaPressionada;
        private const int BUFFER_SIZE = 1024;
        Tanque Tanque =null;
        int Port = 1138;
        string Ip;

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
        }

        public void inicia() {


            client = listener.AcceptTcpClient();


           Comunica1();

            
        }

        void read(ref Byte[] byteStream){

            int bytesReceived = stream.Read(byteStream, 0, BUFFER_SIZE);
            Tanque.Botoes(Encoding.ASCII.GetString(byteStream,0, bytesReceived));

        }

        void write()
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

        void ProcessData(string cadeia, string ip)
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
                        RecebimentoMensagem10(new String(msg), ip);

                        break;
                    }
                case 11:
                    {
                        RecebimentoMensagem11(new String(msg), ip);

                        break;
                    }

            }
        }

        private void RecebimentoMensagem10(string p, string ip)
        {
            Tanque.Jogo.StartGame();
        }

        private void RecebimentoMensagem11(string str, string ip)
        {
            string[] strings = str.Split(new Char[] { '|' });
            string posX, posY, dir;
            posX = strings[0];
            posY = strings[1];
            dir = strings[2];



        }


        void Comunica1(){            

                try
                {
                    stream = client.GetStream();

                    while (true)
                    {

                        Byte[] byteStream = new byte[BUFFER_SIZE];

                        read(ref byteStream);
                        
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


