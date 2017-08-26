using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HaloAnimationEditor
{
	/// <summary>
	/// Summary description for TagReference.
	/// </summary>
	public class MAETagReference : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label name;
		private System.Windows.Forms.TextBox field;
		private System.Windows.Forms.Button browse;
		public System.Windows.Forms.OpenFileDialog SelectTag;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ComboBox tag;
		private string info;

		public string ControlName
		{
			get { return this.name.Text; }
			set { this.name.Text = value; }
		}

		public string Field
		{
			get { return this.field.Text; }
			set { this.field.Text = value; }
		}

		public string TagType
		{
			get { return this.tag.Text; }
			set { this.tag.Text = value; }
		}

		public bool NoTagType
		{
			get { return this.tag.Enabled; }
			set { this.tag.Enabled = value; }
		}

		public string FilterText
		{
			get { return this.SelectTag.Filter; }
			set { this.SelectTag.Filter = value; }
		}

		public string Info
		{
			get { return this.info; }
			set 
			{ 
				this.info = value;
				this.SetTips(value); 
			}
		}

		public void SetTips(string tips)
		{
			toolTip.SetToolTip(this, tips);
			toolTip.SetToolTip(this.name, tips);
			toolTip.SetToolTip(this.field, tips);
			toolTip.SetToolTip(this.tag, tips);
		}

		public MAETagReference()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.Field = "";
			this.Info = "";

			RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft Games\\Halo HEK");

			this.SelectTag.InitialDirectory = (string)rk.GetValue("EXE Path") + "\\tags\\";
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.name = new System.Windows.Forms.Label();
			this.field = new System.Windows.Forms.TextBox();
			this.browse = new System.Windows.Forms.Button();
			this.SelectTag = new System.Windows.Forms.OpenFileDialog();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tag = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// name
			// 
			this.name.Location = new System.Drawing.Point(0, 0);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(160, 24);
			this.name.TabIndex = 0;
			this.name.Text = "tag reference";
			this.name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// field
			// 
			this.field.Location = new System.Drawing.Point(312, 0);
			this.field.Name = "field";
			this.field.Size = new System.Drawing.Size(232, 20);
			this.field.TabIndex = 2;
			this.field.Text = "";
			// 
			// browse
			// 
			this.browse.Location = new System.Drawing.Point(544, 0);
			this.browse.Name = "browse";
			this.browse.Size = new System.Drawing.Size(24, 24);
			this.browse.TabIndex = 3;
			this.browse.Text = "...";
			this.toolTip.SetToolTip(this.browse, "Click to open a file");
			this.browse.Click += new System.EventHandler(this.OnBrowse);
			// 
			// SelectTag
			// 
			this.SelectTag.InitialDirectory = "tags\\";
			this.SelectTag.Title = "Tag Reference";
			// 
			// tag
			// 
			this.tag.Location = new System.Drawing.Point(168, 0);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(144, 21);
			this.tag.TabIndex = 4;
			this.tag.Text = "model_animations";
			// 
			// MAETagReference
			// 
			this.Controls.Add(this.tag);
			this.Controls.Add(this.browse);
			this.Controls.Add(this.field);
			this.Controls.Add(this.name);
			this.Name = "MAETagReference";
			this.Size = new System.Drawing.Size(576, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnBrowse(object sender, System.EventArgs e)
		{
			if(SelectTag.ShowDialog() == DialogResult.OK)
				this.Field = SelectTag.FileName;
		}
	}
}
