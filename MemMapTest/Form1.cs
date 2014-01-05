using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MemMapTest
{
	public partial class Form1 : Form
	{
		private MemoryMappedFile sharedFile = null;

		public Form1()
		{
			InitializeComponent();
		}

		private void btnRead_Click(object sender, EventArgs e)
		{
			// create the file if it doesn't exist
			if (sharedFile == null) sharedFile = MemoryMappedFile.CreateOrOpen("testmap", 1000, MemoryMappedFileAccess.ReadWrite);

			// process safe handling
			Mutex mutex = new Mutex(false, "testmapmutex");

			if (mutex.WaitOne()) {
				try {
					using (MemoryMappedViewStream stream = sharedFile.CreateViewStream()) {
						var textReader = new StreamReader(stream);
						txtResult.Text = textReader.ReadToEnd();
						textReader.Close();
					}
				}
				finally { mutex.ReleaseMutex(); }
			}
		}

		private void btnWrite_Click(object sender, EventArgs e)
		{
			// create the file if it doesn't exist
			if (sharedFile == null) sharedFile = MemoryMappedFile.CreateOrOpen("testmap", 1000, MemoryMappedFileAccess.ReadWrite);

			// process safe handling
			Mutex mutex = new Mutex(false, "testmapmutex");

			if (mutex.WaitOne()) {
				try {
					using (MemoryMappedViewStream stream = sharedFile.CreateViewStream()) {
						var writer = new StreamWriter(stream);
						writer.WriteLine(txtResult.Text);
						writer.Flush();
					}
				}
				finally { mutex.ReleaseMutex(); }
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (sharedFile != null) sharedFile.Dispose();
		}
	}
}
