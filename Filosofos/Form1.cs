using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Filosofos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {

            l.Add(food);
            l.Add(food2);
            l.Add(food3);
            l.Add(food4);
            l.Add(food5);
            points.Add(new Point(150, 5));
            points.Add(new Point(55, 85));
            points.Add(new Point(240, 85));
            points.Add(new Point(85, 190));
            points.Add(new Point(205, 190));
            var i = 0;
            foreach (var x in l)
            {
                x.Parent = background_pictureBox;
                x.Location = points[i];
                i++;
                x.BackColor = Color.Transparent;
                x.BringToFront();
            }

            for (var l = 0; l < 5; l++)
            {
                forks.Add(new Mutex());
                waitTime.Add(new System.Timers.Timer(600));
                waiting.Add(new bool());
            }

            Filosofo t = new Filosofo(fil1, l[0], 0, "", 0, 1, actionLbl,statusLbl);
            filosofos.Add(t);
            t = new Filosofo(fil2, l[1], 0, "", 1, 3, action2Lbl,status2Lbl);
            filosofos.Add(t);
            t = new Filosofo(fil3, l[2], 0, "", 2, 0, action3Lbl,status3Lbl);
            filosofos.Add(t);
            t = new Filosofo(fil4, l[3], 0, "", 3, 4, action4Lbl,status4Lbl);
            filosofos.Add(t);
            t = new Filosofo(fil5, l[4], 0, "", 4, 2, action5Lbl,status5Lbl);
            filosofos.Add(t);

            for (int r = 0; r < 5; r++)
            {
                Filosofo u = filosofos[r];
                u.dish.Visible = false;
                u.count = 0;
                u.image.Image = thinkingpic.Image;
                u.act.Text = "N.Comidas: " + filosofos[r].count.ToString();
                filosofos[r] = u;
            }
            start();
        }
        bool endWork;
        struct Filosofo{
            public PictureBox image;
            public PictureBox dish;
            public int count;
            public string action;
            public int firstM;
            public int secondM;
            public Label act;
            public Label status;
            public Filosofo(PictureBox i, PictureBox x, int y, string z,int fm,int sm,Label a,Label st) : this()
            {
                this.image = i;
                this.dish = x;
                this.count = y;
                this.action = z;
                this.firstM = fm;
                this.secondM = sm;
                this.act = a;
                this.status = st;
            }
        };
        List<Mutex> forks = new List<Mutex>();
        List<System.Timers.Timer> waitTime = new List<System.Timers.Timer>();
        List<bool> waiting = new List<bool>();
        List<(int, int)> mutexforks=new List<(int, int)>();
        List<Point> points = new List<Point>();
        List<PictureBox> l = new List<PictureBox>();
        List<Filosofo> filosofos = new List<Filosofo>();
        Bitmap fork;
        Bitmap table;

        
        private void start()
        {
            endWork = false;
            for (int i = 0; i < 5; i++)
            {
                int temp = i;
                Thread aux = new Thread(new ThreadStart(() => Procedure(temp)));
                aux.Start();
            }
           // startButton.Visible = false;
        }

        private void Procedure(int fi)
        {
            bool ate;
            waitTime[fi].AutoReset = false;
            int randNumber;
            while (!endWork)
            {
                randNumber = new Random().Next(2000, 5000);
                waitTime[fi].Interval = randNumber;
                waitTime[fi].Start();
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    filosofos[fi].dish.Visible = false;
                    filosofos[fi].status.Text = "Pensando";
                    
                }));
                while (waitTime[fi].Enabled) ;
                randNumber = new Random().Next(500, 1500);
                ate = false;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    filosofos[fi].dish.Visible = false;
                    filosofos[fi].status.Text = "Intentando Comer";
                    
                }));
                while (!ate && !endWork)
                {

                    if (forks[filosofos[fi].firstM].WaitOne(randNumber))
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            filosofos[fi].dish.Visible = false;
                        }));
                        if (forks[filosofos[fi].secondM].WaitOne(randNumber))
                        {
                            randNumber = new Random().Next(3000, 5000);
                            waitTime[fi].Interval = randNumber;
                            waitTime[fi].Start();
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                filosofos[fi].dish.Visible = true;
                                filosofos[fi].image.Image=eatingpic.Image;
                                filosofos[fi].status.Text = "Comiendo";
                            }));
                            while (waitTime[fi].Enabled) ;
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                Filosofo u = filosofos[fi];
                                u.dish.Visible = false;
                                u.count++;
                                u.image.Image = thinkingpic.Image;
                                u.act.Text = "N.Comidas: "+u.count.ToString();
                                filosofos[fi] = u;
                            }));
                            forks[filosofos[fi].firstM].ReleaseMutex();
                            forks[filosofos[fi].secondM].ReleaseMutex();
                            ate = true;
                            StopProgram();
                        }
                        else
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                filosofos[fi].dish.Visible = false;
                            }));
                            forks[filosofos[fi].firstM].ReleaseMutex();
                        }
                    }


                }
            }
            this.BeginInvoke(new MethodInvoker(delegate
            {
                filosofos[fi].status.Text = "Durmiendo";
                filosofos[fi].dish.Visible = false;
            }));
        }

        private void StopProgram()
        {
            for (int i = 0; i < 5; i++)
            {
                if (filosofos[i].count < 5)
                {
                    return;
                }
            }
            endWork = true;



        }

        public void FinishWait(object sender, ElapsedEventArgs e)
        {


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
