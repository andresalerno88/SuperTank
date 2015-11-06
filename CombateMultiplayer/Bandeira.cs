using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    class Bandeira : ProtoSprite
    {
        TelaDeJogo Jogo;

        public int Dono;

        public Bandeira(float x, float y,int dono)
        {            
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.05f;
            Dimension.Y = 0.05f;
            geraRetanguloDeDesenho();
            Dono = dono;
            isSolid = false;
            //img = (Image)Properties.Resources.ResourceManager.GetObject("Obstaculo");
        }

    }
}