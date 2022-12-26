using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Solve_Captcha
{
	// Token: 0x02000002 RID: 2
	public class Form2 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Form2()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002070 File Offset: 0x00000270
		private void Form2_Load(object sender, EventArgs e)
		{
			base.Hide();
			Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, ImageFormat.Bmp);
				this.pictureBox1.Size = new Size(base.Width, base.Height);
				this.pictureBox1.Image = Image.FromStream(memoryStream);
			}
			base.Show();
			this.Cursor = Cursors.Cross;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000213C File Offset: 0x0000033C
		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			bool flag = this.pictureBox1.Image == null;
			if (!flag)
			{
				bool flag2 = this.start;
				if (flag2)
				{
					this.pictureBox1.Refresh();
					this.selectWidth = e.X - this.selectX;
					this.selectHeight = e.Y - this.selectY;
					this.pictureBox1.CreateGraphics().DrawRectangle(this.selectPen, this.selectX, this.selectY, this.selectWidth, this.selectHeight);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021CC File Offset: 0x000003CC
		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			bool flag = !this.start;
			if (flag)
			{
				bool flag2 = e.Button == MouseButtons.Left;
				if (flag2)
				{
					this.selectX = e.X;
					this.selectY = e.Y;
					this.selectPen = new Pen(Color.Red, 1f);
					this.selectPen.DashStyle = DashStyle.DashDotDot;
				}
				this.pictureBox1.Refresh();
				this.start = true;
			}
			else
			{
				bool flag3 = this.pictureBox1.Image == null;
				if (!flag3)
				{
					bool flag4 = e.Button == MouseButtons.Left;
					if (flag4)
					{
						this.pictureBox1.Refresh();
						this.selectWidth = e.X - this.selectX;
						this.selectHeight = e.Y - this.selectY;
						this.pictureBox1.CreateGraphics().DrawRectangle(this.selectPen, this.selectX, this.selectY, this.selectWidth, this.selectHeight);
					}
					this.start = false;
					this.SaveToClipboard();
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000022E9 File Offset: 0x000004E9
		private void SaveToClipboard()
		{
			base.Close();
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000006 RID: 6 RVA: 0x000022F4 File Offset: 0x000004F4
		// (remove) Token: 0x06000007 RID: 7 RVA: 0x0000232C File Offset: 0x0000052C
		public event Form2.RequestClose_de RequestClose;

		// Token: 0x06000008 RID: 8 RVA: 0x00002361 File Offset: 0x00000561
		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.RequestClose();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002370 File Offset: 0x00000570
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023A8 File Offset: 0x000005A8
		private void InitializeComponent()
		{
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Location = new Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(335, 312);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += this.pictureBox1_MouseDown;
			this.pictureBox1.MouseMove += this.pictureBox1_MouseMove;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(603, 435);
			base.Controls.Add(this.pictureBox1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "Form2";
			base.StartPosition = FormStartPosition.Manual;
			this.Text = "Form2";
			base.WindowState = FormWindowState.Maximized;
			base.FormClosing += this.Form2_FormClosing;
			base.Load += this.Form2_Load;
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}

		// Token: 0x04000001 RID: 1
		public int selectX;

		// Token: 0x04000002 RID: 2
		public int selectY;

		// Token: 0x04000003 RID: 3
		public int selectWidth;

		// Token: 0x04000004 RID: 4
		public int selectHeight;

		// Token: 0x04000005 RID: 5
		public Pen selectPen;

		// Token: 0x04000006 RID: 6
		private bool start = false;

		// Token: 0x04000008 RID: 8
		private IContainer components = null;

		// Token: 0x04000009 RID: 9
		private PictureBox pictureBox1;

		// Token: 0x02000008 RID: 8
		// (Invoke) Token: 0x06000035 RID: 53
		public delegate void RequestClose_de();
	}
}

