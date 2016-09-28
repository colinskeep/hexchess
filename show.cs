using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Threading;

namespace HexC
{
    public class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Form1";
        }

        public Form1()
        {
            InitializeComponent();
        }

        static public void StartMeUp()
        {
            System.Threading.Thread t = new Thread(Form1.YeahStart);
            t.Start();
        }

        [STAThread]
        public static void YeahStart(object parameter)
        {        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            m_form = new Form1();
            m_form.Size = new Size(500, 500);
            Application.Run(m_form);
        }

        protected override void OnShown(EventArgs e)
        {
            HexC.Program.HCMain();
        }

        const int SIZEY = 60;
        const int OFFSET = 200;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_pp == null)
                return;
            Graphics g = e.Graphics;

            for (int q = -5; q <= 5; q++)
                for (int r = -5; r <= 5; r++)
                {
                    int x = q * SIZEY;
                    int y = (r * SIZEY) / 2;
                    x = x + y;

                    string s = q.ToString() + "," + r.ToString();
                    bool bold = false;
                    foreach(var p in m_pp)
                    {
                        if (q == p.Location.Q)
                            if (r == p.Location.R)
                            {
                                s = p.Color.ToString().Substring(0, 1) + p.PieceType.ToString().Substring(0, 2);
                                bold = true;
                            }
                    }

                    BoardLocation bl = new BoardLocation(q, r);
                    if (bl.IsValidLocation())
                    {
                        g.DrawString(s, SystemFonts.DialogFont, bold ? SystemBrushes.ControlDarkDark : SystemBrushes.Highlight, (float)(x * .6) + OFFSET, y + OFFSET);
                    }
                }
        }

        static List<PlacedPiece> m_pp = null;
        static Form1 m_form = null;

        static public void ShowBoard(List<PlacedPiece> pp)
        {
            m_pp = pp;
            m_form.Refresh();
//            m_form.Update();
        }
    }
}