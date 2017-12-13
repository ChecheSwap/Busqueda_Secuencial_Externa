
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using searches.FILE_MANIPULATION;
using searches.ROUTINES;

namespace searches
{
    public partial class vista : Form
    {
        private string path = null;
        private string key = null;
        private Thread th1;
        private Stopwatch s1;
        private tiposdebusqueda tipobusqueda;
        private rutinas busquedas;
        private fileoperations files;


        private string[] source;



        public vista(string path, string key, tiposdebusqueda tipobusqueda)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;            
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;

            this.path = path;
            this.key = key;            
            this.s1 = new Stopwatch();


            this.tipobusqueda = tipobusqueda;
            this.busquedas = new rutinas();
            this.files = new fileoperations();
            this.lblkey.Text = key;
            this.source = this.files.getdataSplit(this.path);
            this.lblelementos.Text = this.source.Length.ToString() + " Elementos";


            switch (tipobusqueda)
            {
                case (tiposdebusqueda.ITERATIVA):
                    {
                        this.lblnombremetodo.Text = "ITERATIVA";
                        break;
                    }
                case (tiposdebusqueda.RECURSIVA):
                    {
                        this.lblnombremetodo.Text = "RECURSIVA";
                        break;
                    }
            }

            this.Text = "Resultados de Busqueda";
            
        }  

        private void vista_Load(object sender, EventArgs e)
        {
            this.th1 = new Thread(new ThreadStart(delegate ()
            {
                this.startsearch();
            }))
            { IsBackground = true};
            this.th1.Start();        
        }

        private void startsearch()
        {
            observer resultados = new observer();
            this.s1.Reset();

            switch (this.tipobusqueda)
            {
                case (tiposdebusqueda.ITERATIVA):
                    {                                                
                        this.s1.Start();                        

                        resultados = this.busquedas.secuencialiterativa(ref source, this.key, posicionArchivo.FIRST);
                        
                        this.s1.Stop();
                        
                        break;
                    }
                case (tiposdebusqueda.RECURSIVA):
                    {
                        this.s1.Start();

                        string[] source = this.files.getdataSplit(this.path);

                        resultados = this.busquedas.secuencialrecursive(ref source, this.key, posicionArchivo.FIRST);

                        this.s1.Stop();

                        break;
                    }
                default:
                    {
                        break;
                    }
            }


            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    this.lbltime.Text = this.s1.Elapsed.ToString();
                    this.lblincidencia.Text = (resultados.flag) ? "Si" : "No";
                    this.lblfrecuencia.Text = resultados.cantidad.ToString();
                }));
            }
        }
    }
}
