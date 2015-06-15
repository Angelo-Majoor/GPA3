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

            // Load the effect
            Effect effect = this.Content.Load<Effect>("Effects/Effect1");

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
            Effect effect = mesh.Effects[0];

            // Set the effect parameters
            effect.CurrentTechnique = effect.Techniques["Phong"];

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
            effect.Parameters["PointLight"].SetValue(new Vector3(200, 50, 0));
            // Set the value of the World matrix to use in the effect file
            effect.Parameters["World"].SetValue(Matrix.CreateScale(1.0f));
            // Calculate the inverse of the World matrix to use in the effect file
            effect.Parameters["WorldInverse"].SetValue(Matrix.Invert(Matrix.CreateScale(10.0f)));

            Vector3[] lights = new Vector3[5];
            lights[0] = new Vector3(200, 50, 0);
            lights[1] = new Vector3(0, 50, 200);
            lights[2] = new Vector3(-200, 50, 0);
            lights[3] = new Vector3(0, 50, -200);
            lights[4] = new Vector3(0, 400, 0);

            effect.Parameters["LightPositions"].SetValue(lights);

            // Draw the model
            mesh.Draw();

            base.Draw(gameTime);
        }
    }
}
