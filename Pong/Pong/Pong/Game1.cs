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

namespace Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Variable To Store Whether The Game Is Over Or Not
        Boolean gameover = false;


        //Screen Size
        int displaywidth = 800;
        int displayheight = 540;


        //Variable To generate Random Numbers
        Random randomiser = new Random();

        //Structure For Moving 2D Graphics
        struct graphics2d
        {

            public Texture2D image;         //Holds 2d Graphic
            public Vector3 position;        //On Screen Position
            public Vector3 oldposition;     //Position Of Graphic Before Collision
            public Rectangle rect;          //Holds Position And Dimensiong Of Graphic
            public Vector2 origin;          //Point On The Image Where The Positing Points To
            public float size;              //Size Ratio, For Scaling Up Or Down The Image Size When Drawn
            public float rotation;          //Amount Or Rotation To Apply
            public Vector3 velocity;        //Direction And Speed Of Object
            public BoundingSphere bsphere;  //Bounding Sphere For Object
            public BoundingBox bbox;        //Bounding Box For Object
            public float power;             //Power Of An Object With Regards To Acceleration & Speed
            public float rotationspeed;     //Speed At Which The Object Can Rotate, Turn Or Spin
            public int score;               //Score

        }

        graphics2d ball;                            //Ball Graphic

        graphics2d[] bat = new graphics2d[2];       //Array Of Bats

        graphics2d background;                      //Background Graphic

        SpriteFont mainfont;                        //Font For Drawing Text On The Screen

        const int maxscore = 25;                        // Sets A Max Score

        SoundEffect lightsabre, yoda, darth,winner;           //In-Game Sounds

        SoundEffect soundtrack;
        SoundEffectInstance music;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = displaywidth;
            this.graphics.PreferredBackBufferHeight = displayheight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        void resetgame()
        {
            gameover = false;                                                       //Set Game Over False For The Start Of A New Game

            //Set Initial Positions Of The 2 Bats
            bat[0].position = new Vector3(50, displayheight / 2, 0);
            bat[1].position = new Vector3(displaywidth - 50, displayheight / 2, 0);

            //Resets Scores
            bat[0].score = 0;
            bat[1].score = 0;

            //Resets The Ball
            resetball();
        }
        
        
        //Set Initial Position Of Ball To The Middle Of The Screen
        void resetball()
        {
            ball.position = new Vector3(displaywidth / 2, displayheight / 2, 0);


            //Generate Random Velocities For The Ball
            do
            {
                ball.velocity.X = (randomiser.Next(3) - 1) * (5 + randomiser.Next(3));
                ball.velocity.Y = randomiser.Next(5) - 2;
            } while (ball.velocity.X == 0 || ball.velocity.Y == 0);

        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background.image = Content.Load<Texture2D>("pitch");                                        //Load Bullet Image
            background.rect.X = 0;
            background.rect.Y = 0;
            background.rect.Width = displaywidth;
            background.rect.Height = displayheight;

            ball.image = Content.Load<Texture2D>("football");                                 //Load Ball Image
            ball.size = 0.15f;                                                                          //Set Size Ratio
            ball.origin.X = ball.image.Width / 2;                                                       //Set Origin To Centre Of Image
            ball.origin.Y = ball.image.Height/ 2;                                                       //Same As Above
            ball.rect.Width = (int)(ball.image.Width * ball.size);                                      //Set Size Of Rectangle Based On Size Ratio
            ball.rect.Height = (int)(ball.image.Height*ball.size);                                      //Same As Above
            ball.rotationspeed = 0.1f;                                                                  //How Fast The Ball Can Spin

            
            mainfont = Content.Load<SpriteFont>("quartz4");                                             //Load The quartz Font
    
            resetball();

            resetgame();


            lightsabre = Content.Load<SoundEffect>("applause-6");
            yoda = Content.Load<SoundEffect>("strongam");
            darth = Content.Load<SoundEffect>("darkside");
            winner = Content.Load<SoundEffect>("explosion");
            soundtrack = Content.Load<SoundEffect>("MainMenu");
            music = soundtrack.CreateInstance();
            music.IsLooped = true;

         //   if (music.State == SoundState.Stopped) music.Play();
          //  if (music.State == SoundState.Playing) music.Stop();
          //  if (music.State == SoundState.Playing) music.Pause();
         //   if (music.State == SoundState.Paused) music.Resume();
  

            bat[0].image = Content.Load<Texture2D>("goalie");                                          //Load Bat Graphic For Player 1
            bat[1].image = Content.Load<Texture2D>("goalie2");                                       //Load Bat Graphic For Player 2

           
            
            
            
           for (int count = 0; count < bat.Count(); count++)
            {
                bat[count].size = 0.2f;                                                                 //Set The Size Ratio
                bat[count].origin.X = bat[count].image.Width / 2;                                       //Set The Origin To The Centre
                bat[count].origin.Y = bat[count].image.Height / 2;                                      //As Above
                bat[count].rect.Width = (int)(bat[count].image.Width / 2 * bat[count].size);                //Set Size Of Rectangle Based On Size Ratio         
                bat[count].rect.Height = (int)(bat[count].image.Height  /2 * bat[count].size);                //As Above
                bat[count].rotation = 0;                                                                //Set Initial Rotation
                bat[count].power = 1f;                                                                //Set Power Of Bat With Regards To Speed
            }

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // TODO: Add your update logic here
            //Reads The Joypads
            GamePadState[] pad = new GamePadState[2];
            pad[0] = GamePad.GetState(PlayerIndex.One);                         //Reads GamePad 1
            pad[1] = GamePad.GetState(PlayerIndex.Two);                         //Reads GamePad 2

            const float friction = 0.96f;

            if(gameover)
                music.Play();

            if (!gameover)
            {

                

                
               

                if (pad[0].Buttons.Back == ButtonState.Pressed)
                    gameover = true;

                //Move Two Bats
                for (int i = 0; i < bat.Count(); i++)
                {
                    if (!pad[i].IsConnected)
                        if (bat[i].position.Y < ball.position.Y)
                            bat[i].velocity.Y += bat[i].power;
                        else
                            bat[i].velocity.Y -= bat[i].power;

                    //Move The Bats Based On Joypad Input
                  //  bat[i].position.Y -= pad[i].ThumbSticks.Left.Y * bat[i].power;
                    bat[i].velocity.Y -= pad[i].ThumbSticks.Left.Y * bat[i].power;
                    bat[i].position += bat[i].velocity;

                    bat[i].velocity.Y *= friction;

                    //Alter Bat Direction When Bat Hits Bounderies
                    //Bat Hits The Bottom Of The Screen
                    if (bat[i].position.Y > displayheight - bat[i].rect.Height / 2)
                    {
                        bat[i].position.Y = displayheight - bat[i].rect.Height / 2;
                        bat[i].velocity.Y = 0;
                    }
                    //Bat Hits The Top Of The Screen
                    if (bat[i].position.Y < bat[i].rect.Height / 2)
                    {
                        bat[i].position.Y = bat[i].rect.Height / 2;
                        bat[i].velocity.Y = 0;
                    }

                    //Set The Bat Rectangle To The Current Position
                    bat[i].rect.X = (int)bat[i].position.X;
                    bat[i].rect.Y = (int)bat[i].position.Y;

                    //Create Bounding Box
                    bat[i].bbox = new BoundingBox(new Vector3(bat[i].position.X - bat[i].rect.Width / 2, bat[i].position.Y - bat[i].rect.Height / 2, 0),
                                  new Vector3(bat[i].position.X + bat[i].rect.Width / 2, bat[i].position.Y + bat[i].rect.Height / 2, 0));
                }


            ball.position += ball.velocity;                 //Move The Ball
            ball.rotation += ball.rotationspeed;            //Spins Ball
            ball.velocity *= 1.0005f;                     //Speeds Ball Up Over Time


            //If The Ball Hits The Top Or The Bottom Of The Screen Reverse Its Direction On The Y Axis
            if (ball.position.Y < ball.rect.Height / 2)
            {
                ball.velocity.Y = -ball.velocity.Y;
                ball.position.Y = ball.rect.Height / 2;
            }
            if (ball.position.Y > displayheight - ball.rect.Height / 2)
            {
                ball.velocity.Y = -ball.velocity.Y;
                ball.position.Y = displayheight - ball.rect.Height / 2;
            }

            //Check If The Ball Goes Out Either Side
            if (ball.position.X > displaywidth + ball.rect.Width)
            {
                bat[0].score += 5;
                resetball();
                yoda.Play();
            }
            if (ball.position.X < -ball.rect.Width)
            {
                bat[1].score += 5;
                resetball();
                darth.Play();
            }

            float ballspeed = ball.velocity.Length();
            ball.velocity.Normalize();

            Boolean hitball = false;

                //Create BoundingSphere Around The Ball
            ball.bsphere = new BoundingSphere(ball.position, ball.rect.Width / 2);

            // Bounce Ball Off Bat 0
            if (bat[0].bbox.Intersects(ball.bsphere) && bat[0].position.X <= ball.position.X)
            {
                hitball = true;
                ball.velocity.X = Math.Abs(ball.velocity.X);   //Force Ball To Go Right
                ball.velocity.Y = (ball.position.Y - bat[0].position.Y) / 40f;
                lightsabre.Play();
            }
                
                // Bounce Ball Off Bat 1
            if (bat[1].bbox.Intersects(ball.bsphere) && bat[1].position.X >= ball.position.X)
            {
                ball.velocity.X = -Math.Abs(ball.velocity.X);                           //Force Ball To Go Left
                lightsabre.Play();
            }

            //Set Position Of Ball Rectangle To Match Its Position For Drawing Purposes
            ball.rect.X = (int)ball.position.X;
            ball.rect.Y = (int)ball.position.Y;

                //If Either Player Reaches Maxscore, End Game
            if (bat[0].score >= maxscore || bat[1].score >= maxscore)
            {
                winner.Play();
                gameover = true;
               
                //winner.Play();
            }
                    }
                else
                    {
                   //Game Is Over
                //Start Game Again When User Presses Start
                        
                    if (pad[0].Buttons.Start == ButtonState.Pressed || pad[1].Buttons.Start == ButtonState.Pressed)
                        resetgame();
                    if (pad[0].Buttons.Back == ButtonState.Pressed)
                        this.Exit();
                }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(background.image, background.rect, Color.White);
            spriteBatch.Draw(ball.image, ball.rect, null, Color.White, ball.rotation, ball.origin, SpriteEffects.None, 0);


            for (int i = 0; i < bat.Count(); i++)
            {
                spriteBatch.Draw(bat[i].image, bat[i].rect, null, Color.White, bat[i].rotation, bat[i].origin, SpriteEffects.None, 0);
            }



           
            spriteBatch.DrawString(mainfont, "P1 Score " + bat[0].score.ToString(), new Vector2(10, 10), Color.Yellow);
            spriteBatch.DrawString(mainfont, " P2 Score " + bat[1].score.ToString(), new Vector2(displaywidth - 220, 10), Color.Yellow);
            

            if (bat[0].score >= maxscore)
            {
                
                spriteBatch.DrawString(mainfont, "P1 WINS", new Vector2(displaywidth / 2 - 90, displayheight / 2 - 50), Color.Red);
            }

            if (bat[1].score >= maxscore)
            {
               
                spriteBatch.DrawString(mainfont, "P2 WINS", new Vector2(displaywidth / 2 - 90, displayheight / 2 - 50), Color.Yellow);
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
