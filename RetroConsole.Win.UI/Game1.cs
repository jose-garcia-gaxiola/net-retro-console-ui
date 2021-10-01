using Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Oldc.UI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D sceneTexture;
        Vector2 scenePosition;
        private static int fps = 0;
        int currFps = 0;
        private static byte step = 0x0;
        static Color[] data;
        Bus bus;
        private SpriteFont font;
        private Debug6502 debug6502;
        private Texture2D texture;
        private Timer timer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            timer = new Timer((o) =>
            {
                currFps = fps;
                fps = 0;
            }, null, 0, 1000);

            bus = new Bus();
            bus.Ppu.OnFrameCompleted += PpuOnFrameCompleted;
            //debug6502 = new Debug6502(bus);
        }

        private async void PpuOnFrameCompleted(uint[] frame)
        {
            if (sceneTexture != null)
            {
                sceneTexture.SetData(frame, 0, frame.Length);
                fps++;
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            scenePosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                        _graphics.PreferredBackBufferHeight / 2);

            //load test cart

            FileInfo fi;

            fi = new FileInfo("C:\\testprj\\nestest.nes");

            if (fi.Exists)
            {
                using (var f = fi.OpenRead())
                {
                    byte[] stream = new byte[f.Length];
                    f.Read(stream, 0, Convert.ToInt32(f.Length));

                    bus.InsertCartridge(new Cartridge(stream));
                }

                bus.Reset();
                bus.Run();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sceneTexture = CreateTexture(GraphicsDevice, bus.Ppu.ScreenWidth, bus.Ppu.ScreenHeight, pixel => { return pixel % 2 == 0 ? Color.White : Color.BlueViolet; });
            //sceneTexture.SetData(new uint[] { uint.MaxValue, uint.MaxValue }, 0, 2);

            // TODO: use this.Content to load your game content here

            font = Content.Load<SpriteFont>("File");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //bus.Cpu.Clock();
            //debug6502?.PrintState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            _spriteBatch.Draw(sceneTexture,
                                scenePosition,
                                null,
                                Color.White,
                                0f,
                                new Vector2(sceneTexture.Width / 2, sceneTexture.Height / 2),
                                Vector2.One,
                                SpriteEffects.None,
                                0f
                            );

            _spriteBatch.DrawString(font, $"{currFps}", new Vector2(0, 0), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            if (texture is null)
            {
                texture = new Texture2D(device, width, height);
            }

            if (data is null)
            {
                //the array holds the color for each pixel in the texture
                data = new Color[width * height];
            }

            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }
    }
}
