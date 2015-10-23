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
        int tanqueLocal;
        System.Windows.Forms.Timer clockUpdate;
        System.Windows.Forms.Timer clockRefresh;
        System.Windows.Forms.Timer clockAnimation;
        GerenciadorDeRede server;
        GerenciadorDeRede client;

        Thread t;
        String IP;
        public List<ProtoSprite> Sprites;

        int Porto;

        bool gamePaused = true;

        Stack<string> TeclasDeMovimentoPressionada;

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

            Tanque1 = new Tanque(0, 0.6f, 2, this, DimensaoDaTelaX, DimensaoDaTelaY);
            Tanque2 = new Tanque(0.95f, 0.6f, 2, this, DimensaoDaTelaX, DimensaoDaTelaY);

            Sprites.Add(Tanque1);
            Sprites.Add(Tanque2);

            for (int v = 0; v < 10; v++)
            {
                Sprites.Add(new Obstaculo(0.15f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.16f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.85f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.86f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.30f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.31f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.70f, 0.1f + 0.09f * v, this));
                Sprites.Add(new Obstaculo(0.71f, 0.1f + 0.09f * v, this));
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
            t = new Thread(CriaRede);
            t.Start();

            clockUpdate = new System.Windows.Forms.Timer();
            clockUpdate.Tick += Update;
            clockUpdate.Interval = 5;

            clockRefresh = new System.Windows.Forms.Timer();
            clockRefresh.Tick += Draw;
            clockRefresh.Interval = 4;
            clockRefresh.Start();

            clockAnimation = new System.Windows.Forms.Timer();
            clockAnimation.Tick += AnimateAll;
            clockAnimation.Interval = 80;

        }

        public void PauseGame()
        {
            clockUpdate.Stop();
            clockAnimation.Stop();
        }

        public void StartGame() {

            pictureBox1.Invoke((MethodInvoker)delegate() { clockUpdate.Start(); });
            pictureBox1.Invoke((MethodInvoker)delegate() { clockAnimation.Start(); });
            
            
        }
        

        private void CriaRede(object a)
        {
            switch (tanqueLocal)
            {
                case 1:
                    server = new GerenciadorDeRede(Tanque2,Porto,null,false);
                    server.inicia();
                    server.EnviaMensagem10();
                    break;
                case 2:
                    client = new GerenciadorDeRede(Tanque1, Porto, IP, true);
                    client.EnviaMensagem10();
                    client.Comunica2();
                    break;
            }
        }

        public void Update(object sender, EventArgs e)
        {
            UpdateObjects();

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
                    Tanque1.Botoes(TeclasDeMovimentoPressionada.Peek());
                    server.recebeTecla(TeclasDeMovimentoPressionada.Peek());
                    break;
                case 2:
                    Tanque2.Botoes(TeclasDeMovimentoPressionada.Peek());
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
            if (!TeclasDeMovimentoPressionada.Contains(e.KeyCode.ToString()))
            {
                TeclasDeMovimentoPressionada.Push(e.KeyCode.ToString());
            }
        }

        private void TeclaLiberada(object sender, KeyEventArgs e)
        {
            List<string> a = new List<string>(TeclasDeMovimentoPressionada);
            a.Remove(e.KeyCode.ToString());
            TeclasDeMovimentoPressionada = new Stack<string>(a);
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
