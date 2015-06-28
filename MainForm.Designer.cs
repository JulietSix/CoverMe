/* 
 * CoverMe! - Copyright (C) 2015 Juliet_Six
 * 
 * This file is part of "CoverMe!". "CoverMe!" is licensed under
 * the Microsoft Public License. You should have received a copy
 * of this license together with this file. Otherwise, see 
 * http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 */
 
namespace CoverMe
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.labelInfo = new System.Windows.Forms.Label();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// timerUpdate
			// 
			this.timerUpdate.Interval = 20;
			this.timerUpdate.Tick += new System.EventHandler(this.TimerUpdateTick);
			// 
			// labelInfo
			// 
			this.labelInfo.Location = new System.Drawing.Point(12, 9);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(322, 114);
			this.labelInfo.TabIndex = 0;
			this.labelInfo.Text = "Loading...";
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(346, 132);
			this.Controls.Add(this.labelInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CoverMe by J6";
			this.Shown += new System.EventHandler(this.MainFormShown);
			this.ResumeLayout(false);
		}
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.Label labelInfo;
		private System.Windows.Forms.Timer timerUpdate;
	}
}
