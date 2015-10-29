using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombateMultiplayer
{


    public class Tanque : ProtoSprite
    {

        public TelaDeJogo Jogo;
        public float Velocidade = 1f * (GlobalConfigurations.UPDATEINTERVAL / 1000f);
        private System.Windows.Forms.Timer clockCanhao;
        private bool canhaoAtivado = true;
        private int TirosDisparados = 0;

        public Tanque(float x, float y, int direçao, TelaDeJogo j, int resoluçaoX, int resoluçaoY)
        {
            Jogo = j;
            Dimension.X = 0.05f;
            Dimension.Y = 0.05f;
            Position.X = x;
            Position.Y = y;
            Direçao = direçao;
            Resoluçao.X = resoluçaoX;
            Resoluçao.Y = resoluçaoY;
            geraRetanguloDeDesenho();
            img = (Image)Properties.Resources.ResourceManager.GetObject("Tank");
            RetanguloSpritesheet.Width = 82;
            RetanguloSpritesheet.Height = 90;
            numQuadrosParado = 3;
            numQuadrosMovendo = 4;

            clockCanhao = new System.Windows.Forms.Timer();
            clockCanhao.Tick += AtivaCanhao;
            clockCanhao.Interval = 500;
            clockCanhao.Start();

        }

        private void AtivaCanhao(object sender, EventArgs e)
        {
            canhaoAtivado = true;
            clockCanhao.Stop();

        }

        public void Desativa()
        {

        }

        public bool colideComSólido()
        {
            foreach (ProtoSprite p in Jogo.GetCollisions(this))
            {
                if (p.isSolid)
                {
                    return true;
                }
            }
            return false;
        }


        public void Botoes(String dedadaDetectada)
        {
            if (dedadaDetectada == null)
            {

                isMoving = false;
            }
            if (dedadaDetectada == Keys.Down.ToString())
            {
                isMoving = true;
                move(0, Velocidade);
                if (colideComSólido())
                {
                    move(0, -Velocidade);
                }
                else if (SaiuDaTela())
                {
                    move(0, -Velocidade);
                }
                Direçao = 3;
            }
            if (dedadaDetectada == Keys.Up.ToString())
            {
                isMoving = true;
                move(0, -Velocidade);
                if (colideComSólido())
                {
                    move(0, Velocidade);
                }
                else if (SaiuDaTela())
                {
                    move(0, Velocidade);
                }
                Direçao = 1;
            }
            if (dedadaDetectada == Keys.Right.ToString())
            {
                isMoving = true;
                move(Velocidade, 0);
                if (colideComSólido())
                {
                    move(-Velocidade, 0);
                }
                else if (SaiuDaTela())
                {
                    move(-Velocidade, 0);
                }
                Direçao = 2;
            }
            if (dedadaDetectada == Keys.Left.ToString())
            {
                isMoving = true;
                move(-Velocidade, 0);
                if (colideComSólido())
                {
                    move(Velocidade, 0);
                }
                else if (SaiuDaTela())
                {
                    move(Velocidade, 0);
                }
                Direçao = 0;
            }

            if (dedadaDetectada == Keys.Space.ToString())
            {
                if (canhaoAtivado)
                {
                    TirosDisparados++;
                    switch (Direçao)
                    {
                        case 0:
                            Jogo.Sprites.Add(new Tirinho(Position.X - 0.012f, Position.Y + 0.015f, 0,TirosDisparados,true, Jogo));
                            break;
                        case 1:
                            Jogo.Sprites.Add(new Tirinho(Position.X + 0.015f, Position.Y + -0.012f, 1, TirosDisparados, true, Jogo));
                            break;
                        case 2:
                            Jogo.Sprites.Add(new Tirinho(Position.X + 0.052f, Position.Y + 0.015f, 2, TirosDisparados, true, Jogo));
                            break;
                        case 3:
                            Jogo.Sprites.Add(new Tirinho(Position.X + 0.015f, Position.Y + 0.052f, 3, TirosDisparados, true, Jogo));
                            break;

                    }
                    canhaoAtivado = false;
                    clockCanhao.Start();
                }
            }
        }

        private bool SaiuDaTela()
        {
            if (this.Position.X < 0 || this.Position.Y <= 0.1 || this.Position.X + this.Dimension.X > 1 || this.Position.Y + this.Dimension.Y > 1)
            {

                return true;
            }

            return false;
        }
        /*
        public override void Draw(Graphics desenhista)
        {
            desenhista.DrawImage(img, AreaDeContato.X, AreaDeContato.Y);
        
        }
        */


    }
}
