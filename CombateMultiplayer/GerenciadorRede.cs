using System;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace CombateMultiplayer
{
    public class GerenciadorDeRede
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
            Tanque.InputMovimentação(Encoding.ASCII.GetString(byteStream, 0, bytesReceived));

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
                    case 13:
                        {
                            RecebimentoMensagem13(new String(msg));

                            break;
                        }
                    case 15:
                        {
                            RecebimentoMensagem15(new String(msg));

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

        private void RecebimentoMensagem13(string str)
        {
            string[] strings = str.Split(new Char[] { '|' });
            float posX, posY;
            int dir = 0,id;
            char direction;

            posX = float.Parse(strings[0]);
            posY = float.Parse(strings[1]);
            direction = char.Parse(strings[2]);
            id = int.Parse(strings[3]);

            switch (direction)
            {
                case 'E':
                    {
                        dir = 0;
                        break;
                    }
                case 'C':
                    {
                        dir = 1;
                        break;
                    }
                case 'D':
                    {
                        dir = 2;
                        break;
                    }
                case 'B':
                    {
                        dir = 3;
                        break;
                    }
            }
            Tanque.Jogo.Sprites.Add(new Tirinho(posX, posY, dir, id, false, Tanque.Jogo));
            Tanque.moveTo(posX, posY, dir);
            EnviaMensagem12(posX, posY, dir);
        }

        private void RecebimentoMensagem15(string str)
        {
            string[] strings = str.Split(new Char[] { '|' });
            string obj1, obj2;
            int id;
            obj1= strings[0];
            obj2= strings[1];
            id = int.Parse(strings[2]);

            if (obj2[0] == 'B') {
                Tanque.Jogo.DestroiObstaculo(short.Parse(obj2[2].ToString()), short.Parse(obj2[3].ToString()),(short)(int.Parse( obj2[4].ToString())*10 +int.Parse(obj2[5].ToString())));
                Tanque.Jogo.DestroiTiro(id);
            }

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

        public void EnviaMensagem13(float posX, float posY, int dir,int id)
        {
            char direction='?';
            switch (dir)
            {
                case 0:
                    direction = 'E';
                    break;
                case 1:
                    direction = 'C';
                    break;
                case 2:
                    direction = 'D';
                    break;
                case 3:
                    direction = 'B';
                    break;
            }

            string msg = (posX + "|" + posY + "|" + direction +"|"+id);
            byte[] byteMsg = Encoding.ASCII.GetBytes("13" + string.Format("{0:000}", msg.Length + 5) + msg);

            FilaDeMensagens.Enqueue(byteMsg);

        }

        public void EnviaMensagem15(string str1, string str2, int id)
        {
            string msg = (str1 + "|" + str2 + "|" + id);
            byte[] byteMsg = Encoding.ASCII.GetBytes("15" + string.Format("{0:000}", msg.Length + 5) + msg);

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


