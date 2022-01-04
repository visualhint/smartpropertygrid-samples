using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using VisualHint.SmartPropertyGrid;
using System.Globalization;
using System.Threading;

namespace WindowsApplication
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
        private MyPropertyGrid myPropertyGrid1;
        private MetaPropertyGrid metaPropertyGrid1;
        private TextBox leftText;
        private TextBox rightText;
        private Panel panel1;
        private Panel panel2;
		private IContainer components = null;

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
			this.leftText = new System.Windows.Forms.TextBox();
			this.rightText = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.metaPropertyGrid1 = new WindowsApplication.MetaPropertyGrid();
			this.myPropertyGrid1 = new WindowsApplication.MyPropertyGrid();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.metaPropertyGrid1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.myPropertyGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// leftText
			// 
			this.leftText.BackColor = System.Drawing.Color.LightBlue;
			this.leftText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.leftText.Location = new System.Drawing.Point(9, 6);
			this.leftText.Multiline = true;
			this.leftText.Name = "leftText";
			this.leftText.Size = new System.Drawing.Size(256, 129);
			this.leftText.TabIndex = 8;
			this.leftText.TabStop = false;
			this.leftText.Text = @"The left PropertyGrid is an instance of Smart PropertyGrid. It contains a selected set of the right PropertyGrid's properties which is also an instance of Smart PropertyGrid. It will also show the properties of the selected node in the right grid.

For many properties below, F1 can be typed to get more informations.";
			// 
			// rightText
			// 
			this.rightText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rightText.BackColor = System.Drawing.Color.Ivory;
			this.rightText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rightText.Location = new System.Drawing.Point(8, 6);
			this.rightText.Multiline = true;
			this.rightText.Name = "rightText";
			this.rightText.Size = new System.Drawing.Size(292, 128);
			this.rightText.TabIndex = 9;
			this.rightText.TabStop = false;
			this.rightText.Text = @"This grid displays a variety of inplace controls, types and properties so that you get an idea of the flexibility of this grid. It has not been linked to a target instance. Instead, AppendProperty like methods have been used but the same effects could be obtained by using SelectedObject, attributes and callback methods.

In this sample, both grids use the Skybound VisualTips for error tips.";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.LightBlue;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.leftText);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(272, 141);
			this.panel1.TabIndex = 10;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.Ivory;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.rightText);
			this.panel2.Location = new System.Drawing.Point(299, 12);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(308, 141);
			this.panel2.TabIndex = 11;
			// 
			// metaPropertyGrid1
			// 
			this.metaPropertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.metaPropertyGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.metaPropertyGrid1.CommentsHeight = 50;
			this.metaPropertyGrid1.CommentsVisibility = true;
			this.metaPropertyGrid1.LabelColumnWidthRatio = 0.43939393939393939;
			this.metaPropertyGrid1.LabelColumnWidthRatio2 = 0;
			this.metaPropertyGrid1.Location = new System.Drawing.Point(12, 164);
			this.metaPropertyGrid1.Name = "metaPropertyGrid1";
			this.metaPropertyGrid1.NeedsControlKeyForHyperlinks = true;
			this.metaPropertyGrid1.PropertyLabelBackColor = System.Drawing.Color.FromArgb(((System.Byte)(248)), ((System.Byte)(247)), ((System.Byte)(246)));
			this.metaPropertyGrid1.PropertyValueBackColor = System.Drawing.SystemColors.Window;
			this.metaPropertyGrid1.PropInPlaceTextBoxType = null;
			this.metaPropertyGrid1.RightToLeft2 = false;
			this.metaPropertyGrid1.Size = new System.Drawing.Size(272, 431);
			this.metaPropertyGrid1.TabIndex = 1;
			this.metaPropertyGrid1.Text = "metaPropertyGrid1";
			// 
			// myPropertyGrid1
			// 
			this.myPropertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.myPropertyGrid1.BackColor = System.Drawing.SystemColors.Control;
			this.myPropertyGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.myPropertyGrid1.CommentsHeight = 50;
			this.myPropertyGrid1.CommentsVisibility = true;
			this.myPropertyGrid1.LabelColumnWidthRatio = 0.44;
			this.myPropertyGrid1.Location = new System.Drawing.Point(299, 164);
			this.myPropertyGrid1.Name = "myPropertyGrid1";
			this.myPropertyGrid1.NeedsControlKeyForHyperlinks = true;
			this.myPropertyGrid1.PropInPlaceTextBoxType = null;
			this.myPropertyGrid1.ReadOnlyForeColor = System.Drawing.Color.SteelBlue;
			this.myPropertyGrid1.ReadOnlyVisual = VisualHint.SmartPropertyGrid.PropertyGrid.ReadOnlyVisuals.ReadOnlyFeel;
			this.myPropertyGrid1.Size = new System.Drawing.Size(308, 429);
			this.myPropertyGrid1.TabIndex = 2;
			this.myPropertyGrid1.Text = "myPropertyGrid1";
			this.myPropertyGrid1.ToolbarVisibility = true;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(617, 605);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.metaPropertyGrid1);
			this.Controls.Add(this.myPropertyGrid1);
			this.MinimumSize = new System.Drawing.Size(450, 600);
			this.Name = "Form1";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.metaPropertyGrid1)).EndInit();
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
            Application.Run(new Form1());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myPropertyGrid1.LazyLoading = true;

            metaPropertyGrid1.Populate(myPropertyGrid1);
            myPropertyGrid1.Populate();

            myPropertyGrid1.HyperLinkPropertyClicked += new
                VisualHint.SmartPropertyGrid.PropertyGrid.HyperLinkPropertyClickedEventHandler(myPropertyGrid1_HyperLinkPropertyClicked);
            
            // Comment out for a filter textbox
/*            ToolStripLabel lbl = new ToolStripLabel("Filter:");
            ToolStripTextBox txt = new ToolStripTextBox("filter");
            txt.Name = "spg_filterTextbox";
            txt.BorderStyle = BorderStyle.None;
            myPropertyGrid1.Toolbar.Items.Add(lbl);
            myPropertyGrid1.Toolbar.Items.Add(txt);
            txt.TextChanged += new EventHandler(txt_TextChanged);
            myPropertyGrid1.DisplayModeChanged += new VisualHint.SmartPropertyGrid.PropertyGrid.DisplayModeChangedEventHandler(myPropertyGrid1_DisplayModeChanged);*/
        }

        // Comment out for a filter textbox
/*        void myPropertyGrid1_DisplayModeChanged(object sender, EventArgs e)
        {
            // Comment out for a filter textbox
            string filter = ((ToolStripTextBox)myPropertyGrid1.Toolbar.Items["spg_filterTextbox"]).Text;
            if ((filter.Length > 0) && !_insideFilterProperties)
                FilterProperties(filter);
        }

        private bool _insideFilterProperties = false;

        void FilterProperties(string filter)
        {
            _insideFilterProperties = true;

            PropertyEnumerator selectedEnum = myPropertyGrid1.SelectedPropertyEnumerator;

            myPropertyGrid1.BeginUpdate();

            VisualHint.SmartPropertyGrid.PropertyGrid.DisplayModes dispMode = myPropertyGrid1.DisplayMode;
            myPropertyGrid1.DisplayMode = VisualHint.SmartPropertyGrid.PropertyGrid.DisplayModes.Categorized;

            PropertyEnumerator propEnum = myPropertyGrid1.FirstProperty;
            while (propEnum != propEnum.RightBound)
            {
                if ((propEnum.Property.Value != null) || (propEnum.Property is PropertyHyperLink))
                {
                    if (propEnum.Property.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        PropertyEnumerator parentEnum = propEnum.GetDeepEnumerator();
                        while (parentEnum != parentEnum.RightBound)
                        {
                            if (!parentEnum.Property.Visible)
                                myPropertyGrid1.ShowProperty(parentEnum, true);
                            parentEnum.MoveParent();
                        }
                    }
                    else
                    {
                        myPropertyGrid1.ShowProperty(propEnum, false);
                        PropertyEnumerator parentEnum = propEnum.GetDeepEnumerator();
                        parentEnum.MoveParent();
                        while (parentEnum != parentEnum.RightBound)
                        {
                            if (parentEnum.Property.Visible && !myPropertyGrid1.HasOneVisibleChild(parentEnum))
                                myPropertyGrid1.ShowProperty(parentEnum, false);
                            parentEnum.MoveParent();
                        }
                    }
                }

                propEnum.MoveNext();
            }

            if ((selectedEnum.Property != null) && selectedEnum.Property.Visible && !selectedEnum.Property.Selected)
                myPropertyGrid1.SelectProperty(selectedEnum);

            myPropertyGrid1.DisplayMode = dispMode;
            myPropertyGrid1.EndUpdate();

            _insideFilterProperties = false;
        }

        void txt_TextChanged(object sender, EventArgs e)
        {
            ToolStripTextBox txt = (ToolStripTextBox)sender;
            FilterProperties(txt.Text);
        }*/

        void myPropertyGrid1_HyperLinkPropertyClicked(object sender, PropertyHyperLinkClickedEventArgs e)
        {
            if (metaPropertyGrid1.Enabled)
            {
                metaPropertyGrid1.DisableMode = VisualHint.SmartPropertyGrid.PropertyGrid.DisableModes.Simple;
                metaPropertyGrid1.Enabled = false;
                e.PropertyEnum.Property.DisplayName = "Enable left grid";
            }
            else
            {
                metaPropertyGrid1.Enabled = true;
                e.PropertyEnum.Property.DisplayName = "Disable left grid";
            }
        }
    }
}
