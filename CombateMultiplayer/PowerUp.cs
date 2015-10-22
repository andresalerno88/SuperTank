using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    class PowerUp:ProtoSprite
    {
        TelaDeJogo Jogo;
        public PowerUp(float x,float y,TelaDeJogo j)
        {
            Jogo = j;
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.01f;
            Dimension.Y = 0.08f;
            geraRetanguloDeDesenho();
            // img = (Image)Properties.Resources.ResourceManager.GetObject("Obstaculo");
        }
    }
}
