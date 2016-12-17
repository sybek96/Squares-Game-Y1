using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;


namespace AwesomeSquares
{
    enum blood { none, up, down, left, right };
    class WorldSquare
    {
        //variables
        blood checkBlood = blood.none;
        Texture2D wallTexture;
        Texture2D wallBloodUpTexture;
        Texture2D wallBloodDownTexture;
        Texture2D wallBloodLeftTexture;
        Texture2D wallBloodRightTexture;
        Texture2D emptyTexture;
        Vector2 position = new Vector2(-50,-50);
        int squareContents;
        const int size = 50;

        /// <summary>
        /// Default constructor of a world square
        /// </summary>
        public WorldSquare()
        {
            squareContents = 0;
        }
        /// <summary>
        /// Loading content of the world block
        /// </summary>
        /// <param name="theContentManager"></param>
        public void LoadContent(ContentManager theContentManager)
        {
            wallTexture = theContentManager.Load<Texture2D>("Wall");
            wallBloodUpTexture = theContentManager.Load<Texture2D>("WallBloodUp");
            wallBloodDownTexture = theContentManager.Load<Texture2D>("WallBloodDown");
            wallBloodLeftTexture = theContentManager.Load<Texture2D>("WallBloodLeft");
            wallBloodRightTexture = theContentManager.Load<Texture2D>("WallBloodRight");
            emptyTexture = theContentManager.Load<Texture2D>("EmptySpace");
        }
        //Draw method for the world block
        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (squareContents == 1)
            {
                switch (checkBlood)
                {
                    case blood.none:
                        theSpriteBatch.Draw(wallTexture, position, Color.White);
                        break;
                    case blood.up:
                        theSpriteBatch.Draw(wallBloodUpTexture, position, Color.White);
                        break;
                    case blood.down:
                        theSpriteBatch.Draw(wallBloodDownTexture, position, Color.White);
                        break;
                    case blood.left:
                        theSpriteBatch.Draw(wallBloodLeftTexture, position, Color.White);
                        break;
                    case blood.right:
                        theSpriteBatch.Draw(wallBloodRightTexture, position, Color.White);
                        break;
                    default:
                        break;
                }
            }
            if(squareContents == 0)
            {
                theSpriteBatch.Draw(emptyTexture, position, Color.White);
            }
        }
        /// <summary>
        /// Property for position
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        /// <summary>
        /// property of x position
        /// </summary>
        public float PositionX
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }
        /// <summary>
        /// property of y position
        /// </summary>
        public float PositionY
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }
        /// <summary>
        /// Property for the square contain
        /// </summary>
        public int ContainsSquare
        {
            get
            {
                return squareContents;
            }
            set
            {
                squareContents = value;
            }
        }
        /// <summary>
        /// A property for size allows to get the size
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
        }
        /// <summary>
        /// get and set the blood enum
        /// </summary>
        public blood CheckBlood
        {
            get
            {
                return checkBlood;
            }
            set
            {
                checkBlood = value;
            }
        }
    }
}
