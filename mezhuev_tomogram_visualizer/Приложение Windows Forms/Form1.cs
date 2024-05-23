using System;
using System.Windows.Forms;

namespace Приложение_Windows_Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar2.Scroll += trackBar2_Scroll;
            trackBar3.Scroll += trackBar3_Scroll;
            glControl1.Paint += glControl1_Paint;
        }

        enum Mode { Quads, Texture, QuadStrip };
        private Mode mode = Mode.Quads;
        private Bin bin;
        private View view;
        private bool loaded = false;
        private int currentLayer;
        private DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);
        private int frameCount;
        private bool needReload = false;
        private int min;
        private int width;

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
            bin = new Bin();
            view = new View();
            currentLayer = 1;
            min = trackBar2.Value;
            width = trackBar3.Value;
            radioButton1.Checked = true;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                if (bin == null)
                {
                    bin = new Bin();
                }
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                if (view == null)
                {
                    view = new View();
                }
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                switch (mode)
                {
                    case Mode.Quads:
                        view.DrawQuads(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                    case Mode.Texture:
                        if (needReload)
                        {
                            view.generateTextureImage(currentLayer, min, width);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                        glControl1.SwapBuffers();
                        break;
                    case Mode.QuadStrip:
                        view.DrawQuadStrip(currentLayer, min, width);
                        glControl1.SwapBuffers();
                        break;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            glControl1.Invalidate();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            min = trackBar2.Value;
            needReload = true;
            glControl1.Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            width = trackBar3.Value;
            needReload = true;
            glControl1.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                mode = Mode.Quads;
                glControl1.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                mode = Mode.QuadStrip;
                glControl1.Invalidate();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                mode = Mode.Texture;
                needReload = true;
                glControl1.Invalidate();
            }
        }

        void displayFPS()
        {
            if (DateTime.Now >= nextFPSUpdate)
            {
                this.Text = $"CT Visualizer (fps={frameCount})";
                nextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
    }
}
