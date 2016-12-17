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
    class EvilSquare
    {
        //variables
        direction heading;
        int headingChoice;
        int counter = 0;
        Texture2D enemyTexture;
        Rectangle enemyRectangle;
        Vector2 position = new Vector2(0,0);
        int row = 0;
        int col = 0;
        bool alive = true;
        const int size = 50;
        Random rnd = new Random();
        /// <summary>
        /// Default constructor
        /// </summary>
        public EvilSquare(int startingRow, int startingCol)
        {

            position.X = 0 + (startingCol * size);    //set up the x position of the evil square by multiplying its row by the size and adding it to 0
            position.Y = 0 + (startingRow * size);
            row = startingRow;
            col = startingCol;
            enemyRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            NewHeading();
        }
        /// <summary>
        /// Load up the image of the enemy
        /// </summary>
        /// <param name="theContentManager"></param>
        public void LoadContent(ContentManager theContentManager)
        {
            enemyTexture = theContentManager.Load<Texture2D>("EvilSquare");
        }
        /// <summary>
        /// Update the enemy
        /// </summary>
        public void Update(WorldSquare[,] worldMap)
        {
            counter++;
            if (counter == 25)
            {
                 switch (heading)    //first check if on edge and if not then check if cell that it tries to move into contains a block else pick a new direction to move
                {
                    case direction.Up:  //if up
                        if (row > 0 && worldMap[row - 1, col].ContainsSquare == 0)  //check if not on edge and if there is no square in above cell
                        {
                            GoUp(); //if requirements met go up a cell
                        }
                        else
                        {
                            NewHeading();   //if requirements not met, get a new heading
                        }
                        break;
                    case direction.Down:
                        if (row < 13 && worldMap[row + 1, col].ContainsSquare == 0)
                        {
                            GoDown();
                        }
                        else
                        {
                            NewHeading();
                        }
                        break;
                    case direction.Left:
                        if (col > 0 && worldMap[row, col - 1].ContainsSquare == 0)
                        {
                            GoLeft();
                        }
                        else
                        {
                            NewHeading();
                        }
                        break;
                    case direction.Right:
                        if (col < 13 && worldMap[row,col + 1].ContainsSquare == 0)
                        {
                            GoRight();
                        }
                        else
                        {
                            NewHeading();
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Draw if its alive
        /// </summary>
        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (alive)
            {
                theSpriteBatch.Draw(enemyTexture, enemyRectangle, Color.White);
            }
        }
        /// <summary>
        /// method for moving up within the array
        /// </summary>
        private void GoUp()
        {
            row -= 1;
            position.Y = 0 + (row * size);
            enemyRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            counter = 0;
        }
        /// <summary>
        /// method for moving down within the array
        /// </summary>
        private void GoDown()
        {
            row += 1;
            position.Y = 0 + (row * size);
            enemyRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            counter = 0;
        }
        /// <summary>
        /// method for moving left within the array
        /// </summary>
        private void GoLeft()
        {
            col -= 1;
            position.X = 0 + (col * size);
            enemyRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            counter = 0;
        }
        /// <summary>
        /// Method for moving right within the array
        /// </summary>
        private void GoRight()
        {
            col += 1;
            position.X = 0 + (col * size);
            enemyRectangle = new Rectangle((int)position.X, (int)position.Y, size, size);
            counter = 0;
        }
        /// <summary>
        /// Pick a new heading using a random number generator
        /// </summary>
        public void NewHeading()
        {
            headingChoice = rnd.Next(4);
            if (headingChoice == 0)
            {
                heading = direction.Up;
            }
            if (headingChoice == 1)
            {
                heading = direction.Down;
            }
            if (headingChoice == 2)
            {
                heading = direction.Left;
            }
            if (headingChoice == 3)
            {
                heading = direction.Right;
            }
            counter = 0;
        }
        /// <summary>
        /// A method that sets the alive bool to false
        /// </summary>
        public void Death()
        {
            alive = false;
        }
        /// <summary>
        /// Respawns the enemy by setting its alive to true and give it a new position
        /// </summary>
        /// <param name="startingRow"></param>
        /// <param name="startingCol"></param>
        public void Respawn(int startingRow,int startingCol)
        {
            row = startingRow;
            col = startingCol;
            alive = true;
            NewHeading();
            position.Y = 0 + (startingRow * size);
            position.X = 0 + (startingCol * size);
        }
        //**************PROPERTIES**************//
        /// <summary>
        /// Property for the row allows to get and set the value
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
        /// Property for column allows to get and set the value
        /// </summary>
        public int Col
        {
            get
            {
                return col;
            }
            set
            {
                col = value;
            }
        }
        /// <summary>
        /// Property for the heading allows to get and set the heading
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
        /// Property for the alive bool allows to get and set the value
        /// </summary>
        public bool Alive
        {
            get
            {
                return alive;
            }
            set
            {
                alive = value;
            }
        }
        /// <summary>
        /// Will write all the evil square details to an XML file
        /// </summary>
        /// <param name="outFile"></param>
        public void WriteToXMLFile(XmlTextWriter outFile)
        {
            outFile.WriteStartElement("evilSquareObject");

            outFile.WriteStartElement("row");
            outFile.WriteString(Convert.ToString(Row));
            outFile.WriteEndElement();

            outFile.WriteStartElement("column");
            outFile.WriteString(Convert.ToString(Col));
            outFile.WriteEndElement();

            outFile.WriteStartElement("alive");
            outFile.WriteString(Convert.ToString(Alive));
            outFile.WriteEndElement();

            outFile.WriteStartElement("heading");
            outFile.WriteString(Convert.ToString(Heading));
            outFile.WriteEndElement();

            outFile.WriteEndElement();
        }
    }
}
