namespace Lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap image;

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*bmp|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK ) { image = new Bitmap(dialog.FileName); }
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Bitmap newimage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
            {
                image = newimage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sepiya();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new AddBrightness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sobel();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �����������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Filters filter = new MotionBlur();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Transfer();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Turn();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WaveVertical();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ��������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WaveHorizontal();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                MessageBox.Show("����������� �� �������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Files (.png)|.png|JPEG Files (.jpg)|.jpg|BMP Files (.bmp)|.bmp|All files (.)|.";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(saveDialog.FileName);
                    MessageBox.Show("����������� ���������.", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� ���������� �����������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ���������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Size = new Size(1024,768);
            //progressBar1.Location = new Point(12, 700);
            //button1.Location = new Point(694, 700);
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorld(image);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void �������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new PerfectReflector(image);
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}