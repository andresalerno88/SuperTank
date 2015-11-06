using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombateMultiplayer
{
    public partial class TelaDeJogo : Form
    {

        Bitmap areaDeDesenho;
        Graphics desenhista;
        Tanque Tanque1;
        Tanque Tanque2;
        public int tanqueLocal;
        System.Windows.Forms.Timer clockUpdate;
        System.Windows.Forms.Timer clockRefresh;
        System.Windows.Forms.Timer clockAnimation;
        GerenciadorDeRede server;
        GerenciadorDeRede client;
        PowerUp PowerUp;
        Bandeira Bandeira1;
        Bandeira Bandeira2;

        Thread t;
        String IP;
        public List<ProtoSprite> Sprites;
        public List<ProtoSprite> SpritesASeremDeletados;

        int Porto;

        Stack<string> TeclasDeMovimentoPressionada;
        bool SpacePressed = false;

        const int DimensaoDaTelaX = 800, DimensaoDaTelaY = 600;

        public Tanque outroTanque(Tanque t) {
            if (t.Equals(Tanque1))
            {
                return Tanque2;
            }else{
                return Tanque1;
            }
        
        }

        public int posiçaoInteiraX(double a) {
            return (int)(a * DimensaoDaTelaX);
        
        }

        public int posiçaoInteiraY(double a) {
            return (int)(a * DimensaoDaTelaY);
        
        }

        public double posiçaoNormalizadaX(int a){
            return (a / DimensaoDaTelaX);
        }

        public double posiçaoNormalizadaY(int a)
        {
            return (a / DimensaoDaTelaY);
        }

        private void InicializaObjetos() {
            Sprites = new List<ProtoSprite>();
            SpritesASeremDeletados = new List<ProtoSprite>();

            Tanque1 = new Tanque(0, 0.6f, 2, this, DimensaoDaTelaX, DimensaoDaTelaY);
            Tanque2 = new Tanque(0.95f, 0.6f, 2, this, DimensaoDaTelaX, DimensaoDaTelaY);
            Bandeira1 = new Bandeira(0, 0.2f,1);
            Bandeira2 = new Bandeira(0.95f, 0.8f,2);
            PowerUp = new PowerUp(0.5f - 0.05f, 0.55f - 0.05f);
            Sprites.Add(Bandeira1);
            Sprites.Add(Bandeira2);
            Sprites.Add(PowerUp);
            Sprites.Add(Tanque1);
            Sprites.Add(Tanque2);

            for (int v = 0; v < 10; v++)
            {
                Sprites.Add(new Obstaculo(0.15f, 0.1f + 0.09f * v, this,1,1,(short)(v+1)));
                Sprites.Add(new Obstaculo(0.16f, 0.1f + 0.09f * v, this,1, 2, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.85f, 0.1f + 0.09f * v, this, 2, 1, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.86f, 0.1f + 0.09f * v, this, 2, 2, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.30f, 0.1f + 0.09f * v, this, 3, 1, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.31f, 0.1f + 0.09f * v, this, 3, 2, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.70f, 0.1f + 0.09f * v, this, 4, 1, (short)(v + 1)));
                Sprites.Add(new Obstaculo(0.71f, 0.1f + 0.09f * v, this, 4, 2, (short)(v + 1)));
            }
        }

        public TelaDeJogo(int numeroDoTanque, string ip, int porto)
        {
            InitializeComponent();

            IP = ip;
            Porto = porto;
            tanqueLocal = numeroDoTanque;

            areaDeDesenho = new Bitmap(DimensaoDaTelaX, DimensaoDaTelaY); //Cria a área de desenho;
            pictureBox1.Image = areaDeDesenho; //Manda a região delimitada no Design do Visual mostrar nossa área de desenho;
            desenhista = Graphics.FromImage(areaDeDesenho); //Diz onde o desenhista desenha.        

            InicializaObjetos();
            TeclasDeMovimentoPressionada = new Stack<string>();
            TeclasDeMovimentoPressionada.Push("NADA");
            t = new Thread(CriaRede);
            t.Start();

            clockUpdate = new System.Windows.Forms.Timer();
            clockUpdate.Tick += Update;
            clockUpdate.Interval = GlobalConfigurations.UPDATEINTERVAL;

            clockRefresh = new System.Windows.Forms.Timer();
            clockRefresh.Tick += Draw;
            clockRefresh.Interval = GlobalConfigurations.REFRESHINTERVAL;
            clockRefresh.Start();

            clockAnimation = new System.Windows.Forms.Timer();
            clockAnimation.Tick += AnimateAll;
            clockAnimation.Interval = GlobalConfigurations.ANIMATIONINTERVAL;

        }

        public void PauseGame()
        {
            clockUpdate.Stop();
            clockAnimation.Stop();
        }

        public void StartGame() {

            pictureBox1.Invoke((MethodInvoker)delegate() { clockUpdate.Start(); });
            pictureBox1.Invoke((MethodInvoker)delegate() { clockAnimation.Start(); });

            if (tanqueLocal == 1) {

                server.EnviaMensagem10();
            }
        }
        

        private void CriaRede(object a)
        {
            switch (tanqueLocal)
            {
                case 1:
                    server = new GerenciadorDeRede(Tanque2,Porto,null,false);
                    Tanque1.Rede = server;
                    Tanque2.Rede = server;
                    server.inicia();
                    break;
                case 2:
                    client = new GerenciadorDeRede(Tanque1, Porto, IP, true);
                    Tanque2.Rede = client;
                    Tanque1.Rede = client;
                    client.EnviaMensagem10();
                    client.Comunica2();
                    break;
            }
        }

        public void Update(object sender, EventArgs e)
        {
            UpdateObjects();
            Cleanup();

        }

        private void Cleanup()
        {
            foreach (ProtoSprite p in SpritesASeremDeletados)
            {
                Sprites.Remove(p);
            }
            SpritesASeremDeletados.Clear();
        }

        public void Draw(object sender, EventArgs e)
        {
            DrawObjects();

        }

        public void AnimateAll(object sender, EventArgs e)
        {
            AnimateObjects();

        }

        private void AnimateObjects()
        {
            foreach (ProtoSprite p in Sprites)
            {
                p.Animate();
            }
        }

        public void DestroiObjetoRemoto(ProtoSprite p1,ProtoSprite p2, int id) { 
            
            Obstaculo o = (Obstaculo)p2;
            switch (tanqueLocal)
            {                    
                case 1:
                    server.EnviaMensagem15("T", "BL" + o.Coluna + o.Fileira + string.Format("{0:00}",o.Linha),id);
                    break;
                case 2:
                    client.EnviaMensagem15("T", "BL" + o.Coluna + o.Fileira + string.Format("{0:00}", o.Linha), id);
                    break;
                    
            }
        }

        public void DestroiTiro(int id) {
            foreach (ProtoSprite p in Sprites)
            {
                if (p is Tirinho)
                {
                    Tirinho t = (Tirinho)p;
                    if (!t.Local) {
                        if (t.ID == id)
                        {
                            SpritesASeremDeletados.Add(t);
                        }
                    }
                }
            }
        
        }

        public void DestroiSprite(ProtoSprite p)
        {            
            SpritesASeremDeletados.Add(p); 
        }

        public void DestroiObstaculo(short col,short fil,short lin) {
            foreach (ProtoSprite p in Sprites)
            {
                if (p is Obstaculo)
                {
                    Obstaculo o = (Obstaculo)p;
                    if (o.Coluna == col)
                    {
                        if (o.Fileira == fil)
                        {
                            if (o.Linha == lin)
                            {                                
                             SpritesASeremDeletados.Add(o);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateObjects()
        {
            foreach (ProtoSprite p in Sprites)
            {
                p.Update();
            }
            try{
            switch (tanqueLocal)
            {
                    
                case 1:
                    Tanque1.InputMovimentação(TeclasDeMovimentoPressionada.Peek());
                    Tanque1.InputTiro(SpacePressed);
                    server.recebeTecla(TeclasDeMovimentoPressionada.Peek());
                    break;
                case 2:
                    Tanque2.InputMovimentação(TeclasDeMovimentoPressionada.Peek());
                    Tanque2.InputTiro(SpacePressed);
                    client.recebeTecla(TeclasDeMovimentoPressionada.Peek());
                    break;
                    }
            }
            catch(Exception e){

            }               
         
        
            
        }

        private void DrawObjects()
        {
            desenhista.Clear(Color.SandyBrown);
            foreach (ProtoSprite p in Sprites)
            {
                p.Draw(desenhista);
            }
            pictureBox1.Image = areaDeDesenho;
        }

        private void TeclaPressionada(object sender, KeyEventArgs e)
        {
            string tecla = e.KeyCode.ToString();
            if (tecla == "Left" || tecla == "Right" || tecla == "Down" || tecla == "Up")
            {
                if (!TeclasDeMovimentoPressionada.Contains(e.KeyCode.ToString()))
                {
                    TeclasDeMovimentoPressionada.Push(e.KeyCode.ToString());
                }
            }
            if (tecla == Keys.Space.ToString())
            {
                SpacePressed = true;
            }
        }

        private void TeclaLiberada(object sender, KeyEventArgs e)
        {
            List<string> a = new List<string>(TeclasDeMovimentoPressionada);
            a.Remove(e.KeyCode.ToString());
            TeclasDeMovimentoPressionada = new Stack<string>(a);

            if (e.KeyCode.ToString() == Keys.Space.ToString())
            {
                SpacePressed = false;
            }

        }


        private void TelaDeJogo_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        public List<ProtoSprite> GetCollisions(ProtoSprite este) {
            List<ProtoSprite> lista = new List<ProtoSprite>();
            foreach (ProtoSprite outro in Sprites)
            {
                if (este != outro)
                {
                    if (este.ColideCom(outro))
                    {
                        lista.Add(outro);
                    }
                }
            }


           
            return lista;            
            
        }



    }
}
