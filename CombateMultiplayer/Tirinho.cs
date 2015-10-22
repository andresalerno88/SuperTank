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
        public Tirinho(int x,int y,int direçao)
        {
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.0125f;
            Dimension.Y = 0.0125f;
            geraRetanguloDeDesenho();
            Direçao = direçao;
          //img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            

        }
       
        public override void Update()
        {
            switch (Direçao)
            {
                case 0:
                    move(-Velocidade,0); 
                    break;
                case 1:
                    move(0,-Velocidade); 
                    break;
                case 2:
                    move(Velocidade, 0); 
                    break;
                case 3:
                    move(0, Velocidade);
                    break;

            }
        }
    }
}
