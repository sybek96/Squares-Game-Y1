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

/*
 * Joint Project 2: Awesome Squares
 * 
 * Name: Sebastian Kruzel
 * Student number: C00206244
 * Date Started: 26/03/2016
 * Know n Bugs: Right and down wall kills do not cause blood, when dying enemies drawn for a little while, enemies may push the player into a wall space, saving file 2 times in one game causes game to exit
 * extras:  unable to kick blocks if another block blocking the path
 *          blood appears on a block if enemy killed
 *          the name input has a self centering string
 *          saving player object which can also be loaded
 *          enemies can push the player
 *          if player pushed out of bounds they appear on the other side of the screen
 *          once all enemies dead new wave spawns
 *          added sound
 */
namespace AwesomeSquares
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState { NameInput, GamePlay, Instructions, GameOver, Victory };

        GameState currentState = GameState.NameInput;
        Song backGroundMusic;
        SoundEffect walkSound;
        SoundEffect wallKickSound;
        Texture2D nameInputImage;
        Texture2D instructionsImage;
        Texture2D gameOverImage;
        Texture2D victoryImage;
        KeyboardState oldState;
        WorldSquare[,] worldMap = new WorldSquare[14,14];
        EvilSquare[] enemies = new EvilSquare[2];
        Keys[] oldKeys;
        AwesomeSquare player;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle mainFrame;
        SpriteFont font;
        Vector2 textPos = new Vector2(300, 280);
        int wave;
        string playerName;
        static int windowHeight = 700;
        static int windowWidth = 700;
        public const int blockSize = 50;
        const int maxRow = 14;
        const int maxCol = 14;
        bool enemyKilled = false;   //used to check if enemy killed, if yes do blood effect on block
        int[,] mapContents =       {    {0, 0, 0, 0, 0 ,0 ,0, 0, 0, 0, 0, 0 ,0 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0, 0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0 ,0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 1, 0 ,1 ,0, 0, 1, 0, 1, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0 ,0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0, 0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 0, 0, 0, 0 ,0 ,0 ,0, 0, 0, 0, 0 ,0 ,0},
                                        {0, 0, 0, 0, 0 ,0 ,0, 0, 0, 0, 0, 0 ,0 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0, 0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0 ,0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 1, 0 ,1 ,0, 0, 1, 0, 1, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0 ,0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 1, 0, 0, 0 ,1 ,0, 0, 1, 0, 0, 0 ,1 ,0},
                                        {0, 0, 0, 0, 0 ,0 ,0 ,0, 0, 0, 0, 0 ,0 ,0}
                                                                                      };                //will set up the map as shown where 1 is a wall and 0 is an empty space
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initializes all the components of the game
        /// creates new objects and initializes variables
        /// </summary>
        protected override void Initialize()
        {
            oldState = Keyboard.GetState();
            player = new AwesomeSquare();
            int blockPositionX = 0;
            int blockPositionY = 0;
            int row;
            int column;
            for (row = 0; row < maxRow; row++)
            {
                for (column = 0; column < maxCol; column++)
                {
                    worldMap[row, column] = new WorldSquare();
                    worldMap[row, column].PositionX = blockPositionX;
                    worldMap[row, column].PositionY = blockPositionY;
                    blockPositionX += blockSize;
                }
                blockPositionY += blockSize;
                blockPositionX = 0;
            }
            enemies[0] = new EvilSquare(0, 13);
            enemies[1] = new EvilSquare(13, 0);
            SetUpMap();
            playerName = "";
            wave = 1;
            base.Initialize();
        }

        /// <summary>
        /// This method will Load up all the content of the game
        /// </summary>
        protected override void LoadContent()
        {
            for (int row = 0; row < maxRow; row++)  //load up the whole map in a for loop
            {
                for (int column = 0; column < maxCol; column++)
                {
                    worldMap[row,column].LoadContent(this.Content);
                }
            }
            for (int i = 0; i < enemies.Length; i++)    //load up the enemies in a for loop
            {
                enemies[i].LoadContent(this.Content);
            }
            player.LoadContent(this.Content);
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.ApplyChanges();
            windowWidth = graphics.GraphicsDevice.Viewport.Width;
            windowHeight = graphics.GraphicsDevice.Viewport.Height;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            nameInputImage = this.Content.Load<Texture2D>("name_input");
            instructionsImage = this.Content.Load<Texture2D>("GameInstructions");
            gameOverImage = this.Content.Load<Texture2D>("GameOver");
            victoryImage = this.Content.Load<Texture2D>("WinScreen");
            font = Content.Load<SpriteFont>("Font");
            backGroundMusic = this.Content.Load<Song>("Song");
            MediaPlayer.Play(backGroundMusic);
            walkSound = this.Content.Load<SoundEffect>("WalkSound");
            wallKickSound = this.Content.Load<SoundEffect>("WallKick");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Instructions:
                    KeyboardHandling();
                    break;
                case GameState.NameInput:
                    KeyboardHandling();
                    break;
                case GameState.GamePlay:
                    KeyboardHandling(); //handles all user inputs
                    for (int i = 0; i < enemies.Length; i++)    //update all enemies
                    {
                        enemies[i].Update(worldMap);
                    }
                    CheckIfEnemiesCollide(enemies[0], enemies[1]); //check collision between enemies
                    player.Update();    //update the player
                    PlayerHitCheck(enemies);
                    GameOverCheck();
                    base.Update(gameTime);
                    break;
                case GameState.GameOver:
                    KeyboardHandling();
                    break;
                case GameState.Victory:
                    KeyboardHandling();
                    break;
                default:
                    currentState = GameState.NameInput;
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (currentState)
            {
                case GameState.Instructions:
                    spriteBatch.Draw(instructionsImage, mainFrame, Color.White);
                    break;
                case GameState.NameInput:
                    spriteBatch.Draw(nameInputImage, mainFrame, Color.White);
                    spriteBatch.DrawString(font, "INPUT NAME ", new Vector2(280, 250), Color.White);
                    spriteBatch.DrawString(font, playerName, textPos, Color.Blue);
                    break;
                case GameState.GamePlay:
                    for (int row = 0; row < maxRow; row++)
                    {
                        for (int column = 0; column < maxCol; column++)
                        {
                            worldMap[row,column].Draw(this.spriteBatch);
                        }
                    }
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        enemies[i].Draw(this.spriteBatch);
                    }
                    player.Draw(this.spriteBatch);
                    spriteBatch.DrawString(font, "Player name: " + playerName, new Vector2(290, 670), Color.Blue);
                    spriteBatch.DrawString(font,"Health: " + player.Health.ToString(), new Vector2(100, 670), Color.Blue);
                    spriteBatch.DrawString(font, "WAVE: " + wave, new Vector2(10, 10), Color.Red);
                    spriteBatch.DrawString(font, "Score: " + player.Score, new Vector2(500, 10), Color.Red);
                    break;
                case GameState.GameOver:
                    spriteBatch.Draw(gameOverImage, mainFrame, Color.White);
                    break;
                case GameState.Victory:
                    spriteBatch.Draw(victoryImage, mainFrame, Color.White);
                    break;
                default:
                    currentState = GameState.NameInput;
                    break;
            }
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// Handles all user keyboard inputs
        /// </summary>
        private void KeyboardHandling()
        {
            switch (currentState)
            {
                case GameState.NameInput:
                    NameInput();
                    break;
                case GameState.Instructions:
                    KeyboardState currentKeys = Keyboard.GetState();
                    if (currentKeys.IsKeyDown(Keys.Space))
                    {
                        currentState = GameState.GamePlay;
                    }
                    break;
                case GameState.GamePlay:
                    PlayerInputs(walkSound);
                    break;
                case GameState.GameOver:
                    EndGameInput();
                    break;
                case GameState.Victory:
                    EndGameInput();
                    break;
                default:
                    currentState = GameState.NameInput;
                    break;
            }
        }
        /// <summary>
        /// will check if player wants to quit or continue
        /// </summary>
        private void EndGameInput()
        {
            KeyboardState currentKeys = Keyboard.GetState();
            if (currentKeys.IsKeyDown(Keys.R))
            {
                Initialize();
                currentState = GameState.NameInput;
            }
            if (currentKeys.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }
        /// <summary>
        /// Deals with the player name, takes in input via keyboard checking its previous and current state
        /// if new key pressed appear on screen, enter to  confirm name
        /// </summary>
        private void NameInput()
        {
            bool letterFound = false;
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Enter)) //once enter pressed go into game
            {
                player.Name = playerName;
                currentState = GameState.Instructions;
            }
            Keys[] pressedKeys; //array of currently pressed keys
            pressedKeys = keyState.GetPressedKeys();    //set to currently pressed keys
            for (int i = 0; i < pressedKeys.Length; i++)    //go through array of currently pressed keys
            {
                letterFound = false;    //key has not been found yet
                for (int j = 0; j < oldKeys.Length; j++)
                {
                    if (pressedKeys[i] == oldKeys[j])   //check if the previously pressed key is same as the current
                    {
                        letterFound = true; //if yes that key is found
                        break;
                    }
                }
                if (letterFound == false)   //if not found concatinate to the string
                {
                    string keyString = "";
                    switch (pressedKeys[i])
                    {
                        case Keys.A:
                            keyString = "a";
                            break;
                        case Keys.B:
                            keyString = "b";
                            break;
                        case Keys.C:
                            keyString = "c";
                            break;
                        case Keys.D:
                            keyString = "d";
                            break;
                        case Keys.D0:
                            keyString = "0";
                            break;
                        case Keys.D1:
                            keyString = "1";
                            break;
                        case Keys.D2:
                            keyString = "2";
                            break;
                        case Keys.D3:
                            keyString = "3";
                            break;
                        case Keys.D4:
                            keyString = "4";
                            break;
                        case Keys.D5:
                            keyString = "5";
                            break;
                        case Keys.D6:
                            keyString = "6";
                            break;
                        case Keys.D7:
                            keyString = "7";
                            break;
                        case Keys.D8:
                            keyString = "8";
                            break;
                        case Keys.D9:
                            keyString = "9";
                            break;
                        case Keys.E:
                            keyString = "e";
                            break;
                        case Keys.F:
                            keyString = "f";
                            break;
                        case Keys.G:
                            keyString = "g";
                            break;
                        case Keys.H:
                            keyString = "h";
                            break;
                        case Keys.I:
                            keyString = "i";
                            break;
                        case Keys.J:
                            keyString = "j";
                            break;
                        case Keys.K:
                            keyString = "k";
                            break;
                        case Keys.L:
                            keyString = "l";
                            break;
                        case Keys.M:
                            keyString = "m";
                            break;
                        case Keys.N:
                            keyString = "n";
                            break;
                        case Keys.O:
                            keyString = "o";
                            break;
                        case Keys.OemBackslash:
                            keyString = @"\";
                            break;
                        case Keys.OemCloseBrackets:
                            keyString = ")";
                            break;
                        case Keys.OemComma:
                            keyString = ",";
                            break;
                        case Keys.OemOpenBrackets:
                            keyString = "(";
                            break;
                        case Keys.OemPeriod:
                            keyString = ".";
                            break;
                        case Keys.P:
                            keyString = "p";
                            break;
                        case Keys.Q:
                            keyString = "q";
                            break;
                        case Keys.R:
                            keyString = "r";
                            break;
                        case Keys.S:
                            keyString = "s";
                            break;
                        case Keys.Space:
                            keyString = " ";
                            break;
                        case Keys.T:
                            keyString = "t";
                            break;
                        case Keys.U:
                            keyString = "u";
                            break;
                        case Keys.V:
                            keyString = "v";
                            break;
                        case Keys.W:
                            keyString = "w";
                            break;
                        case Keys.X:
                            keyString = "x";
                            break;
                        case Keys.Y:
                            keyString = "y";
                            break;
                        case Keys.Z:
                            keyString = "z";
                            break;
                        default:
                            break;
                    }
                    if (keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))  //if shift pressed go upper case
                    {
                        keyString = keyString.ToUpper();
                    }
                    if (pressedKeys[i] == Keys.Back)    //if backspace remove char at last position provided the string is not empty
                    {
                        if (playerName.Length > 0)
                        {
                            playerName = playerName.Remove(playerName.Length - 1);
                            CheckTextPosition();
                        }
                    }
                    if (playerName.Length < 17) //limit the amount of letters player can input
                    {
                        playerName += keyString;
                        CheckTextPosition();
                    }
                }
            }
            oldKeys = pressedKeys;  //old state is equal the new state

        }
        /// <summary>
        /// Moving around the map
        /// depending on key press checks the next cell if its inside the bounds and no wall
        /// if space is free player may move into the space
        /// </summary>
        private void PlayerInputs(SoundEffect effect)
        {
            KeyboardState newState = Keyboard.GetState();
            //is a key down
            if (newState.IsKeyDown(Keys.Up))
            {
                effect.Play();
                player.Heading = direction.Up;
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Up))
                {
                    if (player.Row > 0 && AboveFree())     //boundary check + wall check
                    {
                        player.Row -= 1;    //change the location
                    }
                }
            }
            if (newState.IsKeyDown(Keys.Down))
            {
                effect.Play();
                player.Heading = direction.Down;
                if (!oldState.IsKeyDown(Keys.Down))
                {
                    if (player.Row < maxRow - 1 && BelowFree())
                    {
                        player.Row += 1;
                    }
                }
            }
            if (newState.IsKeyDown(Keys.Left))
            {
                effect.Play();
                player.Heading = direction.Left;
                if (!oldState.IsKeyDown(Keys.Left))
                {
                    if (player.Column > 0 && LeftFree())
                    {
                        player.Column -= 1;
                    }
                }
            }
            if (newState.IsKeyDown(Keys.Right))
            {
                effect.Play();
                player.Heading = direction.Right;
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    if (player.Column < maxCol - 1 && RightFree())
                    {
                        player.Column += 1;
                    }
                }
            }
            if (newState.IsKeyDown(Keys.Space))
            {
                //check if was not pressed last cycle
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    //if the player is facing right
                    if (player.Heading == direction.Right)
                    {
                        KickRightBlock();
                    }
                    else if (player.Heading == direction.Left)
                    {
                        KickLeftBlock();
                    }
                    else if (player.Heading == direction.Up)
                    {
                        KickAboveBlock();
                    }
                    else if (player.Heading == direction.Down)
                    {
                        KickBelowBlock();
                    }
                }
            }
            if (newState.IsKeyDown(Keys.S))
            {
                if (!oldState.IsKeyDown(Keys.S))
                {
                    SaveToXMLFile();
                }
            }
            if (newState.IsKeyDown(Keys.L))
            {
                if (!oldState.IsKeyDown(Keys.L))
                {
                    LoadFromXMLFile();
                }
            }
            // Update saved state.
            oldState = newState;
        }
        /// <summary>
        /// Will check cell right of player if wall there move it right keep moving while space is available within the array bounds
        /// </summary>
        private void KickRightBlock()
        {
            int blockToPushRow = player.Row;
            int blockToPushCol = player.Column + 1;
            //check if there is a wall to the right of the player if yes move it
            while (blockToPushCol < maxCol - 1 && worldMap[blockToPushRow, blockToPushCol].ContainsSquare == 1 && worldMap[blockToPushRow, blockToPushCol + 1].ContainsSquare == 0)
            {
                Delay();
                wallKickSound.Play();
                worldMap[blockToPushRow, blockToPushCol].ContainsSquare = 0;        //make cell with wall an empty space
                worldMap[blockToPushRow, blockToPushCol + 1].ContainsSquare = 1;    //make the next cell have a wall
                blockToPushCol += 1;    //update the position of the wall that is being pushed
                CheckIfEnemyHit(blockToPushRow,blockToPushCol);
            }
            if(enemyKilled)
            {
                worldMap[blockToPushRow, blockToPushCol].CheckBlood = blood.right;
                enemyKilled = false;
                player.Score += 50;
            }
        }
        /// <summary>
        /// will check cell right of player if a wall is there move it left and keep moving it while space is available within the array bounds
        /// </summary>
        private void KickLeftBlock()
        {
            int blockToPushRow = player.Row;
            int blockToPushCol = player.Column - 1;
            while(blockToPushCol > 0 && worldMap[blockToPushRow,blockToPushCol].ContainsSquare == 1 && worldMap[blockToPushRow,blockToPushCol - 1].ContainsSquare == 0)
            {
                Delay();
                wallKickSound.Play();
                worldMap[blockToPushRow,blockToPushCol].ContainsSquare = 0;
                worldMap[blockToPushRow,blockToPushCol - 1].ContainsSquare = 1;
                blockToPushCol -= 1;
                CheckIfEnemyHit(blockToPushRow, blockToPushCol);
            }
            if (enemyKilled)
            {
                worldMap[blockToPushRow, blockToPushCol].CheckBlood = blood.left;
                enemyKilled = false;
                player.Score += 50;
            }
        }
        /// <summary>
        /// checks cell above the player if a wall is there move it up and keep moving it while there is space within array
        /// </summary>
        private void KickAboveBlock()
        {
            int blockToPushRow = player.Row - 1;
            int blockToPushCol = player.Column;
            while (blockToPushRow > 0 && worldMap[blockToPushRow, blockToPushCol].ContainsSquare == 1 && worldMap[blockToPushRow - 1, blockToPushCol].ContainsSquare == 0)
            {
                Delay();
                wallKickSound.Play();
                worldMap[blockToPushRow, blockToPushCol].ContainsSquare = 0;
                worldMap[blockToPushRow - 1, blockToPushCol].ContainsSquare = 1;
                blockToPushRow -= 1;
                CheckIfEnemyHit(blockToPushRow, blockToPushCol);
            }
            if (enemyKilled)
            {
                worldMap[blockToPushRow, blockToPushCol].CheckBlood = blood.up;
                enemyKilled = false;
                player.Score += 50;
            }
        }
        /// <summary>
        /// checks cell below the player if a wall is there move it down and keep it moving while there is space in the array
        /// </summary>
        private void KickBelowBlock()
        {
            int blockToPushRow = player.Row + 1;
            int blockToPushCol = player.Column;
            while (blockToPushRow < maxRow - 1 && worldMap[blockToPushRow, blockToPushCol].ContainsSquare == 1 && worldMap[blockToPushRow + 1, blockToPushCol].ContainsSquare == 0)
            {
                Delay();
                wallKickSound.Play();
                worldMap[blockToPushRow, blockToPushCol].ContainsSquare = 0;
                worldMap[blockToPushRow + 1, blockToPushCol].ContainsSquare = 1;
                blockToPushRow += 1;
                CheckIfEnemyHit(blockToPushRow, blockToPushCol);
            }
            if (enemyKilled)
            {
                worldMap[blockToPushRow, blockToPushCol].CheckBlood = blood.down;
                enemyKilled = false;
                player.Score += 50;
            }
        }
        /// <summary>
        /// checks block above the player
        /// </summary>
        /// <returns></returns>
        private bool AboveFree()
        {
            if (worldMap[player.Row - 1, player.Column].ContainsSquare == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// checks block below the player
        /// </summary>
        /// <returns></returns>
        private bool BelowFree()
        {
            if (worldMap[player.Row + 1, player.Column].ContainsSquare == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// check cell left of player
        /// </summary>
        /// <returns></returns>
        private bool LeftFree()
        {
            if (worldMap[player.Row, player.Column - 1].ContainsSquare == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// check cell right of player
        /// </summary>
        /// <returns></returns>
        private bool RightFree()
        {
            if (worldMap[player.Row, player.Column + 1].ContainsSquare == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// a method that sets up a given stage 
        /// uses the array of integers within the 2d array of cells to figure out if its a wall or empty space
        /// </summary>
        private void SetUpMap()
        {

            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    worldMap[row, col].ContainsSquare = mapContents[row,col];
                }
            }
        }
        /// <summary>
        ///adding aritificial delay so things don't happen so sudden increments a counter that goes up to 19000000
        /// </summary>
        private void Delay()
        {
            int counter = 0;
            while (counter < 19000)
            {
                counter++;
            }
            counter = 0;
        }
        /// <summary>
        /// checks if the row/column of a kicked block is the same as the row/column of the enemy
        /// </summary>
        private void CheckIfEnemyHit(int kickedBlockRow, int kickedBlockCol)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].Row == kickedBlockRow && enemies[i].Col == kickedBlockCol)
                {
                    enemies[i].Death();
                    enemyKilled = true;
                }
            }
            if (enemies[0].Alive == false && enemies[1].Alive == false)
            {
                RespawnEnemies();
            }
        }
        /// <summary>
        /// Will see if each of the enemies are colliding with the player
        /// checks if each enemy will occupy the same space as the player, if yes the player is pushed back
        /// if at boundary player will be pushed to the other side of the screen
        /// </summary>
        /// <param name="enemies"></param>
        private void PlayerHitCheck(EvilSquare[] enemies)
        {
            for (int i = 0; i < enemies.Length; i++)    //check through each alive enemy
            {
                if (enemies[i].Alive == true)
                {
                    if (enemies[i].Row == player.Row && enemies[i].Col == player.Column)    //check if row and col are same as player    if yes do appropriate movement
                    {
                        switch (enemies[i].Heading)
                        {
                            case direction.Up:
                                if (player.Row > 0)
                                {
                                    player.Row -= 1;    //push player back 2 spaces
                                }
                                else                            //OR
                                {
                                    player.Row = maxRow - 1;    //move player to other side
                                }
                                break;
                            case direction.Down:        //Do for all directions and boundary limits
                                if (player.Row < maxRow - 1)
                                {
                                    player.Row += 1;
                                }
                                else
                                {
                                    player.Row = 0;
                                }
                                break;
                            case direction.Left:
                                if (player.Column > 0)
                                {
                                    player.Column -= 1;
                                }
                                else
                                {
                                    player.Column = maxCol - 1;
                                }
                                break;
                            case direction.Right:
                                if (player.Column < maxCol - 1)
                                {
                                    player.Column += 1;
                                }
                                else
                                {
                                    player.Column = 0;
                                }
                                break;
                            default:
                                break;
                        }//end switch
                        player.Health -= 10;
                    }//end if
                }//end if
            }//end for
        }
        /// <summary>
        /// In order to center text we consider the length of the name
        /// each amount will hold a different position
        /// text is moved depending on the length of the string
        /// </summary>
        private void CheckTextPosition()
        {
            if (playerName.Length == 1)
            {
                textPos.X = 350;
            }
            if (playerName.Length == 2)
            {
                textPos.X = 343;
            }
            if (playerName.Length == 3)
            {
                textPos.X = 336;
            }
            if (playerName.Length == 4)
            {
                textPos.X = 329;
            }
            if (playerName.Length == 5)
            {
                textPos.X = 322;
            }
            if (playerName.Length == 6)
            {
                textPos.X = 315;
            }
            if (playerName.Length == 7)
            {
                textPos.X = 308;
            }
            if (playerName.Length == 8)
            {
                textPos.X = 301;
            }
            if (playerName.Length == 9)
            {
                textPos.X = 294;
            }
            if (playerName.Length == 10)
            {
                textPos.X = 287;
            }
            if (playerName.Length == 11)
            {
                textPos.X = 280;
            }
            if (playerName.Length == 12)
            {
                textPos.X = 273;
            }
            if (playerName.Length == 13)
            {
                textPos.X = 266;
            }
            if (playerName.Length == 14)
            {
                textPos.X = 259;
            }
            if (playerName.Length == 15)
            {
                textPos.X = 252;
            }
            if (playerName.Length == 16)
            {
                textPos.X = 245;
            }
            if (playerName.Length == 17)
            {
                textPos.X = 238;
            }
        }
        /// <summary>
        /// Respawn enemies when both dead if there is a block in the default place, spawn elsewhere
        /// </summary>
        private void RespawnEnemies()
        {
            wave++;
            if (worldMap[0, 13].ContainsSquare == 0)
            {
                enemies[0].Respawn(0, 13);
            }
            else if (worldMap[0, 12].ContainsSquare == 0)
            {
                enemies[0].Respawn(0, 12);
            }
            else if (worldMap[0, 11].ContainsSquare == 0)
            {
                enemies[0].Respawn(0, 11);
            }
            if (worldMap[0, 13].ContainsSquare == 0)
            {
                enemies[1].Respawn(13, 0);
            }
            else if (worldMap[0, 12].ContainsSquare == 0)
            {
                enemies[1].Respawn(12, 0);
            }
            else if (worldMap[0, 11].ContainsSquare == 0)
            {
                enemies[1].Respawn(11, 0);
            }
        }
        /// <summary>
        /// If player out of lives game over screen, if the player beats 2 waves win
        /// </summary>
        private void GameOverCheck()
        {
            if(player.Health == 0)
            {
                Initialize();
                currentState = GameState.GameOver;
            }
            if (wave == 3)
            {
                currentState = GameState.Victory;
            }
        }
        /// <summary>
        /// writes all the current details about the game into an XML file
        /// </summary>
        private void SaveToXMLFile()
        {
            try
            {
                //save up a seperate player and evil squares array files
                XmlTextWriter objXmlTextWriter = new XmlTextWriter("awesomeSquareSave.xml", null);

                objXmlTextWriter.Formatting = Formatting.Indented;
                objXmlTextWriter.WriteStartDocument();

                objXmlTextWriter.WriteComment("Awesome Square save file");
                objXmlTextWriter.WriteStartElement("awesomeSquareData");   //opens root node

                player.WriteToXMLFile(objXmlTextWriter);

                objXmlTextWriter.WriteEndElement(); //close root node
                objXmlTextWriter.WriteEndDocument();
                objXmlTextWriter.Flush();
                objXmlTextWriter.Close();   //end saving player

                objXmlTextWriter = new XmlTextWriter("evilSquares.xml", null);  //the evil squares array save file

                objXmlTextWriter.Formatting = Formatting.Indented;
                objXmlTextWriter.WriteStartDocument();

                objXmlTextWriter.WriteComment("Evil squares save file");
                objXmlTextWriter.WriteStartElement("evilSquaresData");   //opens root node

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].WriteToXMLFile(objXmlTextWriter);
                }
                objXmlTextWriter.WriteEndElement(); //close root node
                objXmlTextWriter.WriteEndDocument();
                objXmlTextWriter.Flush();
                objXmlTextWriter.Close();
            }
            catch (Exception exception)
            {
                Exit(); //if something went wrong exit the game
            }
        }
        private void LoadFromXMLFile()
        {
            string elementName = "";
            try
            {
                XmlTextReader textReader = new XmlTextReader("awesomeSquareSave.xml");
                while (textReader.Read())
                {
                    XmlNodeType nType = textReader.NodeType;

                    if (nType == XmlNodeType.Element)
                    {
                        elementName = textReader.Name.ToString(); //gets the name of the node
                    }
                    if (elementName == "row")
                    {
                        string aString = textReader.ReadString();
                        player.Row = Convert.ToInt32(aString);
                    }
                    if (elementName == "column")
                    {
                        string aString = textReader.ReadString();
                        player.Column = Convert.ToInt32(aString);
                    }
                    if (elementName == "name")
                    {
                        string aString = textReader.ReadString();
                        player.Name = aString;
                    }
                    if (elementName == "health")
                    {
                        string aString = textReader.ReadString();
                        player.Health = Convert.ToInt32(aString);
                    }
                    if (elementName == "score")
                    {
                        string aString = textReader.ReadString();
                        player.Score = Convert.ToInt32(aString);
                    }
                    if (elementName == "heading")
                    {
                        string aString = textReader.ReadString();
                        player.Heading = (direction)Enum.Parse(typeof(direction), aString);
                    }
                    elementName = "";   //resetting the name of the current element
                }//end while
            }//end try
            catch(Exception exception)
            {
                Exit();
            }
        }
        /// <summary>
        /// if row and column of two enemies are the same move the first enemy back one space
        /// </summary>
        /// <param name="enemy1"></param>
        /// <param name="enemy2"></param>
        private void CheckIfEnemiesCollide(EvilSquare enemy1, EvilSquare enemy2)
        {
            if (enemy1.Row == enemy2.Row && enemy1.Col == enemy2.Col)
            {
                switch (enemy1.Heading) //depending on direction go back one cell and get a new heading account for boundaries
                {
                    case direction.Up:
                        if (enemy1.Row != 13)   //if not at the bottom we can add a row since within bounds and get a new heading for enemy1
                        {
                            enemy1.Row += 1;
                            enemy1.NewHeading();
                        }
                        else
                        {
                            enemy2.Row -= 1;    //in a circumstance where enemy 1 is at the bottom we move the enemy 2 up one cell instead
                        }
                        break;
                    case direction.Down:
                        if (enemy1.Row != 0)
                        {
                            enemy1.Row -= 1;
                            enemy1.NewHeading();
                        }
                        else
                        {
                            enemy2.Row += 1;
                        }
                        break;
                    case direction.Left:
                        if (enemy1.Col != 13)
                        {
                            enemy1.Col += 1;
                            enemy1.NewHeading();
                        }
                        else
                        {
                            enemy2.Col -= 1;
                        }
                        break;
                    case direction.Right:
                        if (enemy1.Col != 0)
                        {
                            enemy1.Col -= 1;
                            enemy1.NewHeading();
                        }
                        else
                        {
                            enemy2.Col += 1;
                        }
                        break;
                    default:
                        break;
                }
            }
        }//end check if enemies collide
    }//end game1
}//end namespace
