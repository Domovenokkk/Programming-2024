using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAYTRACING_LAB_3
{
    class View
    {
        int vbo_position;
        int BasicProgramID;
        uint BasicVertexShader;
        uint BasicFragmentShader;
        Vector3 campos;
        int attribute_vpos;
        int uniform_pos;
        int uniform_aspect;
        int aspect;

        public int tetr, cube, bigSphere, smallSphere;


        public int BigSphere;
        public Vector3 ColorBigSphere;
        public int SmallSphere;
        public Vector3 ColorSmallSphere;

        public int Tetraeder;
        public Vector3 ColorTetraeder;

        public int Cube;
        public Vector3 ColorCube;

        public Vector3 ColorBack;
        public Vector3 ColorFront;
        public Vector3 ColorLeft;
        public Vector3 ColorRight;
        public Vector3 ColorTop;
        public Vector3 ColorBottom;

        public int Back;
        public int Front;
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
        public int RTDepth;

        public int bigCf;
        public int bigRCf;

        public int wallCf;
        public int wallRCf;

        public View()
        {
            vbo_position = 0;
            BasicProgramID = 0;
            BasicVertexShader = 0;
            BasicFragmentShader = 0;
            campos = new Vector3(0, 0, 0);
            attribute_vpos = 0;
            uniform_pos = 0;
            uniform_aspect = 0;
            aspect = 0;
            ColorBigSphere = new Vector3(1.0f, 1.0f, 0.0f);
            SmallSphere = 2;
            BigSphere = 2;
            ColorSmallSphere = new Vector3(0.0f, 1.0f, 0.0f);
            tetr = 0; cube = 0;
            bigSphere = 0; smallSphere = 0;

            Tetraeder = 2;
            ColorTetraeder = new Vector3(0.0f, 1.0f, 0.0f);
            Cube = 2;
            ColorCube = new Vector3(1.0f, 0.0f, 0.0f);

            ColorBack = ColorFront = ColorLeft = ColorRight = ColorTop = ColorBottom = new Vector3(1.0f, 1.0f, 1.0f);
            Back = Front = Left = Right = Top = Bottom = 2;
            RTDepth = 6;
            bigCf = 100;
            bigRCf = 130;
            wallCf = 100;
            wallRCf = 130;
        }
        public void DrawNewFrame()
        {
            GL.UseProgram(BasicProgramID);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "BigSphere"), BigSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBigSphere"), ref ColorBigSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "SmallSphere"), SmallSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColSmallSphere"), ref ColorSmallSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColCube"), ref ColorCube);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColTetr"), ref ColorTetraeder);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Cube"), Cube);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Tetr"), Tetraeder);

            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "utetr"), tetr);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "ubigs"), bigSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "usmalls"), smallSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "ucube"), cube);

            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBack"), ref ColorBack);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColFront"), ref ColorFront);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColLeft"), ref ColorLeft);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColRight"), ref ColorRight);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColTop"), ref ColorTop);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBot"), ref ColorBottom);

            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Back"), Back);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Front"), Front);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Left"), Left);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Right"), Right);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Top"), Top);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Bot"), Bottom);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "RTDepth"), RTDepth);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "bigCf"), bigCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "bigRCf"), bigRCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "wallCf"), wallCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "wallRCf"), wallRCf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.DisableVertexAttribArray(attribute_vpos);

        }
        public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\raytracing.vert", ShaderType.VertexShader, (uint)BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\raytracing.frag", ShaderType.FragmentShader, (uint)BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
        }
        void loadShader(String filename, ShaderType type, uint program, out uint address)
        {
            address = (uint)GL.CreateShader(type);
            using (System.IO.StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource((int)address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog((int)address));
        }

        public void initVBO()
        {
            Vector3[] vertdata = new Vector3[]
            {
                new Vector3(-1f, -1f, 0f),
                new Vector3( 1f, -1f, 0f),
                new Vector3( 1f,  1f, 0f),
                new Vector3(-1f,  1f, 0f)};
            GL.GenBuffers(1, out vbo_position);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position); GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.Uniform3(uniform_pos, campos);
            GL.Uniform1(uniform_aspect, aspect);
            

            GL.UseProgram(BasicProgramID);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "BigSphere"), BigSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBigSphere"), ref ColorBigSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "SmallSphere"), SmallSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColSmallSphere"), ref ColorSmallSphere);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColCube"), ref ColorCube);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColTetr"), ref ColorTetraeder);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Cube"), Cube);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Tetr"), Tetraeder);

            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "utetr"), tetr);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "ubigs"), bigSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "usmalls"), smallSphere);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "ucube"), cube);

            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBack"), ref ColorBack);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColFront"), ref ColorFront);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColLeft"), ref ColorLeft);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColRight"), ref ColorRight);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColTop"), ref ColorTop);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "ColBot"), ref ColorBottom);

            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Back"), Back);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Front"), Front);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Left"), Left);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Right"), Right);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Top"), Top);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "Bot"), Bottom);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "RTDepth"), RTDepth);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "bigCf"), bigCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "bigRCf"), bigRCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "wallCf"), wallCf);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "wallRCf"), wallRCf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }





    }
}
