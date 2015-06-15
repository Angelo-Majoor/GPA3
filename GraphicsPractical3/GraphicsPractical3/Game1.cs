// Angelo Majoor - 3843726

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

namespace GraphicsPractical3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private FrameRateCounter frameRateCounter;

        // Game objects and variables
        private Camera camera;

        // Model
        private Model model;
        private Material modelMaterial;

        // Quad
        private VertexPositionNormalTexture[] quadVertices;
        private short[] quadIndices;
        private Matrix quadTransform;

        private Effect effect;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            // Create and add a frame rate counter
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        protected override void Initialize()
        {
            // Copy over the device's rasterizer state to change the current fillMode
            this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            this.graphics.ApplyChanges();
            // Initialize the camera
            this.camera = new Camera(new Vector3(0, 30, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            this.IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a SpriteBatch object
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.spriteFont = Content.Load<SpriteFont>("SpriteFont1");

            // Load the effect
            this.effect = this.Content.Load<Effect>("Effects/Effect1");

            // Load the teapot model
            //this.model = this.Content.Load<Model>("Models/Teapot");
            //this.model = this.Content.Load<Model>("Models/Bunny");
            this.model = this.Content.Load<Model>("Models/Femalehead");

            // Let the model use the effect
            this.model.Meshes[0].MeshParts[0].Effect = effect;
            // Setup the quad
            this.setupQuad();
        }

        /// <summary>
        /// Sets up a 2 by 2 quad around the origin.
        /// </summary>
        private void setupQuad()
        {
            float scale = 50.0f;

            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            this.quadVertices = new VertexPositionNormalTexture[4];
            // Top left
            this.quadVertices[0].Position = new Vector3(-1, 0, -1);
            this.quadVertices[0].Normal = quadNormal;
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0, -1);
            this.quadVertices[1].Normal = quadNormal;
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, 0, 1);
            this.quadVertices[2].Normal = quadNormal;
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, 0, 1);
            this.quadVertices[3].Normal = quadNormal;

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.quadTransform = Matrix.CreateScale(scale);
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Rotate the camera around the object using the left and right buttons
            float deltaAngle = 0;
            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Left))
            {
                deltaAngle += -3 * timeStep;
            }
            if (kbState.IsKeyDown(Keys.Right))
            {
                deltaAngle += 3 * timeStep;
            }
            if (deltaAngle != 0)
            {
                this.camera.Eye = Vector3.Transform(this.camera.Eye, Matrix.CreateRotationY(deltaAngle));
            }

            // Update the window title
            this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen in a predetermined color and clear the depth buffer
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

            // Get the model's only mesh
            ModelMesh mesh = this.model.Meshes[0];
            this.effect = mesh.Effects[0];

            // Set the effect parameters
            this.effect.CurrentTechnique = effect.Techniques["MultipleLightSources"];

            // Matrices for 3D perspective projection
            this.camera.SetEffectParameters(effect);

            // Set the diffuse color
            this.modelMaterial.DiffuseColor = Color.Red;
            // Set the ambient color
            this.modelMaterial.AmbientColor = Color.Red;
            // Set the ambient intensity
            this.modelMaterial.AmbientIntensity = 0.2f;
            // Set the specular color
            //this.modelMaterial.SpecularColor = Color.White;
            // Set the specular intensity
            //this.modelMaterial.SpecularIntensity = 2.0f;
            // Set the specular power
            //this.modelMaterial.SpecularPower = 25.0f;
            // Apply the elements of the SetEffectParameters method that are being used
            this.modelMaterial.SetEffectParameters(effect);

            // Set the value of the point light to use in the effect file
            this.effect.Parameters["PointLight"].SetValue(new Vector3(200, 50, 0));
            // Set the value of the World matrix to use in the effect file
            this.effect.Parameters["World"].SetValue(Matrix.Identity);
            // Calculate the inverse of the World matrix to use in the effect file
            this.effect.Parameters["WorldInverse"].SetValue(Matrix.Invert(Matrix.CreateScale(10.0f)));

            int numberOfLights = 5;

            Light light1 = new Light(new Vector3(150, 150, 0), Color.Red.ToVector4());
            Light light2 = new Light(new Vector3(0, 150, -250), Color.Blue.ToVector4());
            Light light3 = new Light(new Vector3(-150, 150, 0), Color.Yellow.ToVector4());
            Light light4 = new Light(new Vector3(0, 150, 250), Color.HotPink.ToVector4());
            Light light5 = new Light(new Vector3(0, 400, 0), Color.Green.ToVector4());

            Vector3[] lightPositions = new Vector3[numberOfLights];
            lightPositions[0] = light1.lightPosition;
            lightPositions[1] = light2.lightPosition;
            lightPositions[2] = light3.lightPosition;
            lightPositions[3] = light4.lightPosition;
            lightPositions[4] = light5.lightPosition;

            Vector4[] diffuseColors = new Vector4[numberOfLights];
            diffuseColors[0] = light1.lightColor;
            diffuseColors[1] = light2.lightColor;
            diffuseColors[2] = light3.lightColor;
            diffuseColors[3] = light4.lightColor;
            diffuseColors[4] = light5.lightColor;

            this.effect.Parameters["LightPositions"].SetValue(lightPositions);
            this.effect.Parameters["DiffuseColor"].SetValue(diffuseColors);

            // Draw the model
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
