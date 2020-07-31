namespace ImageRecogniser
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
			this.cutToolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.questionBtn = new System.Windows.Forms.ToolStripButton();
			this.numericAnswerBtn = new System.Windows.Forms.ToolStripButton();
			this.variant1Btn = new System.Windows.Forms.ToolStripButton();
			this.variant2Btn = new System.Windows.Forms.ToolStripButton();
			this.variant3Btn = new System.Windows.Forms.ToolStripButton();
			this.variant4Btn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.recogniseBtn = new System.Windows.Forms.ToolStripButton();
			this.recogniseAllBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.processIdTextBox = new System.Windows.Forms.ToolStripTextBox();
			this.screenshootBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.analyzeBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.testBtn = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.testProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1271, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// файлToolStripMenuItem
			// 
			this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьToolStripMenuItem});
			this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
			this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.файлToolStripMenuItem.Text = "Файл";
			// 
			// открытьToolStripMenuItem
			// 
			this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
			this.открытьToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.открытьToolStripMenuItem.Text = "Открыть";
			this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.cutToolStripButton2,
            this.toolStripSeparator2,
            this.questionBtn,
            this.numericAnswerBtn,
            this.variant1Btn,
            this.variant2Btn,
            this.variant3Btn,
            this.variant4Btn,
            this.toolStripSeparator1,
            this.recogniseBtn,
            this.recogniseAllBtn,
            this.toolStripSeparator3,
            this.toolStripLabel2,
            this.processIdTextBox,
            this.screenshootBtn,
            this.toolStripSeparator4,
            this.analyzeBtn,
            this.toolStripSeparator5,
            this.testBtn,
            this.testProgressBar});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1271, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(53, 22);
			this.toolStripLabel1.Text = "Формат:";
			// 
			// toolStripComboBox1
			// 
			this.toolStripComboBox1.Items.AddRange(new object[] {
            "Тестовый",
            "Циферный"});
			this.toolStripComboBox1.Name = "toolStripComboBox1";
			this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
			this.toolStripComboBox1.Text = "Тестовый";
			this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
			// 
			// cutToolStripButton2
			// 
			this.cutToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.cutToolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton2.Image")));
			this.cutToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cutToolStripButton2.Name = "cutToolStripButton2";
			this.cutToolStripButton2.Size = new System.Drawing.Size(61, 22);
			this.cutToolStripButton2.Text = "Нарезать";
			this.cutToolStripButton2.Click += new System.EventHandler(this.cutToolStripButton2_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// questionBtn
			// 
			this.questionBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.questionBtn.Image = ((System.Drawing.Image)(resources.GetObject("questionBtn.Image")));
			this.questionBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.questionBtn.Name = "questionBtn";
			this.questionBtn.Size = new System.Drawing.Size(52, 22);
			this.questionBtn.Text = "Вопрос";
			this.questionBtn.Click += new System.EventHandler(this.questionBtn_Click);
			// 
			// numericAnswerBtn
			// 
			this.numericAnswerBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.numericAnswerBtn.Image = ((System.Drawing.Image)(resources.GetObject("numericAnswerBtn.Image")));
			this.numericAnswerBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.numericAnswerBtn.Name = "numericAnswerBtn";
			this.numericAnswerBtn.Size = new System.Drawing.Size(49, 22);
			this.numericAnswerBtn.Text = "Цифра";
			this.numericAnswerBtn.Click += new System.EventHandler(this.numericAnswerBtn_Click);
			// 
			// variant1Btn
			// 
			this.variant1Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.variant1Btn.Image = ((System.Drawing.Image)(resources.GetObject("variant1Btn.Image")));
			this.variant1Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.variant1Btn.Name = "variant1Btn";
			this.variant1Btn.Size = new System.Drawing.Size(65, 22);
			this.variant1Btn.Text = "Вариант 1";
			this.variant1Btn.Click += new System.EventHandler(this.variant1Btn_Click);
			// 
			// variant2Btn
			// 
			this.variant2Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.variant2Btn.Image = ((System.Drawing.Image)(resources.GetObject("variant2Btn.Image")));
			this.variant2Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.variant2Btn.Name = "variant2Btn";
			this.variant2Btn.Size = new System.Drawing.Size(65, 22);
			this.variant2Btn.Text = "Вариант 2";
			this.variant2Btn.Click += new System.EventHandler(this.variant2Btn_Click);
			// 
			// variant3Btn
			// 
			this.variant3Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.variant3Btn.Image = ((System.Drawing.Image)(resources.GetObject("variant3Btn.Image")));
			this.variant3Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.variant3Btn.Name = "variant3Btn";
			this.variant3Btn.Size = new System.Drawing.Size(65, 22);
			this.variant3Btn.Text = "Вариант 3";
			this.variant3Btn.Click += new System.EventHandler(this.variant3Btn_Click);
			// 
			// variant4Btn
			// 
			this.variant4Btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.variant4Btn.Image = ((System.Drawing.Image)(resources.GetObject("variant4Btn.Image")));
			this.variant4Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.variant4Btn.Name = "variant4Btn";
			this.variant4Btn.Size = new System.Drawing.Size(65, 22);
			this.variant4Btn.Text = "Вариант 4";
			this.variant4Btn.Click += new System.EventHandler(this.variant4Btn_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// recogniseBtn
			// 
			this.recogniseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.recogniseBtn.Image = ((System.Drawing.Image)(resources.GetObject("recogniseBtn.Image")));
			this.recogniseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.recogniseBtn.Name = "recogniseBtn";
			this.recogniseBtn.Size = new System.Drawing.Size(73, 22);
			this.recogniseBtn.Text = "Распознать";
			this.recogniseBtn.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// recogniseAllBtn
			// 
			this.recogniseAllBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.recogniseAllBtn.Image = ((System.Drawing.Image)(resources.GetObject("recogniseAllBtn.Image")));
			this.recogniseAllBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.recogniseAllBtn.Name = "recogniseAllBtn";
			this.recogniseAllBtn.Size = new System.Drawing.Size(94, 22);
			this.recogniseAllBtn.Text = "Распознать все";
			this.recogniseAllBtn.Click += new System.EventHandler(this.recogniseAllBtn_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(82, 22);
			this.toolStripLabel2.Text = "ИД процесса:";
			// 
			// processIdTextBox
			// 
			this.processIdTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.processIdTextBox.Name = "processIdTextBox";
			this.processIdTextBox.Size = new System.Drawing.Size(100, 25);
			this.processIdTextBox.Text = "8580";
			this.processIdTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.processIdTextBox_KeyPress);
			// 
			// screenshootBtn
			// 
			this.screenshootBtn.Image = ((System.Drawing.Image)(resources.GetObject("screenshootBtn.Image")));
			this.screenshootBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.screenshootBtn.Name = "screenshootBtn";
			this.screenshootBtn.Size = new System.Drawing.Size(78, 22);
			this.screenshootBtn.Text = "Скришот";
			this.screenshootBtn.Click += new System.EventHandler(this.screenshootBtn_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// analyzeBtn
			// 
			this.analyzeBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.analyzeBtn.Image = ((System.Drawing.Image)(resources.GetObject("analyzeBtn.Image")));
			this.analyzeBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.analyzeBtn.Name = "analyzeBtn";
			this.analyzeBtn.Size = new System.Drawing.Size(51, 22);
			this.analyzeBtn.Text = "Анализ";
			this.analyzeBtn.Click += new System.EventHandler(this.analyzeBtn_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// testBtn
			// 
			this.testBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.testBtn.Image = ((System.Drawing.Image)(resources.GetObject("testBtn.Image")));
			this.testBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.testBtn.Name = "testBtn";
			this.testBtn.Size = new System.Drawing.Size(38, 22);
			this.testBtn.Text = "Пуск";
			this.testBtn.Click += new System.EventHandler(this.testBtn_Click_1);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 49);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1271, 540);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(638, 3);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(630, 534);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(629, 534);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "JPG, PNG|*.jpg;*.png";
			// 
			// testProgressBar
			// 
			this.testProgressBar.Name = "testProgressBar";
			this.testProgressBar.Size = new System.Drawing.Size(100, 15);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1271, 589);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton recogniseBtn;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ToolStripButton cutToolStripButton2;
		private System.Windows.Forms.ToolStripButton questionBtn;
		private System.Windows.Forms.ToolStripButton variant1Btn;
		private System.Windows.Forms.ToolStripButton variant2Btn;
		private System.Windows.Forms.ToolStripButton variant3Btn;
		private System.Windows.Forms.ToolStripButton variant4Btn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton recogniseAllBtn;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton screenshootBtn;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripTextBox processIdTextBox;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton analyzeBtn;
		private System.Windows.Forms.ToolStripButton numericAnswerBtn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton testBtn;
		private System.Windows.Forms.ToolStripProgressBar testProgressBar;
	}
}

