using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    class Tirinho:ProtoSprite
    {
        Image img = (Image)Properties.Resources.ResourceManager.GetObject("Tiro");
        float Velocidade = 0.01f;
        TelaDeJogo Jogo;
        public int ID;
        public bool Local;


        public Tirinho(float x,float y,int direçao,int id,bool isLocal,TelaDeJogo j)
        {
            ID = id;
            Local = isLocal;
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.0125f;
            Dimension.Y = 0.0125f;
            Jogo = j;
            geraRetanguloDeDesenho();
            Direçao = direçao;
            isSolid = false;
            RetanguloSpritesheet.Height = 2; //TODO Adequar ao sprite
          //img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            

        }
       
        public override void Update()
        {
            switch (Direçao)
            {
                case 0:
                    move(-Velocidade,0);  //tiro para esquerda
                    break;
                case 1:
                    move(0,-Velocidade); //tiro para cima
                    break;
                case 2:
                    move(Velocidade, 0); //tiro para direita
                    break;
                case 3:
                    move(0, Velocidade); //tiro para baixo
                    break;

            }
            if(Local)
            colideComAlvo();
        }

        public bool colideComAlvo()
        {
            foreach (ProtoSprite p in Jogo.GetCollisions(this))
            {
                if (p is Tanque)
                {
                    Tanque aux = (Tanque)p;
                    aux.Desativa();
                }
                if (p is Obstaculo)
                {
                    Destroi(p);
                    Jogo.DestroiObjetoRemoto(this, p, ID);
                    Destroi(this);
                }

            }
            return false;
        }

        private void Destroi(ProtoSprite p)
        {
            Jogo.SpritesASeremDeletados.Add(p);
        }

    }
}
