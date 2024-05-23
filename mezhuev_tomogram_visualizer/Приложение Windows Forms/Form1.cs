using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Приложение_Windows_Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        enum Mode { Quads, Texture2D, QuadStrip };
        private Mode mode = Mode.Quads;
        private Bin bin;
        private View view;
        private bool loaded = false;
        private int currentLayer;
        private DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        private int FrameCount;
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
                    view= new View();
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
        }

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualaiser (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
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
