using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    class Obstaculo:ProtoSprite
    {
        TelaDeJogo Jogo;

        public Obstaculo(float x,float y,TelaDeJogo j)
        {
            Jogo = j;
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.01f;
            Dimension.Y = 0.09f;
            geraRetanguloDeDesenho();
            // img = (Image)Properties.Resources.ResourceManager.GetObject("Obstaculo");
        }
       
    }
}
