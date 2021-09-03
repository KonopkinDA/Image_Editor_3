using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public class Pixel
    {
        public int R;
        public int G;
        public int B;

        public double SR
        {
            get
            {
                return ((R + G + B) / 3);
            }
        }
    }
    public partial class Form1 : Form
    {
        Bitmap image;
        Pixel[,] mImage;
        List<string> methods;

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            mImage = new Pixel[image.Width, image.Height];
            methods = new List<string> {"Критерий Гаврилова", "Критерий Отсу", "Критерий Ниблека", "Критерий Сауволы",
        "Критерий Кристиана Вульфа","Критерий Бредли-Рота"};

            pictureBox1.Image = image;
            foreach (var item in methods)
            {
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = 0;
            textBox1.Visible = false;
            textBox2.Visible = false;
            comboBox1.SelectedIndexChanged += MethodStart;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                Bitmap imageT = new Bitmap(openFileDialog.FileName);
                image = new Bitmap(imageT, pictureBox1.Width, pictureBox1.Height);
                imageT.Dispose();
                pictureBox1.Image = image;
            }

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    mImage[i, j] = new Pixel();
                }
            }
            mImage[0, 0].R = 2;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color c = image.GetPixel(i, j);
                    mImage[i, j].R = c.R;
                    mImage[i, j].G = c.G;
                    mImage[i, j].B = c.B;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDileFialog = new SaveFileDialog();
            saveDileFialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveDileFialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            saveDileFialog.RestoreDirectory = true;
            image = (Bitmap)pictureBox2.Image;

            if (saveDileFialog.ShowDialog() == DialogResult.OK)
            {
                if (image != null)
                {
                    image.Save(saveDileFialog.FileName);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    pictureBox2.Image = GavrilovMethod(mImage);
                    break;
                case 1:
                    pictureBox2.Image = OtsuMethod(mImage);
                    break;
                case 2:
                    pictureBox2.Image = NiblecMethod(mImage, Convert.ToInt32(textBox1.Text), Convert.ToDouble(textBox2.Text));
                    break;
                case 3:
                    pictureBox2.Image = SauvolaMethod(mImage, Convert.ToInt32(textBox1.Text), Convert.ToDouble(textBox2.Text));
                    break;
                case 4:
                    pictureBox2.Image = KristianVoolfMethod(mImage, Convert.ToInt32(textBox1.Text));
                    break;
                case 5:
                    pictureBox2.Image = BredlyRotMethod(mImage, Convert.ToInt32(textBox1.Text), Convert.ToDouble(textBox2.Text));
                    break;
            }
        }

        public Bitmap GavrilovMethod(Pixel[,] pImage)
        {
            Bitmap nImage = new Bitmap(image);

            double sr = 0;

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    sr += pImage[i, j].SR;
                }
            }

            sr /= (nImage.Width * nImage.Height);

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR <= sr)
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public Bitmap OtsuMethod(Pixel[,] pImage)
        {
            Bitmap nImage = new Bitmap(image);
            double[] N = new double[256];

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    N[(int)(pImage[i, j].SR)] += 1;
                }
            }

            for (int l = 0; l < N.Length; l++)
            {
                N[l] /= nImage.Width * nImage.Height;
            }

            double sygma = 0;
            int tR = 0;

            double rt = 0;
            for (int i = 0; i < 256; i++)
            {
                rt += (i * N[i]);
            }

            for (int t = 0; t < 256; t++)
            {
                double w1 = 0, w2 = 0;
                for (int i = 0; i < t - 1; i++)
                {
                    w1 += N[i];
                }
                w2 = 1 - w1;

                double r1 = 0, r2 = 0;
                for (int i = 0; i < t - 1; i++)
                {
                    r1 += (i * N[i]);
                }
                r1 /= w1;

                r2 = (rt - (r1 * w1)) / (w2);

                double nSygma = w1 * w2 * (r1 - r2) * (r1 - r2);

                if (nSygma > sygma)
                {
                    sygma = nSygma;
                    tR = t;
                }

            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR <= tR)
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public Bitmap NiblecMethod(Pixel[,] pImage, int a, double k)
        {
            Bitmap nImage = new Bitmap(image);

            int[,] t = new int[image.Width, image.Height];

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    int ni, nj;
                    int ki, kj;
                    ni = i - (int)(a / 2);
                    nj = j - (int)(a / 2);
                    ki = i + (int)(a / 2);
                    kj = j + (int)(a / 2);

                    double m = 0, mQ = 0;
                    int mCount = 0;

                    for (int i2 = ni; i2 < ki + 1; i2++)
                    {
                        for (int j2 = nj; j2 < kj + 1; j2++)
                        {
                            if (i2 >= 0 && i2 < nImage.Width)
                            {
                                if (j2 >= 0 && j2 < nImage.Height)
                                {
                                    m += pImage[i2, j2].SR;
                                    mQ += pImage[i2, j2].SR * pImage[i2, j2].SR;
                                    mCount++;
                                }
                            }
                        }
                    }

                    m /= mCount;
                    mQ /= mCount;

                    double d = 0;
                    d = mQ - (m * m);

                    double sygma = 0;
                    sygma = Math.Sqrt(d);

                    t[i, j] = (int)(m + k * sygma);

                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR <= t[i, j])
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public Bitmap SauvolaMethod(Pixel[,] pImage, int a, double k)
        {
            Bitmap nImage = new Bitmap(image);

            int[,] t = new int[image.Width, image.Height];

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    int ni, nj;
                    int ki, kj;
                    ni = i - (int)(a / 2);
                    nj = j - (int)(a / 2);
                    ki = i + (int)(a / 2);
                    kj = j + (int)(a / 2);

                    double m = 0, mQ = 0;
                    int mCount = 0;

                    for (int i2 = ni; i2 < ki + 1; i2++)
                    {
                        for (int j2 = nj; j2 < kj + 1; j2++)
                        {
                            if (i2 >= 0 && i2 < nImage.Width)
                            {
                                if (j2 >= 0 && j2 < nImage.Height)
                                {
                                    m += pImage[i2, j2].SR;
                                    mQ += pImage[i2, j2].SR * pImage[i2, j2].SR;
                                    mCount++;
                                }
                            }
                        }
                    }

                    m /= mCount;
                    mQ /= mCount;

                    double d = 0;
                    d = mQ - (m * m);

                    double sygma = 0;
                    sygma = Math.Sqrt(d);

                    t[i, j] = (int)(m * (1 + k * (sygma / 128 - 1)));

                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR <= t[i, j])
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public Bitmap KristianVoolfMethod(Pixel[,] pImage, int a)
        {
            Bitmap nImage = new Bitmap(image);

            int[,] t = new int[image.Width, image.Height];

            double minL = 255;
            double maxSygma = 0;

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR < minL)
                    {
                        minL = pImage[i, j].SR;
                    }
                    int ni, nj;
                    int ki, kj;
                    ni = i - (int)(a / 2);
                    nj = j - (int)(a / 2);
                    ki = i + (int)(a / 2);
                    kj = j + (int)(a / 2);

                    double m = 0, mQ = 0;
                    int mCount = 0;

                    for (int i2 = ni; i2 < ki + 1; i2++)
                    {
                        for (int j2 = nj; j2 < kj + 1; j2++)
                        {
                            if (i2 >= 0 && i2 < nImage.Width)
                            {
                                if (j2 >= 0 && j2 < nImage.Height)
                                {
                                    m += pImage[i2, j2].SR;
                                    mQ += pImage[i2, j2].SR * pImage[i2, j2].SR;
                                    mCount++;
                                }
                            }
                        }
                    }

                    m /= mCount;
                    mQ /= mCount;

                    double d = 0;
                    d = mQ - (m * m);

                    double sygma = 0;
                    sygma = Math.Sqrt(d);

                    if (sygma > maxSygma)
                    {
                        maxSygma = sygma;
                    }
                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    int ni, nj;
                    int ki, kj;
                    ni = i - (int)(a / 2);
                    nj = j - (int)(a / 2);
                    ki = i + (int)(a / 2);
                    kj = j + (int)(a / 2);

                    double m = 0, mQ = 0;
                    int mCount = 0;

                    for (int i2 = ni; i2 < ki + 1; i2++)
                    {
                        for (int j2 = nj; j2 < kj + 1; j2++)
                        {
                            if (i2 >= 0 && i2 < nImage.Width)
                            {
                                if (j2 >= 0 && j2 < nImage.Height)
                                {
                                    m += pImage[i2, j2].SR;
                                    mQ += pImage[i2, j2].SR * pImage[i2, j2].SR;
                                    mCount++;
                                }
                            }
                        }
                    }

                    m /= mCount;
                    mQ /= mCount;

                    double d = 0;
                    d = mQ - (m * m);

                    double sygma = 0;
                    sygma = Math.Sqrt(d);

                    t[i, j] = (int)(0.5 * m + 0.5 * minL + 0.5 * (sygma / maxSygma * (m - minL)));

                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR <= t[i, j])
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public Bitmap BredlyRotMethod(Pixel[,] pImage, int a, double k)
        {
            Bitmap nImage = new Bitmap(image);

            int[,] imageSum = new int[image.Width, image.Height];

            int[,] areasSum = new int[image.Width, image.Height];

            int[,] sumCount = new int[image.Width, image.Height];

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    int s1, s2, s3;
                    if (i - 1 >= 0)
                    {
                        s1 = imageSum[i - 1, j];
                    }
                    else
                    {
                        s1 = 0;
                    }
                    if (j - 1 >= 0)
                    {
                        s2 = imageSum[i, j - 1];
                    }
                    else
                    {
                        s2 = 0;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        s3 = imageSum[i - 1, j - 1];
                    }
                    else
                    {
                        s3 = 0;
                    }

                    imageSum[i, j] = (int)pImage[i, j].SR + s1 + s2 - s3;
                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    int ni, nj;
                    int ki, kj;
                    ni = i - (int)(a / 2);
                    nj = j - (int)(a / 2);
                    ki = i + (int)(a / 2);
                    kj = j + (int)(a / 2);


                    for (int i2 = ni; i2 < ki + 1; i2++)
                    {
                        for (int j2 = nj; j2 < kj + 1; j2++)
                        {
                            if (i2 >= 0 && i2 < nImage.Width)
                            {
                                if (j2 >= 0 && j2 < nImage.Height)
                                {
                                    sumCount[i, j]++;
                                }
                            }
                        }
                    }

                    int s1, s2, s3, s4;

                    if (ki < nImage.Width && kj < nImage.Height) 
                    {
                        s1 = imageSum[ki, kj];
                    }
                    else
                    {
                        s1 = 0;
                    }

                    if (ni - 1 > 0 && nj - 1 > 0) 
                    {
                        s2 = imageSum[ni - 1, nj - 1];
                    }
                    else
                    {
                        s2 = 0;
                    }

                    if (ni - 1 > 0 && kj < nImage.Height)
                    {
                        s3 = imageSum[ni - 1, kj];
                    }
                    else
                    {
                        s3 = 0;
                    }

                    if (ki < nImage.Width && nj - 1 > 0)
                    {
                        s4 = imageSum[ki, nj - 1];
                    }
                    else
                    {
                        s4 = 0;
                    }

                    areasSum[i, j] = s1 + s2 - s3 - s4;
                }
            }

            for (int i = 0; i < nImage.Width; i++)
            {
                for (int j = 0; j < nImage.Height; j++)
                {
                    if (pImage[i, j].SR * sumCount[i, j] < areasSum[i, j] * (1 - k))
                    {
                        Color c = Color.FromArgb(0, 0, 0);
                        nImage.SetPixel(i, j, c);
                    }
                    else
                    {
                        Color c = Color.FromArgb(255, 255, 255);
                        nImage.SetPixel(i, j, c);
                    }
                }
            }

            return nImage;
        }

        public void MethodStart(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                    break;
                case 1:
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                    break;
                case 2:
                    textBox1.Visible = true;
                    textBox2.Visible = true;
                    textBox1.Text = "5";
                    textBox2.Text = "-0,2";
                    break;
                case 3:
                    textBox1.Visible = true;
                    textBox2.Visible = true;
                    textBox1.Text = "5";
                    textBox2.Text = "0,2";
                    break;
                case 4:
                    textBox1.Visible = true;
                    textBox2.Visible = false;
                    textBox1.Text = "5";
                    break;
                case 5:
                    textBox1.Visible = true;
                    textBox2.Visible = true;
                    textBox1.Text = "5";
                    textBox2.Text = "0,15";
                    break;
            }

        }
    }
}
