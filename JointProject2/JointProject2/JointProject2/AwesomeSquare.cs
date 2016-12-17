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
    enum direction { Up, Down, Left, Right };       //an enumerator to get the direction
    class AwesomeSquare
    {
        //Variables
        Texture2D awesomeSquareTexture;
        Rectangle awesomeSquareRectangle;
        Texture2D awesomeEyeTexture;
        Rectangle awesomeEyeRectangle;
        Vector2 position = new Vector2(0,0);
        int row = 0;
        int column = 0;
        int eyePosX;
        int eyePosY;
        int health = 100;
        int score = 0;
        string name;
        direction heading;
        bool alive;
        int size = 50;
        int eyeSize = 5;

        /// <summary>
        /// default constructor
        /// </summary>
        public AwesomeSquare()
        {
            heading = direction.Right;
            alive = true;
            awesomeSquareRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            awesomeEyeRectangle = new Rectangle(eyePosX, eyePosY, eyeSize, eyeSize);

        }
        /// <summary>
        /// Loading up the awesome square texture
        /// </summary>
        /// <param name="theContentManager"></param>
        public void LoadContent(ContentManager theContentManager)
        {
            awesomeSquareTexture = theContentManager.Load<Texture2D>("AwesomeSquare");
            awesomeEyeTexture = theContentManager.Load<Texture2D>("Eye");
        }
        /// <summary>
        /// Update the variables
        /// </summary>
        public void Update()
        {
            position.X = 0 + (column * size);
            position.Y = 0 + (row * size);
            awesomeSquareRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            EyeUpdate();
            awesomeEyeRectangle = new Rectangle(eyePosX, eyePosY, eyeSize, eyeSize);

        }
        /// <summary>
        /// Drawing the awesome square
        /// </summary>
        /// <param name="theSpriteBatch"></param>
        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (alive)
            {
                theSpriteBatch.Draw(awesomeSquareTexture,awesomeSquareRectangle,Color.White);
            }
            theSpriteBatch.Draw(awesomeEyeTexture, awesomeEyeRectangle, Color.White);
        }
        /// <summary>
        /// Update the eye on the player
        /// </summary>
        private void EyeUpdate()
        {
            switch (heading)
            {
                case direction.Up:
                    eyePosX = (int)position.X + 22;
                    eyePosY = (int)position.Y + 10;
                    break;
                case direction.Down:
                    eyePosX = (int)position.X + 22;
                    eyePosY = (int)position.Y + 40;
                    break;
                case direction.Left:
                    eyePosX = (int)position.X + 10;
                    eyePosY = (int)position.Y + 22;
                    break;
                case direction.Right:
                    eyePosX = (int)position.X + 40;
                    eyePosY = (int)position.Y + 22;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// will write the player details in an XML file
        /// </summary>
        /// <param name="outFile"></param>
        public void WriteToXMLFile(XmlWriter outFile)
        {
            outFile.WriteStartElement("awesomeSquareObject");

            outFile.WriteStartElement("row");
            outFile.WriteString(Convert.ToString(Row));
            outFile.WriteEndElement();

            outFile.WriteStartElement("column");
            outFile.WriteString(Convert.ToString(Column));
            outFile.WriteEndElement();

            outFile.WriteStartElement("name");
            outFile.WriteString(Name);
            outFile.WriteEndElement();

            outFile.WriteStartElement("health");
            outFile.WriteString(Convert.ToString(Health));
            outFile.WriteEndElement();

            outFile.WriteStartElement("score");
            outFile.WriteString(Convert.ToString(Score));
            outFile.WriteEndElement();

            outFile.WriteStartElement("heading");
            outFile.WriteString(Convert.ToString(Heading));
            outFile.WriteEndElement();

            outFile.WriteEndElement();
        }
        //**************PROPERTIES**************//
        /// <summary>
        /// Row property allows to get and set the row value
        /// </summary>
        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }
        /// <summary>
        /// Property of the column allows to get and set the column
        /// </summary>
        public int Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
            }
        }
        /// <summary>
        /// A property of the heading enumerator allows to get and set the value
        /// </summary>
        public direction Heading
        {
            get
            {
                return heading;
            }
            set
            {
                heading = value;
            }
        }
        /// <summary>
        /// Allows to get and set the name
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        /// <summary>
        /// get and set health
        /// </summary>
        public int Health
        {
            get
            {
                return health; 
            }
            set
            {
                health = value;
            }
        }
        /// <summary>
        /// get and set the score
        /// </summary>
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }
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
    }
}
