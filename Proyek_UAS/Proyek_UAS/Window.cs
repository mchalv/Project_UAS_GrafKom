using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyek_UAS
{
    internal class Window : GameWindow
    {
        Camera _camera;

        List<Assets3D> Michael = new List<Assets3D>();
        List<Assets3D> Audrey = new List<Assets3D>();
        List<Assets3D> Gregorius = new List<Assets3D>();

        List<Assets3D> Ground = new List<Assets3D>();
        List<Assets3D> Wall = new List<Assets3D>();

        Assets3D[] LightObjects = new Assets3D[4];
        Assets3D LightObject = new Assets3D(new Vector3(1, 1, 1));

        bool _firstMove = true;
        Vector2 _lastPos;

        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(-14.5f, 3f, 14.5f),
            new Vector3(14.5f, 3f, 14.5f),
            new Vector3(-14.5f, 3f, -14.5f),
            new Vector3(14.5f, 3f, -8.5f)
        };

        static class Constants
        {
            public const string path = "../../../Shaders/";
        }

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.82f, 0.82f, 0.82f, 0.8f);
            GL.Enable(EnableCap.DepthTest);

            // PENCAHAYAAN //
            //LightObject.createSphere(14.5f, 3f, -8.5f, 0.5f, 0.5f, 0.5f, 1000, 1000);
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    LightObjects[i] = new Assets3D(new Vector3(1, 0, 0));
                }
                else if (i == 1)
                {
                    LightObjects[i] = new Assets3D(new Vector3(1, 1, 0));
                }
                else if (i == 2)
                {
                    LightObjects[i] = new Assets3D(new Vector3(0, 1, 0));
                }
                else if (i == 3)
                {
                    LightObjects[i] = new Assets3D(new Vector3(0, 0, 1));
                }
                LightObjects[i].createSphere(_pointLightPositions[i].X, _pointLightPositions[i].Y, _pointLightPositions[i].Z, 0.5f, 0.5f, 0.5f, 1000, 1000);
                LightObjects[i].load(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            }

            // MAP LABIRIN //

            Ground.Add(new Assets3D(new Vector3(1f, 0.698f, 0.5f)));
            for (int i = 1; i < 4; i++)
            {
                Ground.Add(new Assets3D(new Vector3(0.58f, 0.84f, 0.98f)));
            }
            Ground[0].createCube(0f, -0.4f, 0f, 50f, 2f, 40f);
            Ground[1].createCube(Ground[0].getPos().X + Ground[0].getlenx() / 2.07f, Ground[0].getPos().Y + Ground[0].getleny() * 7f, Ground[0].getPos().Z, Ground[0].getleny(), Ground[0].getlenx() / 1.7f, Ground[0].getlenz());
            Ground[2].createCube(Ground[0].getPos().X - Ground[0].getlenx() / 2.07f, Ground[0].getPos().Y + Ground[0].getleny() * 7f, Ground[0].getPos().Z, Ground[0].getleny(), Ground[0].getlenx() / 1.7f, Ground[0].getlenz());
            Ground[3].createCube(Ground[0].getPos().X, Ground[0].getPos().Y + Ground[0].getleny() * 7f, Ground[0].getPos().Z - Ground[0].getlenz() / 2.1f, Ground[0].getlenx(), Ground[0].getlenx() / 1.7f, Ground[0].getleny());

            Wall.Add(new Assets3D(new Vector3(1f, 0.698f, 0.5f)));
            for (int i = 1; i < 42; i++)
            {
                Wall.Add(new Assets3D(new Vector3(0.76f, 0.698f, 0.5f)));
            }
            setUpWall();

            // PLANKTON //
            var badan_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var mata_plankton = new Assets3D(new Vector3(0.89f, 0.87f, 0.6f));
            var kornea_plankton = new Assets3D(new Vector3(0.69f, 0.28f, 0.28f));
            var kaki1_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var kaki2_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var tangan1_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var tangan2_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var antena1_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var antena2_plankton = new Assets3D(new Vector3(0.25f, 0.51f, 0.42f));
            var alis_plankton = new Assets3D(new Vector3(0f, 0f, 0f));
            var mulut_plankton = new Assets3D(new Vector3(0f, 0f, 0f));
            badan_plankton.createCube(1.5f, 0.7f, 15f, 0.05f, 0.1f, 0.05f);
            mata_plankton.createCube(badan_plankton.getPos().X, badan_plankton.getPos().Y + badan_plankton.getleny() / 4f, badan_plankton.getPos().Z + badan_plankton.getlenz() / 2f, badan_plankton.getlenx() / 2.5f, badan_plankton.getleny() / 4f, badan_plankton.getlenz() / 2f);
            kornea_plankton.createCube(mata_plankton.getPos().X, mata_plankton.getPos().Y, mata_plankton.getPos().Z + mata_plankton.getlenz() / 2f, mata_plankton.getlenx() / 3f, mata_plankton.getleny() / 3f, mata_plankton.getlenz() / 3f);
            kaki1_plankton.createCube(badan_plankton.getPos().X - badan_plankton.getlenx() / 2, badan_plankton.getPos().Y - badan_plankton.getleny() / 1.5f, badan_plankton.getPos().Z, badan_plankton.getlenx() / 5, badan_plankton.getleny() / 2f, badan_plankton.getlenz() / 4f);
            kaki2_plankton.createCube(badan_plankton.getPos().X + badan_plankton.getlenx() / 2, badan_plankton.getPos().Y - badan_plankton.getleny() / 1.5f, badan_plankton.getPos().Z, badan_plankton.getlenx() / 5, badan_plankton.getleny() / 2f, badan_plankton.getlenz() / 4f);
            tangan1_plankton.createCube(badan_plankton.getPos().X - badan_plankton.getlenx() / 1.75f, badan_plankton.getPos().Y, badan_plankton.getPos().Z + badan_plankton.getlenz() / 2f, badan_plankton.getlenx() / 5, badan_plankton.getleny() / 5f, badan_plankton.getlenz());
            tangan2_plankton.createCube(badan_plankton.getPos().X + badan_plankton.getlenx() / 1.75f, badan_plankton.getPos().Y, badan_plankton.getPos().Z + badan_plankton.getlenz() / 2f, badan_plankton.getlenx() / 5, badan_plankton.getleny() / 5f, badan_plankton.getlenz());           
            antena1_plankton.createCube(badan_plankton.getPos().X - badan_plankton.getlenx() / 2.5f, badan_plankton.getPos().Y + badan_plankton.getleny() / 2, badan_plankton.getPos().Z, badan_plankton.getlenx() / 10, badan_plankton.getleny() * 0.75f, badan_plankton.getlenz() / 10);            
            antena2_plankton.createCube(badan_plankton.getPos().X + badan_plankton.getlenx() / 2.5f, badan_plankton.getPos().Y + badan_plankton.getleny() / 2, badan_plankton.getPos().Z, badan_plankton.getlenx() / 10, badan_plankton.getleny() * 0.75f, badan_plankton.getlenz() / 10);

            Michael.Add(badan_plankton);
            Michael.Add(mata_plankton);
            Michael.Add(kornea_plankton);
            Michael.Add(kaki1_plankton);
            Michael.Add(kaki2_plankton);
            Michael.Add(tangan1_plankton);
            Michael.Add(tangan2_plankton);
            Michael.Add(antena1_plankton);
            Michael.Add(antena2_plankton);

            // RESEP //
            var botol_kaca = new Assets3D(new Vector3(0.996f, 0.9254f, 0.7843f));
            var tutup_botol = new Assets3D(new Vector3(0f, 0f, 0f));

            botol_kaca.createCube(1.5f, 0.75f, 15.05f, 0.05f, 0.1f, 0.05f);
            tutup_botol.createCube(botol_kaca.getPos().X, botol_kaca.getPos().Y + botol_kaca.getleny() / 2f, botol_kaca.getPos().Z, botol_kaca.getlenx() / 1.25f, botol_kaca.getleny() / 3f, botol_kaca.getlenz() / 1.25f);

            Audrey.Add(botol_kaca);
            Audrey.Add(tutup_botol);

            // SPONGEBOB //
            var balok1 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var balok2 = new Assets3D(new Vector3(1f, 1f, 1f));
            var balok3 = new Assets3D(new Vector3(0.7f, 0.4f, 0.2f));
            var lingkaran1 = new Assets3D(new Vector3(1, 1, 1));
            var lingkaran2 = new Assets3D(new Vector3(1, 1, 1));
            var tabung1 = new Assets3D(new Vector3(1, 1, 1));
            var tabung2 = new Assets3D(new Vector3(1, 1, 1));
            var tabung3 = new Assets3D(new Vector3(0.7f, 0.4f, 0.2f));
            var tabung4 = new Assets3D(new Vector3(0.7f, 0.4f, 0.2f));
            var tabung5 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var tabung6 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var tabung7 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var tabung8 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var tabung9 = new Assets3D(new Vector3(1f, 1f, 1f));
            var tabung10 = new Assets3D(new Vector3(1f, 1f, 1f));
            var balok4 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var balok5 = new Assets3D(new Vector3(1.0f, 0.9f, 0.4f));
            var balok6 = new Assets3D(new Vector3(0, 0, 0));
            var balok7 = new Assets3D(new Vector3(0, 0, 0));
            var oval9 = new Assets3D(new Vector3(1.0f, 0.93f, 0.4f));

            balok1.createCube(1.5f, 1.2f, 13f, 0.5f, 0.5f, 0.3f);
            balok2.createCube(balok1.getPos().X, balok1.getPos().Y - balok1.getleny() / 2, balok1.getPos().Z, balok1.getlenx(), balok1.getleny() / 8, balok1.getlenz());
            balok3.createCube(balok2.getPos().X, balok2.getPos().Y - balok2.getleny() / 2, balok2.getPos().Z, balok2.getlenx(), balok2.getleny(), balok2.getlenz());
            lingkaran1.createCube(balok1.getPos().X + balok1.getlenx() / 6, balok1.getPos().Y + balok1.getleny() / 5, balok1.getPos().Z + balok1.getlenz() / 2, balok1.getlenx() / 6, balok1.getleny() / 6, balok1.getlenz() / 7);
            lingkaran2.createCube(balok1.getPos().X - balok1.getlenx() / 6, balok1.getPos().Y + balok1.getleny() / 5, balok1.getPos().Z + balok1.getlenz() / 2, balok1.getlenx() / 6, balok1.getleny() / 6, balok1.getlenz() / 7);
            tabung1.createCube(balok1.getPos().X - balok1.getlenx() / 1.85f, balok1.getPos().Y - balok1.getleny() / 3, balok1.getPos().Z, balok1.getlenx() / 10f, balok1.getleny() / 6f, balok1.getlenz() / 2f); //ini
            tabung2.createCube(balok1.getPos().X + balok1.getlenx() / 1.85f, balok1.getPos().Y - balok1.getleny() / 3, balok1.getPos().Z, balok1.getlenx() / 10f, balok1.getleny() / 6f, balok1.getlenz() / 2f); //ini
            tabung3.createCube(balok3.getPos().X - balok3.getlenx() / 3.5f, balok3.getPos().Y - balok3.getleny() , balok3.getPos().Z, balok3.getlenx() / 8, balok3.getleny(), balok3.getlenz() / 4f); //ini
            tabung4.createCube(balok3.getPos().X + balok3.getlenx() / 3.5f, balok3.getPos().Y - balok3.getleny(), balok3.getPos().Z, balok3.getlenx() / 8, balok3.getleny(), balok3.getlenz() / 4f); //ini
            tabung5.createCube(tabung1.getPos().X - tabung1.getlenx() / 5f, tabung1.getPos().Y - tabung1.getleny() * 2f, tabung1.getPos().Z, tabung1.getlenx() / 2, tabung1.getleny() * 3, tabung1.getlenz() / 5f);
            tabung6.createCube(tabung2.getPos().X + tabung2.getlenx() / 5f, tabung2.getPos().Y - tabung2.getleny() * 2f, tabung2.getPos().Z, tabung2.getlenx() / 2, tabung2.getleny() * 3, tabung2.getlenz() / 5f);
            tabung7.createCube(tabung3.getPos().X - tabung3.getlenx() * 2f, tabung3.getPos().Y - tabung3.getleny() * 2, tabung3.getPos().Z, tabung3.getlenx() * 1.25f, tabung3.getleny() * 1.25f, tabung3.getlenz());
            tabung8.createCube(tabung4.getPos().X + tabung4.getlenx() * 2f, tabung4.getPos().Y - tabung4.getleny() * 2, tabung4.getPos().Z, tabung4.getlenx() * 1.25f, tabung4.getleny() * 1.25f, tabung4.getlenz());
            tabung9.createCube(tabung7.getPos().X + tabung7.getlenx() * 1.65f, tabung7.getPos().Y + tabung7.getleny() / 1.4f, tabung7.getPos().Z, tabung7.getlenx() / 1.5f, tabung7.getleny(), tabung7.getlenz());
            tabung10.createCube(tabung8.getPos().X - tabung8.getlenx() * 1.65f, tabung8.getPos().Y + tabung8.getleny() / 1.4f, tabung8.getPos().Z, tabung8.getlenx() / 1.5f, tabung8.getleny(), tabung8.getlenz());
            balok4.createCube(tabung5.getPos().X + tabung5.getlenx() * 5.5f, tabung5.getPos().Y - tabung5.getleny() / 4f, tabung5.getPos().Z, tabung5.getlenx(), tabung5.getleny(), tabung5.getlenz());
            balok5.createCube(tabung6.getPos().X - tabung6.getlenx() * 5.5f, tabung6.getPos().Y - tabung6.getleny() / 4f, tabung6.getPos().Z, tabung6.getlenx(), tabung6.getleny(), tabung6.getlenz());
            balok6.createCube(tabung9.getPos().X, tabung9.getPos().Y - tabung9.getleny() * 1.5f, tabung9.getPos().Z + tabung9.getlenz() / 2f, tabung9.getlenx() * 2, tabung9.getleny() / 1.5f, tabung9.getlenz() * 2);
            balok7.createCube(tabung10.getPos().X, tabung10.getPos().Y - tabung10.getleny() * 1.5f, tabung10.getPos().Z + tabung10.getlenz() / 2f, tabung10.getlenx() * 2, tabung10.getleny() / 1.5f, tabung10.getlenz() * 2);
            oval9.createCube(balok1.getPos().X, balok1.getPos().Y, balok1.getPos().Z + balok1.getlenz() / 2, balok1.getlenx() / 25, balok1.getleny() / 25, balok1.getlenz() / 2);
            
            Gregorius.Add(balok1);
            Gregorius.Add(balok2);
            Gregorius.Add(balok3);
            Gregorius.Add(lingkaran1);
            Gregorius.Add(lingkaran2);
            Gregorius.Add(tabung1);
            Gregorius.Add(tabung2);
            Gregorius.Add(tabung3);
            Gregorius.Add(tabung4);
            Gregorius.Add(tabung5);
            Gregorius.Add(tabung6);
            Gregorius.Add(tabung7);
            Gregorius.Add(tabung8);
            Gregorius.Add(tabung9);
            Gregorius.Add(tabung10);
            Gregorius.Add(balok4);
            Gregorius.Add(balok5);
            Gregorius.Add(balok6);
            Gregorius.Add(balok7);
            Gregorius.Add(oval9);

            foreach (Assets3D i in Michael)
            {
                i.load_withnormal(Constants.path + "shader_lighting.vert", Constants.path + "shader_lighting.frag", Size.X, Size.Y);
                i.scale(0.1f);
            }
            foreach (Assets3D i in Audrey)
            {
                i.load_withnormal(Constants.path + "shader_lighting.vert", Constants.path + "shader_lighting.frag", Size.X, Size.Y);
                i.scale(0.1f);
            }
            foreach (Assets3D i in Gregorius)
            {
                i.load_withnormal(Constants.path + "shader_lighting.vert", Constants.path + "shader_lighting.frag", Size.X, Size.Y);
                i.scale(0.1f);
            }
            foreach (Assets3D i in Wall)
            {
                i.load_withnormal(Constants.path + "shader_lighting.vert", Constants.path + "shader_lighting.frag", Size.X, Size.Y);
                i.scale(1f);
            }
            foreach (Assets3D i in Ground)
            {
                i.load_withnormal(Constants.path + "shader_lighting.vert", Constants.path + "shader_lighting.frag", Size.X, Size.Y);
            }

            _camera = new Camera(new Vector3(0f, 3f, 18f), Size.X / (float)Size.Y);
            CursorGrabbed = true;
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            float time = (float)args.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 temp = Matrix4.Identity;

            foreach (Assets3D i in Michael)
            {
                i.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                i.setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0f, 0f, 0f), _camera.Position);
                i.setDirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f), new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f));
                i.setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                i.setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            }

            foreach (Assets3D i in Gregorius)
            {
                i.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                i.setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0f, 0f, 0f), _camera.Position);
                i.setDirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f), new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f));
                i.setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                i.setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            }

            foreach (Assets3D i in Audrey)
            {
                i.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                i.setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0f, 0f, 0f), _camera.Position);
                i.setDirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f), new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f));
                i.setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                i.setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            }

            foreach (Assets3D i in Wall)
            {
                i.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                i.setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0f, 0f, 0f), _camera.Position);
                i.setDirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f), new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f));
                i.setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                i.setSpotLight(new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            }

            foreach (Assets3D i in Ground)
            {
                i.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
                i.setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0f, 0f, 0f), _camera.Position);
                i.setDirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f), new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.5f, 0.5f, 0.5f));
                i.setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
                i.setSpotLight(new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            }

            for (int i = 0; i < 4; i++)
            {
                LightObjects[i].render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());

            }



            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            float time = (float)args.Time;
            float cameraSpeed = 3f;

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            CameraMovement(cameraSpeed, time);
            KeyPress(time);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _camera.Fov = _camera.Fov - e.OffsetY;
        }

        protected void KeyPress(float time)
        {
            if (KeyboardState.IsKeyDown(Keys.J))
            {
                foreach (Assets3D i in Michael)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, 95 * time);
                }
                foreach (Assets3D i in Audrey)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, 95 * time);
                }
                foreach (Assets3D i in Gregorius)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, 95 * time);
                }
            }
            if (KeyboardState.IsKeyDown(Keys.K))
            {
                foreach (Assets3D i in Michael)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, -95 * time);
                }
                foreach (Assets3D i in Audrey)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, -95 * time);
                }
                foreach (Assets3D i in Gregorius)
                {
                    i.rotate(Vector3.Zero, Vector3.UnitY, -95 * time);
                }
            }
        }

        protected void CameraMovement(float cameraSpeed, float time)
        {
            var mouse = MouseState;
            var sensitivity = 0.2f;
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;

            }

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * time;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * time;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * time;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * time;
            }


            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * time;
            }
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _camera.Position -= _camera.Up * cameraSpeed * time;
            }

            if (KeyboardState.IsKeyDown(Keys.T))
            {
                _camera.Pitch += 0.5f;
            }
            if (KeyboardState.IsKeyDown(Keys.G))
            {
                _camera.Pitch -= 0.5f;
            }

            if (KeyboardState.IsKeyDown(Keys.E))
            {
                _camera.Yaw -= 0.5f;
            }
            if (KeyboardState.IsKeyDown(Keys.Q))
            {
                _camera.Yaw += 0.5f;
            }
        }


        public void setUpWall()
        {
            Wall[0].createCube(0f, 0f, 0f, 30f, 1f, 30f);
            Wall[1].createCube(Wall[0].getPos().X + (Wall[0].getlenx() / 2f) - 0.5f, Wall[0].getPos().Y + Wall[0].getleny() * 1.5f, Wall[0].getPos().Z + Wall[0].getlenz() * 0.1f, Wall[0].getlenx() / 30f, Wall[0].getleny() * 2f, Wall[0].getlenz() / 1.25f);
            Wall[2].createCube(Wall[1].getPos().X - Wall[1].getlenz() / 4f, Wall[1].getPos().Y, Wall[1].getPos().Z + (Wall[1].getlenz() / 2f) - Wall[1].getlenx() / 2f, Wall[1].getlenz() / 2f, Wall[1].getleny(), Wall[1].getlenx());
            Wall[3].createCube(Wall[2].getPos().X - Wall[2].getlenx() * 1.3f, Wall[2].getPos().Y, Wall[2].getPos().Z, Wall[1].getlenz() / 1.55f, Wall[1].getleny(), Wall[1].getlenx());
            Wall[4].createCube(Wall[1].getPos().X - Wall[0].getlenx() + Wall[1].getlenx(), Wall[1].getPos().Y, Wall[1].getPos().Z * 1.8f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 1.25f);
            Wall[5].createCube(Wall[4].getPos().X, Wall[4].getPos().Y, Wall[4].getPos().Z - Wall[4].getlenz() / 1.2f, Wall[4].getlenx(), Wall[4].getleny(), Wall[4].getlenz() / 2.15f);
            Wall[6].createCube(Wall[5].getPos().X + Wall[2].getlenx() / 1.7f, Wall[5].getPos().Y, Wall[5].getPos().Z - Wall[5].getlenz() / 2f + Wall[2].getlenz() / 2, Wall[2].getlenx() * 1.25f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[7].createCube(Wall[6].getPos().X + Wall[6].getlenx() / 2f, Wall[6].getPos().Y, Wall[6].getPos().Z + Wall[1].getlenz() / 9.5f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[8].createCube(Wall[7].getPos().X, Wall[7].getPos().Y, Wall[7].getPos().Z, Wall[2].getlenx() / 1.25f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[9].createCube(Wall[8].getPos().X + Wall[8].getlenx() / 2f, Wall[8].getPos().Y, Wall[8].getPos().Z + Wall[1].getlenz() / 9.5f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[10].createCube(Wall[9].getPos().X + Wall[2].getlenx() / 2.53f, Wall[9].getPos().Y, Wall[9].getPos().Z + Wall[9].getlenz() / 6f, Wall[2].getlenx() / 1.15f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[11].createCube(Wall[4].getPos().X + Wall[2].getlenx() / 2.18f, Wall[4].getPos().Y, Wall[4].getPos().Z - Wall[4].getlenz() / 2f, Wall[2].getlenx(), Wall[2].getleny(), Wall[2].getlenz());
            Wall[12].createCube(Wall[11].getPos().X, Wall[11].getPos().Y, Wall[11].getPos().Z - Wall[1].getlenz() / 6f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 3f);
            Wall[13].createCube(Wall[12].getPos().X + Wall[2].getlenx() / 2f, Wall[12].getPos().Y, Wall[12].getPos().Z + Wall[2].getlenz(), Wall[2].getlenx(), Wall[2].getleny(), Wall[2].getlenz());
            Wall[14].createCube(Wall[13].getPos().X + Wall[1].getlenx() * 3f, Wall[13].getPos().Y, Wall[13].getPos().Z + Wall[1].getlenz() / 8f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[15].createCube(Wall[11].getPos().X + Wall[1].getlenx() * 3f, Wall[11].getPos().Y, Wall[11].getPos().Z + Wall[1].getlenz() / 8f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[16].createCube(Wall[4].getPos().X + Wall[2].getlenx() / 4f, Wall[4].getPos().Y, Wall[4].getPos().Z, Wall[2].getlenx() / 2f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[17].createCube(Wall[16].getPos().X + Wall[1].getlenx() * 3f, Wall[16].getPos().Y, Wall[16].getPos().Z - Wall[1].getlenz() / 20f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 2f);
            Wall[18].createCube(Wall[17].getPos().X - Wall[2].getlenx() / 6f, Wall[17].getPos().Y, Wall[17].getPos().Z - Wall[17].getlenz() / 2.18f, Wall[2].getlenx() / 3f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[19].createCube(Wall[4].getPos().X + Wall[2].getlenx() / 5f, Wall[4].getPos().Y, Wall[4].getPos().Z - Wall[17].getlenz() / 3.5f, Wall[2].getlenx() / 3f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[20].createCube(Wall[4].getPos().X + Wall[1].getlenx() * 3f, Wall[4].getPos().Y, Wall[4].getPos().Z + Wall[1].getlenz() / 5f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 5f);
            Wall[21].createCube(Wall[20].getPos().X + Wall[2].getlenx() / 2f, Wall[20].getPos().Y, Wall[20].getPos().Z + Wall[20].getlenz() / 2.5f, Wall[2].getlenx(), Wall[2].getleny(), Wall[2].getlenz());
            Wall[22].createCube(Wall[21].getPos().X - Wall[21].getlenx() / 25f, Wall[21].getPos().Y, Wall[21].getPos().Z - Wall[1].getlenz() / 6f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 2.8f);
            Wall[23].createCube(Wall[21].getPos().X + Wall[21].getlenx() / 3.5f, Wall[21].getPos().Y, Wall[21].getPos().Z - Wall[1].getlenz() / 14f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 10f);
            Wall[24].createCube(Wall[21].getPos().X + Wall[21].getlenx() / 2.2f, Wall[21].getPos().Y, Wall[21].getPos().Z - Wall[1].getlenz() / 10f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 5f);
            Wall[25].createCube(Wall[24].getPos().X - Wall[24].getlenx() / 1.5f, Wall[24].getPos().Y, Wall[24].getPos().Z - Wall[2].getlenz() * 2.5f, Wall[2].getlenx() / 1.6f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[26].createCube(Wall[25].getPos().X - Wall[25].getlenx() / 2.3f, Wall[25].getPos().Y, Wall[25].getPos().Z, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[27].createCube(Wall[26].getPos().X + Wall[26].getlenx() * 1.9f, Wall[26].getPos().Y, Wall[26].getPos().Z - Wall[26].getlenz() / 2f, Wall[2].getlenx() / 2.5f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[28].createCube(Wall[25].getPos().X + Wall[25].getlenx() / 2f, Wall[25].getPos().Y, Wall[25].getPos().Z + Wall[25].getlenz() * 1.1f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 2.75f);
            Wall[29].createCube(Wall[28].getPos().X + Wall[2].getlenx() / 3f, Wall[28].getPos().Y, Wall[28].getPos().Z + Wall[2].getlenz() * 3.85f, Wall[2].getlenx() / 1.5f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[30].createCube(Wall[29].getPos().X + Wall[29].getlenx() / 2f, Wall[29].getPos().Y, Wall[29].getPos().Z - Wall[1].getlenz() / 12.5f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 5f);
            Wall[31].createCube(Wall[28].getPos().X + Wall[2].getlenx() / 2.18f, Wall[28].getPos().Y, Wall[28].getPos().Z - Wall[28].getlenz() / 2.1f, Wall[2].getlenx(), Wall[2].getleny(), Wall[2].getlenz());
            Wall[32].createCube(Wall[31].getPos().X - Wall[31].getlenx() / 5f, Wall[31].getPos().Y, Wall[31].getPos().Z + Wall[1].getlenz() / 15f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 6f);
            Wall[33].createCube(Wall[31].getPos().X, Wall[31].getPos().Y, Wall[31].getPos().Z + Wall[1].getlenz() / 9f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 4f);
            Wall[34].createCube(Wall[33].getPos().X - Wall[2].getlenx() / 8f, Wall[33].getPos().Y, Wall[33].getPos().Z + Wall[33].getlenz() / 2.1f, Wall[2].getlenx() / 3f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[35].createCube(Wall[14].getPos().X - Wall[2].getlenx() / 6f, Wall[14].getPos().Y, Wall[14].getPos().Z + Wall[14].getlenz() / 2.4f, Wall[2].getlenx() / 4f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[36].createCube(Wall[13].getPos().X + Wall[13].getlenx() / 2.2f, Wall[13].getPos().Y, Wall[13].getPos().Z + Wall[1].getlenz() / 5.5f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 3f);
            Wall[37].createCube(Wall[36].getPos().X - Wall[1].getlenx() / 2f, Wall[36].getPos().Y, Wall[36].getPos().Z + Wall[36].getlenz() / 2f, Wall[2].getlenx() / 1.1f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[38].createCube(Wall[36].getPos().X + Wall[2].getlenx() / 3.3f, Wall[36].getPos().Y, Wall[36].getPos().Z - Wall[36].getlenz() / 8f, Wall[2].getlenx() / 1.5f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[39].createCube(Wall[38].getPos().X + Wall[38].getlenx() / 8f, Wall[38].getPos().Y, Wall[38].getPos().Z - Wall[38].getlenz(), Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 8f);
            Wall[40].createCube(Wall[1].getPos().X - Wall[2].getlenx() / 2.5f, Wall[1].getPos().Y, Wall[1].getPos().Z - Wall[1].getlenz() / 5.5f, Wall[2].getlenx() / 1.25f, Wall[2].getleny(), Wall[2].getlenz());
            Wall[41].createCube(Wall[40].getPos().X, Wall[40].getPos().Y, Wall[40].getPos().Z + Wall[1].getlenz() / 15f, Wall[1].getlenx(), Wall[1].getleny(), Wall[1].getlenz() / 6f);
        }
    }
}