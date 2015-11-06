using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    class PowerUp : ProtoSprite
    {
        TelaDeJogo Jogo;

        public PowerUp(float x, float y)
        {
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.05f;
            Dimension.Y = 0.05f;
            geraRetanguloDeDesenho();
            isSolid = false;
            // img = (Image)Properties.Resources.ResourceManager.GetObject("Obstaculo");
        }


    }
}