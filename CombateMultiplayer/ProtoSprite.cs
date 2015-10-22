using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombateMultiplayer
{
    public struct Position 
    {
        public float X;
        public float Y;
    }

    public struct Dimension
    {
        public float X;
        public float Y;
    }

    public class ProtoSprite 
    { 
        private Rectangle RetanguloDeDesenho;
        protected Rectangle RetanguloSpritesheet;
        public Dimension Dimension;
        public Position Position;
        public Point Resoluçao = new Point(800,600);

        /// <summary>
        /// 0 é esquerda; 1 pra cima; 2 direita; 3 pra baixo
        /// </summary>
        protected int Direçao; 
        protected int numQuadrosParado, numQuadrosMovendo;
        int QuadroAtual;
        protected bool isMoving=false;

        protected Image img = (Image)Properties.Resources.ResourceManager.GetObject("ImageNOTFOUND");

        public ProtoSprite(int x, int y,int largura,int altura ) {
            RetanguloDeDesenho = new Rectangle(x, y, largura, altura);
        }


        public void geraRetanguloDeDesenho(){
            RetanguloDeDesenho = new Rectangle((int)(Position.X * Resoluçao.X),(int)( Position.Y * Resoluçao.Y),(int)( Dimension.X * Resoluçao.X),(int)( Dimension.Y * Resoluçao.Y));
        
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        /// <param name="dimX">Dimensão do Sprite em X</param>
        /// <param name="dimY">Dimensão do Sprite em y</param>
        /// <param name="numQP">Número de quadros da animação parada</param>
        /// <param name="numQM">Número de quadros da animação movendo</param>
        public ProtoSprite(int x, int y, int largura, int altura, int dimX, int dimY, int numQP,int numQM,int resoluçaoX,int resoluçaoY)
        {
            Resoluçao = new Point(resoluçaoX, resoluçaoY);
            Position.X = x / resoluçaoX;
            Position.Y = y / resoluçaoY;
            Dimension.X = largura / resoluçaoX;
            Dimension.Y = altura / resoluçaoY;
            RetanguloDeDesenho = new Rectangle(x, y, largura, altura);
            RetanguloSpritesheet = new Rectangle(0, 0, dimX, dimY);
            numQuadrosParado = numQP;
            numQuadrosMovendo = numQM;
        }

        public void move(float x, float y){
            Position.X += x;
            Position.Y += y;
            RetanguloDeDesenho.X = (int)(Position.X * Resoluçao.X);
            RetanguloDeDesenho.Y = (int)(Position.Y * Resoluçao.Y);
        
        }


        public ProtoSprite()
        {
            RetanguloDeDesenho = new Rectangle(0, 0, 50, 050);
            RetanguloSpritesheet = new Rectangle(0, 0, 50, 50);
        }

        public virtual void Draw(Graphics desenhista){
            RetanguloSpritesheet.X = QuadroAtual * RetanguloSpritesheet.Width;
            RetanguloSpritesheet.Y = Direçao * RetanguloSpritesheet.Height;
            desenhista.DrawImage(img,RetanguloDeDesenho,RetanguloSpritesheet,GraphicsUnit.Pixel);
        
        }

        public virtual void Animate()
        {
            QuadroAtual++;
            if (!isMoving)
            {
                if (QuadroAtual >= numQuadrosParado)
                {
                    QuadroAtual = 0;
                }
            }
            else
            {
                if (QuadroAtual >= numQuadrosMovendo + numQuadrosParado)
                {
                    QuadroAtual = numQuadrosParado;
                }

            }
        }


        public virtual void Update()
        {
            
        }

        public bool ColideCom(ProtoSprite other)
        {
            if((this.Position.X>other.Position.X + other.Dimension.X)&&(other.Position.X<this.Position.X+this.Dimension.X)){
                if ((this.Position.Y > other.Position.Y + other.Dimension.Y) && (other.Position.Y < this.Position.Y + this.Dimension.Y))
                {
                    return true;
                }            
            }
            return false;
        }

    }
}
