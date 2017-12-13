using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using searches.FILE_MANIPULATION;
using searches.ROUTINES;

namespace searches
{
    public partial class central : Form
    {
        private fileoperations files;
        private rutinas metodos;

        private SaveFileDialog svd;
        private OpenFileDialog ofd;     
        private Int64 [] tarray;
        private Random random;
        private int limita, limitb, xlength;
        private bool flag;
        private Stopwatch watch;                
        private Thread thgenerar;

        private vista reporte;
        
        public central()
        {
            InitializeComponent();
            this.ocultar();
            this.Text = "Analisis de Rendimiento de Algoritmos de Busqueda";
            this.StartPosition = FormStartPosition.CenterScreen;            
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.rtb1.ReadOnly = true;

            this.nupdtotal.ValueChanged += (sender, args) => { int.TryParse(this.nupdtotal.Value.ToString(), out this.xlength); };
            this.nupd1.ValueChanged += (sender, args) => { int.TryParse(this.nupd1.Value.ToString(), out this.limita); };
            this.nupd2.ValueChanged += (sender, args) => { int.TryParse((this.nupd2.Value+1).ToString(), out this.limitb); };

            this.nupdtotal.KeyPress += (sender, args) => { if (args.KeyChar == (char)Keys.Enter) this.btngenerar(); };
            this.nupdtotal.Maximum = 9999999999999999999;
            this.nupdtotal.Text = "";
            this.nupd1.Minimum = -20;
            this.nupd2.Minimum = -20;
            this.nupd1.Maximum = 1000000;
            this.nupd2.Maximum = 1000000;
            this.nupd1.Value = -20;
            this.nupd2.Value = 20;
           
            
            this.random = new Random(System.DateTime.Today.Millisecond);
            this.watch = new Stopwatch();

            this.button1.Click += (sender, args) =>
            {
                this.btngenerar();
            };

            this.btnsave.Click += (sender, args) => { this.btnsaverut(); };

            this.btnsave2.Click += (sender, args) => { this.btnsaverut2(); };

            this.btnopenfile.Click += (sender, args) => { this.btnopenrut(); };

            this.btnopen2.Click += (sender, args) => { this.btnopenrut2(); };
            this.files = new fileoperations();

            this.svd = new SaveFileDialog() {

                Title = "Guardar Documento", Filter = ".txt (Texto)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
               // CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true                
            };

            this.ofd = new OpenFileDialog()
            {
                Title = "Abrir Documento", Filter = ".txt (Texto)|*.txt",
                InitialDirectory = @"C:\Users\VestigiumSenex\Desktop",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true
            };

            this.mostrarcandado();

            this.cbrec.Checked = true;

            this.txtkey.KeyPress += (sender, args) => { if(args.KeyChar == (char)Keys.Enter) this.btnbuscarrutina(); };
            this.btnbuscar.Click += (sender, args) => { this.btnbuscarrutina(); };
        }

        private void btnbuscarrutina()
        {
            if(this.txtkey.Text.Trim() == string.Empty)
            {
                msg.danger("Ingrese Elemento a buscar!");
                this.txtkey.Focus();
                return;
            }
            else
            {
                if (this.cbiter.Checked)
                {
                    this.reporte = new vista(this.txtrut.Text, this.txtkey.Text.Trim(), tiposdebusqueda.ITERATIVA);                    
                }
                else
                {
                    this.reporte = new vista(this.txtrut.Text, this.txtkey.Text.Trim(), tiposdebusqueda.RECURSIVA);
                }

                this.reporte.Show();
            }
        }
        private void btnsaverut()
        {
            if (this.tarray == null)
            {
                msg.danger("Genere numeros a Guardar.");
                return;
            }

            if (svd.ShowDialog() == DialogResult.OK)
            {
                files.make(ref this.tarray,svd.FileName, separations.COMMA);
            };
        }

        private void btnsaverut2()
        {
            if(rtbsec2.Text.Trim() == string.Empty)
            {
                msg.danger("Ingrese caracteres a guardar!");
                return;
            }


            if(this.svd.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                char [] send = this.rtbsec2.Text.ToArray();

                files.make(ref send, this.svd.FileName, separations.NONE);

                this.rtbsec2.Text = "";
            }

            
        }

        private void btnopenrut()
        {
            if(this.ofd.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                this.rtbsec2.Text+=files.getdata(this.ofd.FileName);
            }
        }

        private void btnopenrut2()
        {
            if (this.ofd.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                this.txtrut.Text = this.ofd.FileName;
                this.ocultarcandado();
            }
        }

        private void btncleartxtruta()
        {
            this.txtrut.Text = "";
            this.mostrarcandado();
        }

        private void mostrarcandado()
        {
            this.candado.Visible = true;
            this.panelsearch.Enabled = false;
        }

        private void ocultarcandado()
        {
            this.candado.Visible = false;
            this.panelsearch.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            this.ActiveControl = this.nupdtotal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.btngenerar();
        }

        private void btngenerar()
        {
            this.lbltime.Text = "Time of work = Unavailable";

            if (this.xlength == 0)
            {
                msg.error("Defina una longitud mayor a 0.");
                this.nupdtotal.Focus();
            }
            else
            {
                this.mostrar();                
                this.rtb1.Text = "";
                this.rtb1.Enabled = false;
               
                this.thgenerar = new Thread(new ThreadStart(delegate ()
                {

                    this.threadserie();

                }))
                { IsBackground = true};
                this.thgenerar.Start();
            }
        }

        public async void threadserie() //FUNCTION TO CREATE THE SUBTHREAD OF RANDOMNESS
        {
            
            this.watch.Start();
            bool f = await Task.Run(() => this.createserie(this.xlength));
            this.watch.Stop();

            this.lbltime.Invoke(new MethodInvoker(delegate () {
                this.lbltime.Text = "Time of work = " + this.watch.Elapsed.ToString("hh\\:mm\\:ss\\.ff");
            }));

            this.watch.Reset();
            this.ocultar();

            this.rtb1.Invoke(new MethodInvoker(delegate () {
                this.rtb1.Enabled = true;
            }));
            ;
               
        }

        public bool createserie(int x) //FUNCTION TO GENERATE THE RANDOM NUMBERS
        {
            if (this.flag)
            {
                this.flag = false;
            }
            try
            {                
                this.tarray = new Int64[(int)x];
                
                for (int a = 0; a < x; ++a)
                {
                    this.tarray[a] = this.random.Next(this.limita, this.limitb);

                    if (this.rtb1.InvokeRequired)
                    {
                        rtb1.Invoke(new MethodInvoker(delegate () {
                            this.rtb1.Text += (a + 1).ToString() + ")-> " + this.tarray[a].ToString() + "\n";
                        }));
                        
                    }
                    else
                    {
                        this.rtb1.Text += (a + 1).ToString() + ")-> " + this.tarray[a].ToString() + "\n";
                    }
                    
                }

                this.flag = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return this.flag;
        }
        private void clearvals() //FUNCTION TO CLEAR VALUES OF INTEREST
        {
            this.rtb1.Text = "";
            this.nupdtotal.Value = 0;
            this.nupdtotal.Text = "";            
        }

        public void mostrar()
        {
            if (this.paneltop.InvokeRequired)
            {
                paneltop.Invoke(new MethodInvoker(delegate () {
                    this.paneltop.Visible = true;
                }));

                return;
            }
            
            this.paneltop.Visible = true;            
        }

        public void ocultar()
        {
            if(paneltop.InvokeRequired)
            {
                paneltop.Invoke(new MethodInvoker(delegate () {
                    this.paneltop.Visible = false;
                }));

                return;
            }

            this.paneltop.Visible = false;
        }

        private void rendimientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            msg.ok("-Proyecto: Busqueda Lineal \n\n-Alumno: Jesús José Navarrete Baca\n\n-Profesor: ISCH Carlos Eduardo Garcia Vargas\n\n-Materia:Estructura de datos II", "Información");
        }

        private void cb1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cb2_CheckedChanged(object sender, EventArgs e)
        {
 
        }

        private void cb3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbmulti_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void startCalc()
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {}

        private void btnsave_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.btncleartxtruta();
        }

        private void cbrec_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbrec.Checked)
            {
                this.cbiter.Checked = false;
            }
            else
            {
                this.cbiter.Checked = true;
            }
        }

        private void cbiter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbiter.Checked)
            {
                this.cbrec.Checked = false;
            }else
            {
                this.cbrec.Checked = true;
            }
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {

        }

        private void btnopen2_Click(object sender, EventArgs e)
        {

        }

        private void cbnuevo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

    }
}
