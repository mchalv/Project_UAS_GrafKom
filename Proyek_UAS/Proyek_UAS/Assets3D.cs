using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyek_UAS
{
    class Assets3D
    {
        public List<Vector3> vertices = new List<Vector3>();
        private List<uint> indices = new List<uint>();

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private Shader _shader;

        private Matrix4 model = Matrix4.Identity;
        private Matrix4 view;
        private Matrix4 projection;

        private Vector3 color;

        public List<Vector3> _euler = new List<Vector3>();
        public Vector3 objectCenter = Vector3.Zero;

        public Vector3 _centerPosition = new Vector3(0, 0, 0);

        List<float> realVertices = new List<float>();
        List<Vector3> textureVertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();

        protected float RadX, RadY, RadZ;
        protected float posX, posY, posZ;
        protected float LengthX, LengthY, LengthZ;
        protected float Radius, Height, Extended;

        protected float _size = 1;

        public Assets3D()
        {
        }

        public Assets3D(Vector3 color)
        {
            this.color = color;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }

        public void load(string shadervert, string shaderfrag, int sizeX, int sizeY)
        {
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            if (indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            view = Matrix4.CreateTranslation(0, 0, -8.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), sizeX / (float)sizeY, 0.01f, 100f);

            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

        }

        public void load_withnormal(string shadervert, string shaderfrag, float sizeX, float sizeY)
        {
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            if (indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            view = Matrix4.CreateTranslation(0, 0, -8.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), sizeX / (float)sizeY, 0.1f, 100f);

            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

        }


        public void render(int line, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            Matrix4 model = Matrix4.Identity;

            model = temp;

            _shader.SetVector3("objColor", color);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);
            if (indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                switch (line)
                {
                    case 0:
                        GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);
                        break;
                    case 1:
                        GL.DrawArrays(PrimitiveType.LineStrip, 0, vertices.Count);
                        break;
                }
            }
        }

        /// <summary>
        /// Berfungsi untuk me-reset sudut euler (sudut relatif terhadap objek)
        /// </summary>
        public void resetEuler()
        {
            _euler.Clear();
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }

        public Vector3 getPos() { return _centerPosition; }
        public float getradx() { return RadX; }
        public float getrady() { return RadY; }
        public float getradz() { return RadZ; }

        public float getrad() { return Radius; }
        public float getheight() { return Height; }

        public float getlenx() { return LengthX; }
        public float getleny() { return LengthY; }
        public float getlenz() { return LengthZ; }

        #region Pencahayaan

        public void setFragVariable(Vector3 ObjectColor, Vector3 LightColor, Vector3 LightPos, Vector3 ViewPos)
        {
            _shader.SetVector3("objColor", ObjectColor);
            _shader.SetVector3("viewPos", ViewPos);
        }
        public void setDirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
        {
            _shader.SetVector3("dirLight.direction", direction);
            _shader.SetVector3("dirLight.ambient", ambient);
            _shader.SetVector3("dirLight.diffuse", diffuse);
            _shader.SetVector3("dirLight.specular", specular);
        }
        public void setPointLight(Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            _shader.SetVector3("pointLight.position", position);
            _shader.SetVector3("pointLight.ambient", ambient);
            _shader.SetVector3("pointLight.diffuse", diffuse);

            _shader.SetVector3("pointLight.specular", specular);
            _shader.SetFloat("pointLight.constant", constant);
            _shader.SetFloat("pointLight.linear", linear);
            _shader.SetFloat("pointLight.quadratic", quadratic);
        }
        public void setPointLights(Vector3[] position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
        {
            for (int i = 0; i < position.Length; i++)
            {
                _shader.SetVector3($"pointLight[{i}].position", position[i]);
                _shader.SetVector3($"pointLight[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _shader.SetVector3($"pointLight[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _shader.SetVector3($"pointLight[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetFloat($"pointLight[{i}].constant", 1.0f);
                _shader.SetFloat($"pointLight[{i}].linear", 0.09f);
                _shader.SetFloat($"pointLight[{i}].quadratic", 0.032f);
            }

        }
        public void setSpotLight(Vector3 position, Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic, float cutOff, float outerCutOff)
        {
            _shader.SetVector3("spotLight.position", position);
            _shader.SetVector3("spotLight.direction", direction);
            _shader.SetVector3("spotLight.ambient", ambient);
            _shader.SetVector3("spotLight.diffuse", diffuse);

            _shader.SetVector3("spotLight.specular", specular);
            _shader.SetFloat("spotLight.constant", constant);
            _shader.SetFloat("spotLight.linear", linear);
            _shader.SetFloat("spotLight.quadratic", quadratic);
            _shader.SetFloat("spotLight.cutOff", cutOff);
            _shader.SetFloat("spotLight.outerCutOff", outerCutOff);
        }

        #endregion

        #region ObjekLingkaran

        public void createSphere(float _x, float _y, float _z, float radX, float radY, float radZ, int sectorCount, int stackCount)
        {
            _centerPosition = new Vector3(_x, _y, _z);
            RadX = radX;
            RadY = radY;
            RadZ = radZ;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = pi / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                posX = RadX * (float)Math.Cos(stackAngle);
                posY = RadY * (float)Math.Sin(stackAngle);
                posZ = RadZ * (float)Math.Cos(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + posX * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y + posY;
                    temp_vector.Z = _z + posZ * (float)Math.Sin(sectorAngle);

                    vertices.Add(temp_vector);
                }
            }

            posX = objectCenter.X;
            posY = objectCenter.Y;
            posZ = objectCenter.Z;

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);

                    }

                    if (i != stackCount - 1)
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }
        }
        public void createHalfSphere(float _x, float _y, float _z, float radX, float radY, float radZ, float sectorCount, float stackCount)
        {
            objectCenter = new Vector3(_x, _y, _z);
            RadX = radX;
            RadY = radY;
            RadZ = radZ;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = pi / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                posX = RadX * (float)Math.Cos(stackAngle);
                posY = RadY * (float)Math.Cos(stackAngle);
                posZ = RadZ * (float)Math.Sin(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = _x + posX * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = _y + posY * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = _z + posZ;

                    vertices.Add(temp_vector);
                }
            }

            posX = objectCenter.X;
            posY = objectCenter.Y;
            posZ = objectCenter.Z;

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount / 2; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        indices.Add(k1);
                        indices.Add(k2);
                        indices.Add(k1 + 1);

                    }

                    if (i != stackCount - 1)
                    {
                        indices.Add(k1 + 1);
                        indices.Add(k2);
                        indices.Add(k2 + 1);
                    }
                }
            }
        }

        #endregion

        #region ObjekKerucut

        public void createCone(float _x, float _y, float _z, float radius, float height, float slices)
        {
            Vector3 temp_vector;
            float sliceArc = 360.0f / (float)slices;
            float angle = 0;
            Radius = radius;
            Height = height;
            for (int i = 0; i < slices; i++)
            {
                temp_vector.X = _x + Radius * (float)Math.Cos(MathHelper.DegreesToRadians(angle));
                temp_vector.Y = _y + 0;
                temp_vector.Z = _z + Radius * (float)Math.Sin(MathHelper.DegreesToRadians(angle));
                vertices.Add(temp_vector);

                temp_vector.X = _x;
                temp_vector.Y = _y + Height;
                temp_vector.Z = _z;

                vertices.Add(temp_vector);
                angle += sliceArc;
            }
            _centerPosition = new Vector3(_x, _y, _z);
        }

        #endregion

        #region ObjekKurva

        public void createEllipsoidVertices(float _x, float _y, float _z, float radiusX, float height, float radiusZ, float extended)
        {
            posX = _x;
            posY = _y;
            posZ = _z;
            RadX = radiusX;
            RadZ = radiusZ;
            Height = height;
            Extended = extended;

            Vector3 temp_vector;
            float _pi = (float)Math.PI;

            for (float v = -Height / 2; v <= (Height / 2); v += 0.0001f)
            {
                Vector3 p = setBeizer((v + (Height / 2)) / Height);
                for (float u = -_pi; u <= _pi; u += (_pi / 30))
                {
                    temp_vector.X = p.X + RadX * (float)Math.Cos(u);
                    temp_vector.Y = p.Y + RadZ * (float)Math.Sin(u);
                    temp_vector.Z = posZ + v;

                    vertices.Add(temp_vector);

                }
            }
        }
        Vector3 setBeizer(float t)
        {
            //Console.WriteLine(t);
            Vector3 p = new Vector3(0f, 0f, 0f);
            float[] k = new float[3];

            k[0] = (float)Math.Pow((1 - t), 3 - 1 - 0) * (float)Math.Pow(t, 0) * 1;
            k[1] = (float)Math.Pow((1 - t), 3 - 1 - 1) * (float)Math.Pow(t, 1) * 2;
            k[2] = (float)Math.Pow((1 - t), 3 - 1 - 2) * (float)Math.Pow(t, 2) * 1;


            //titik 1
            p.X += k[0] * posX;
            p.Y += k[0] * posY - Height;

            //titik 2
            p.X += k[1] * (posX + Extended);
            p.Y += k[1] * posY;

            //titik 3
            p.X += k[2] * posX;
            p.Y += k[2] * posY + Height;

            //Console.WriteLine(p.X + " "+ p.Y);

            return p;
        }

        #endregion

        #region ObjekBalok

    //    public void createCube(float _x, float _y, float _z, float length_x, float length_y, float length_z)
    //    {
    //        this.posX = _x;
    //        this.posY = _y;
    //        this.posZ = _z;

    //        this.LengthX = length_x;
    //        this.LengthY = length_y;
    //        this.LengthZ = length_z;

    //        var tempVertices = new List<Vector3>();
    //        Vector3 temp_vector;

    //        //Titik 1
    //        temp_vector.X = _x - length_x / 2.0f;
    //        temp_vector.Y = _y + length_y / 2.0f;
    //        temp_vector.Z = _z - length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 2
    //        temp_vector.X = _x + length_x / 2.0f;
    //        temp_vector.Y = _y + length_y / 2.0f;
    //        temp_vector.Z = _z - length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 3
    //        temp_vector.X = _x - length_x / 2.0f;
    //        temp_vector.Y = _y - length_y / 2.0f;
    //        temp_vector.Z = _z - length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 4
    //        temp_vector.X = _x + length_x / 2.0f;
    //        temp_vector.Y = _y - length_y / 2.0f;
    //        temp_vector.Z = _z - length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 5
    //        temp_vector.X = _x - length_x / 2.0f;
    //        temp_vector.Y = _y + length_y / 2.0f;
    //        temp_vector.Z = _z + length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 6
    //        temp_vector.X = _x + length_x / 2.0f;
    //        temp_vector.Y = _y + length_y / 2.0f;
    //        temp_vector.Z = _z + length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 7
    //        temp_vector.X = _x - length_x / 2.0f;
    //        temp_vector.Y = _y - length_y / 2.0f;
    //        temp_vector.Z = _z + length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        //Titik 8
    //        temp_vector.X = _x + length_x / 2.0f;
    //        temp_vector.Y = _y - length_y / 2.0f;
    //        temp_vector.Z = _z + length_z / 2.0f;
    //        tempVertices.Add(temp_vector);

    //        var tempIndices = new List<uint>
    //        {
				////Back
				//1, 2, 0,
    //            2, 1, 3,
				
				////Top
				//5, 0, 4,
    //            0, 5, 1,

				////Right
				//5, 3, 1,
    //            3, 5, 7,

				////Left
				//0, 6, 4,
    //            6, 0, 2,

				////Front
				//4, 7, 5,
    //            7, 4, 6,

				////Bottom
				//3, 6, 2,
    //            6, 3, 7
    //        };
    //        vertices = tempVertices;
    //        indices = tempIndices;
    //    }

        public void createCube(float x, float y, float z, float length_x, float length_y, float length_z)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            this.LengthX = length_x;
            this.LengthY = length_y;
            this.LengthZ = length_z;

            //FRONT FACE

            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));


            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, -1.0f));

            //BACK FACE
            //TITIK 5
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 6
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));
            //TITIK 7
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 6
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));
            //TITIK 7
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //TITIK 8
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 0.0f, 1.0f));

            //LEFT FACE
            //TITIK 1
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 3
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 3
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(-1.0f, 0.0f, 0.0f));

            //RIGHT FACE
            //TITIK 2
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));
            //TITIK 8
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(1.0f, 0.0f, 0.0f));

            //BOTTOM FACES
            //TITIK 3
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 4
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 7
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));
            //TITIK 8
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y - length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, -1.0f, 0.0f));

            //TOP FACES
            //TITIK 1
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 2
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 2
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z - length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 5
            temp_vector.X = x - length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
            //TITIK 6
            temp_vector.X = x + length_x / 2.0f;
            temp_vector.Y = y + length_y / 2.0f;
            temp_vector.Z = z + length_z / 2.0f;
            vertices.Add(temp_vector);
            vertices.Add(new Vector3(0.0f, 1.0f, 0.0f));
        }

        #endregion

        #region ObjekTabung

        public void createTube(float _x, float _y, float _z, uint segments, float radius, float height)
        {
            Radius = radius;
            Height = height;
            for (double y = 0; y < 2; y++)
            {
                for (double x = 0; x < segments; x++)
                {
                    double theta = (x / (segments - 1)) * 2 * Math.PI;

                    posX = (float)(Radius * Math.Cos(theta)) + _x;
                    posY = (float)(Height * y) + _y;
                    posZ = (float)(Radius * Math.Sin(theta)) + _z;

                    vertices.Add(new Vector3()
                    {
                        X = posX,
                        Y = posY,
                        Z = posZ,
                    });
                }
            }
            for (uint x = 0; x < segments - 1; x++)
            {
                indices.Add(x);
                indices.Add(x + segments);
                indices.Add(x + segments + 1);

                indices.Add(x + segments + 1);
                indices.Add(x + 1);
                indices.Add(x);
            }
        }

        #endregion

        #region transforms
        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            var radAngle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            model *= Matrix4.CreateTranslation(-pivot);
            model *= arbRotationMatrix;
            model *= Matrix4.CreateTranslation(pivot);

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

            objectCenter = getRotationResult(pivot, vector, radAngle, objectCenter);

        }

        public void rotatecenter(float angle = 0.01f, char a = 'x')
        {
            Vector4 pos = new Vector4(getPos());
            switch (a)
            {
                case 'x':
                    model = model * Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z);
                    
                    pos = Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z) * pos;

                    break;
                case 'y':
                    model = model * Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z);

                    pos = Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z) * pos;

                    break;
                case 'z':
                    model = model * Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z);
                    Console.WriteLine(_centerPosition.X);
                    Console.WriteLine(_centerPosition.Y);
                    Console.WriteLine(_centerPosition.Z);
                    pos = Matrix4.CreateTranslation(-1 * _centerPosition.X, -1 * _centerPosition.Y, -1 * _centerPosition.Z) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle)) * Matrix4.CreateTranslation(_centerPosition.X, _centerPosition.Y, _centerPosition.Z) * pos;

                    break;
            }
        }

        public Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;

            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));

            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));

            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void translate(float x, float y, float z)
        {
            model *= Matrix4.CreateTranslation(x, y, z);
            objectCenter.X += x;
            objectCenter.Y += y;
            objectCenter.Z += z;

        }

        //public void scale(float scaleX, float scaleY, float scaleZ)
        //{
        //    model *= Matrix4.CreateTranslation(-objectCenter);
        //    model *= Matrix4.CreateScale(scaleX, scaleY, scaleZ);
        //    model *= Matrix4.CreateTranslation(objectCenter);

        //    foreach (var i in child)
        //    {
        //        i.scale(scaleX, scaleY, scaleZ);
        //    }
        //}

        public void scale(float m = 2)
        {
            if (m == 1)
            {
                model = model * Matrix4.CreateScale(1);
                return;
            }
            model = model * Matrix4.CreateScale(_size + m * _size);
            posX *= (_size + m * _size);
            posY *= (_size + m * _size);
            posZ *= (_size + m * _size);
        }
        #endregion

    }
}
