namespace voice_assistant
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.inputField = new System.Windows.Forms.RichTextBox();
			this.output = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(222, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "ROBO (Base)";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// inputField
			// 
			this.inputField.Location = new System.Drawing.Point(12, 50);
			this.inputField.Name = "inputField";
			this.inputField.Size = new System.Drawing.Size(256, 112);
			this.inputField.TabIndex = 2;
			this.inputField.Text = "";
			// 
			// output
			// 
			this.output.Location = new System.Drawing.Point(301, 50);
			this.output.Name = "output";
			this.output.Size = new System.Drawing.Size(256, 112);
			this.output.TabIndex = 3;
			this.output.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(569, 174);
			this.Controls.Add(this.output);
			this.Controls.Add(this.inputField);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox inputField;
		private System.Windows.Forms.RichTextBox output;
	}
}

