using OpenTK;
using OpenTK.Graphics.ES30;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ray_Tracing
{
    public partial class Form1 : Form
    {
        private View view;

        public Form1()
        {
            InitializeComponent();
            view = new View();
            glControl1.Paint += glControl1_Paint;
            glControl1.Load += glControl1_Load;
            glControl1.Resize += glControl1_Resize;
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            view.RenderFrame();
            glControl1.SwapBuffers();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.ClearColor(Color.Black);
            view.InitShaders();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            view.SetAspectRatio((float)glControl1.Width / (float)glControl1.Height);
        }
    }
}