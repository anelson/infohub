using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Spring.Context;
using com.db4o;
using com.db4o.config;
using com.db4o.ext;
using com.db4o.query;

using InfoHub.Common;


namespace InfoHub.DataStore.Db4oBrowser
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Db4oBrowser : System.Windows.Forms.Form
	{
		ExtObjectContainer _store;
		private InfoHub.DataStore.Db4oBrowser.Db4oTree _db4oTree;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Db4oBrowser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public Db4oBrowser(ExtObjectContainer store) : this() {
			_store = store;

			_db4oTree.Store = _store;
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
			this._db4oTree = new InfoHub.DataStore.Db4oBrowser.Db4oTree();
			this.SuspendLayout();
			// 
			// _db4oTree
			// 
			this._db4oTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._db4oTree.ImageIndex = -1;
			this._db4oTree.Location = new System.Drawing.Point(8, 32);
			this._db4oTree.Name = "_db4oTree";
			this._db4oTree.SelectedImageIndex = -1;
			this._db4oTree.Size = new System.Drawing.Size(832, 416);
			this._db4oTree.TabIndex = 0;
			// 
			// Db4oBrowser
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(848, 454);
			this.Controls.Add(this._db4oTree);
			this.Name = "Db4oBrowser";
			this.Text = "db4o Repository Browser";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			//Choose a data file to open
			ExtObjectContainer store = null;
			do {
				using (OpenFileDialog dlg = new OpenFileDialog()) {
					dlg.CheckFileExists = true;
					dlg.CheckPathExists = true;
					dlg.DefaultExt = "*";
					dlg.Multiselect = false;

					if (dlg.ShowDialog() != DialogResult.OK) {
						//User canceled
						return;
					}

					//Attempt to open the file
					try {
						store = Db4o.openFile(dlg.FileName).ext();
					} catch (Exception e) {
						MessageBox.Show(
							null,
							"Error opening database",
							String.Format("Error opening database '{0}': {1}\r\n\r\n{2}",
								dlg.FileName,
								e.Message,
								e.ToString()),
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
				}
			} while (store == null);

			//Opened a store.  Load it in the form
			Application.Run(new Db4oBrowser(store));

			store.close();
		}
	}
}
