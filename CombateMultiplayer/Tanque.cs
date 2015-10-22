using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombateMultiplayer
{
    class Tanque : ProtoSprite
    {

        public TelaDeJogo Jogo;
        float Velocidade = 0.005f;

        public Tanque(float x,float y,int direçao, TelaDeJogo j,int resoluçaoX,int resoluçaoY)
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
        }

        public bool colideComSólido() {
            foreach (ProtoSprite p in Jogo.GetCollisions(this))
            {
                if (p.isSolid) {
                    return true;
                }
            }
            return false;            
        }


        public void Botoes(String dedadaDetectada)
        {
            if(dedadaDetectada == null){

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
                Direçao = 3;
            }
            if (dedadaDetectada == Keys.Up.ToString())
            {
                isMoving = true;
                move(0,-Velocidade);
                if (colideComSólido())
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
                Direçao = 0;
            }
            if (dedadaDetectada == Keys.Space.ToString())
            {
                switch (Direçao)
                {
                    case 0:
                        Jogo.Sprites.Add(new Tirinho(Position.X - 0.012f, Position.Y + 0.015f, 0));
                        break;
                    case 1:
                        Jogo.Sprites.Add(new Tirinho(Position.X + 0.015f, Position.Y + -0.012f, 1));
                        break;
                    case 2:
                        Jogo.Sprites.Add(new Tirinho(Dimension.X + 0.052f, Dimension.Y + 0.015f, 2));
                        break;
                    case 3:
                        Jogo.Sprites.Add(new Tirinho(Dimension.X + 0.015f, Dimension.Y + 0.052f, 3));
                        break;
                
                }
            }
        }
        /*
        public override void Draw(Graphics desenhista)
        {
            desenhista.DrawImage(img, AreaDeContato.X, AreaDeContato.Y);
        
        }
        */


    }
}
