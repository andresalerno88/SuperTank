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
        short Coluna, Fileira, Linha;


        public Obstaculo(float x,float y,TelaDeJogo j, short col, short fil, short lin)
        {
            Jogo = j;
            Position.X = x;
            Position.Y = y;
            Dimension.X = 0.01f;
            Dimension.Y = 0.09f;
            geraRetanguloDeDesenho();

            Coluna = col;
            Fileira = fil;
            Linha = lin;

            // img = (Image)Properties.Resources.ResourceManager.GetObject("Obstaculo");
        }
       
    }
}
