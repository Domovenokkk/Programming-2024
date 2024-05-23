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
            glControl1.Paint += glControl1_Paint;
        }

        enum Mode { Quads, Texture2D, QuadStrip };
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
            //min = trackBar2.Value;
            //width = trackBar3.Value;
            //radioButton1.Checked = true;
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
                view.DrawQuads(currentLayer);
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            glControl1.Invalidate(); // Redraw with the new layer
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
