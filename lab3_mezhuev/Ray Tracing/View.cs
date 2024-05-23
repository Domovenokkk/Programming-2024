using OpenTK.Graphics.ES30;
using System;
using System.IO;
using OpenTK;

namespace Ray_Tracing
{
    class View
    {
        private int BasicVertexShader;
        private int BasicFragmentShader;
        private int BasicProgramID;
        private int vbo_position;
        private int attribute_vpos;
        private int uniform_pos;
        private int uniform_aspect;
        private Vector3 campos = new Vector3(0.0f, 0.0f, 5.0f);
        private float aspect = 1.0f;
        private Vector3[] vertdata;

        public void loadShader(string filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"Shader file not found: {filename}");
            }

            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);

            string infoLog = GL.GetShaderInfoLog(address);
            if (!string.IsNullOrEmpty(infoLog))
            {
                Console.WriteLine($"Ошибка компиляции шейдера ({type}): {infoLog}");
            }

            GL.AttachShader(program, address);
        }

        public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();

            string vertexShaderPath = "C:\\Users\\Max\\source\\Prog-2024\\lab3_mezhuev\\Ray Tracing\\raytracing.vert";
            string fragmentShaderPath = "C:\\Users\\Max\\source\\Prog-2024\\lab3_mezhuev\\Ray Tracing\\raytracing.frag";

            // Вывод текущего рабочего каталога для отладки
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

            // Проверка существования файлов
            Console.WriteLine($"Vertex Shader exists: {File.Exists(vertexShaderPath)}");
            Console.WriteLine($"Fragment Shader exists: {File.Exists(fragmentShaderPath)}");

            loadShader(vertexShaderPath, ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader(fragmentShaderPath, ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);

            int status;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);

            string programInfoLog = GL.GetProgramInfoLog(BasicProgramID);
            if (!string.IsNullOrEmpty(programInfoLog))
            {
                Console.WriteLine($"Ошибка линковки программы: {programInfoLog}");
            }

            GL.GenBuffers(1, out vbo_position);
            vertdata = new Vector3[] {
                new Vector3(-1f, -1f, 0f),
                new Vector3(1f, -1f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(-1f, 1f, 0f)
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
        }

        public void drawQuad()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(BasicProgramID);

            attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vertexPosition");
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            uniform_pos = GL.GetUniformLocation(BasicProgramID, "uCamera");
            uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");

            GL.Uniform3(uniform_pos, ref campos);
            GL.Uniform1(uniform_aspect, aspect);

            GL.DrawArrays(PrimitiveType.Quads, 0, vertdata.Length);

            // Проверка ошибок OpenGL
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine($"OpenGL Error: {error}");
            }

            GL.DisableVertexAttribArray(attribute_vpos);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);
        }

        public void RenderFrame()
        {
            drawQuad();
        }

        public void SetAspectRatio(float newAspect)
        {
            aspect = newAspect;
        }
    }
}