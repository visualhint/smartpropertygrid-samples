using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace QuickStart
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        private MyPropertyGrid myPropertyGrid1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.myPropertyGrid1 = new QuickStart.MyPropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.myPropertyGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// myPropertyGrid1
			// 
			this.myPropertyGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.myPropertyGrid1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.myPropertyGrid1.FormBackColor = System.Drawing.Color.Empty;
			this.myPropertyGrid1.Location = new System.Drawing.Point(12, 12);
			this.myPropertyGrid1.Name = "myPropertyGrid1";
			this.myPropertyGrid1.NeedsControlKeyForHyperlinks = true;
			this.myPropertyGrid1.Opacity = 0;
			this.myPropertyGrid1.Size = new System.Drawing.Size(268, 249);
			this.myPropertyGrid1.TabIndex = 0;
			this.myPropertyGrid1.Text = "myPropertyGrid1";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.myPropertyGrid1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.myPropertyGrid1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
#if !_DOTNET2
			Application.DoEvents(); // fixes a bug in .Net 1.1 with VisualStyles ??!!
#endif
			try
			{
				Application.Run(new Form1());
			}
			catch (LicenseException)
			{
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			myPropertyGrid1.Initialize();
		}
	}
}
