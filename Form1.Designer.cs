namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoad = new Button();
            btnSave = new Button();
            btnCrop = new Button();
            btnUndo = new Button();
            btnRedo = new Button();
            btnRotateLeft = new Button();
            btnRotateRight = new Button();
            pictureBox1 = new PictureBox();
            btnCompare = new Button();
            btnFlipHorizontal = new Button();
            btnFlipVertical = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnOriginalSize = new Button();
            btnFilters = new Button();
            btnColor = new Button();
            btnLight = new Button();
            btnMarkup = new Button();
            btnAddText = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnLoad
            // 
            btnLoad.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoad.Location = new Point(0, 0);
            btnLoad.Margin = new Padding(0, 0, 0, 5);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(100, 45);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.Location = new Point(0, 50);
            btnSave.Margin = new Padding(0, 0, 0, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 45);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCrop
            // 
            btnCrop.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnCrop.Location = new Point(0, 100);
            btnCrop.Margin = new Padding(0, 0, 0, 5);
            btnCrop.Name = "btnCrop";
            btnCrop.Size = new Size(100, 45);
            btnCrop.TabIndex = 2;
            btnCrop.Text = "Crop";
            btnCrop.UseVisualStyleBackColor = true;
            btnCrop.Click += btnCrop_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Bottom;
            btnUndo.BackgroundImage = Properties.Resources.undo;
            btnUndo.BackgroundImageLayout = ImageLayout.Stretch;
            btnUndo.Location = new Point(0, 0);
            btnUndo.Margin = new Padding(0, 0, 0, 5);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(50, 44);
            btnUndo.TabIndex = 10;
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            btnUndo.MouseEnter += btnUndo_MouseEnter;
            btnUndo.MouseLeave += btnUndo_MouseLeave;
            // 
            // btnRedo
            // 
            btnRedo.Anchor = AnchorStyles.Bottom;
            btnRedo.BackgroundImage = Properties.Resources.redo;
            btnRedo.BackgroundImageLayout = ImageLayout.Stretch;
            btnRedo.Location = new Point(50, 0);
            btnRedo.Margin = new Padding(0, 0, 0, 5);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(50, 44);
            btnRedo.TabIndex = 11;
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            btnRedo.MouseEnter += btnRedo_MouseEnter;
            btnRedo.MouseLeave += btnRedo_MouseLeave;
            // 
            // btnRotateLeft
            // 
            btnRotateLeft.Anchor = AnchorStyles.Bottom;
            btnRotateLeft.BackgroundImage = Properties.Resources.rotate_left;
            btnRotateLeft.BackgroundImageLayout = ImageLayout.Stretch;
            btnRotateLeft.Location = new Point(0, 49);
            btnRotateLeft.Margin = new Padding(0, 0, 0, 5);
            btnRotateLeft.Name = "btnRotateLeft";
            btnRotateLeft.Size = new Size(50, 45);
            btnRotateLeft.TabIndex = 12;
            btnRotateLeft.UseVisualStyleBackColor = true;
            btnRotateLeft.Click += btnRotateLeft_Click;
            btnRotateLeft.MouseEnter += btnRotateLeft_MouseEnter;
            btnRotateLeft.MouseLeave += btnRotateLeft_MouseLeave;
            // 
            // btnRotateRight
            // 
            btnRotateRight.Anchor = AnchorStyles.Bottom;
            btnRotateRight.BackgroundImage = Properties.Resources.rotate_right;
            btnRotateRight.BackgroundImageLayout = ImageLayout.Stretch;
            btnRotateRight.Location = new Point(50, 49);
            btnRotateRight.Margin = new Padding(0, 0, 0, 5);
            btnRotateRight.Name = "btnRotateRight";
            btnRotateRight.Size = new Size(50, 45);
            btnRotateRight.TabIndex = 13;
            btnRotateRight.UseVisualStyleBackColor = true;
            btnRotateRight.Click += btnRotateRight_Click;
            btnRotateRight.MouseEnter += btnRotateRight_MouseEnter;
            btnRotateRight.MouseLeave += btnRotateRight_MouseLeave;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(120, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1062, 653);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            // 
            // btnCompare
            // 
            btnCompare.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnCompare.Location = new Point(0, 450);
            btnCompare.Margin = new Padding(0, 0, 0, 5);
            btnCompare.Name = "btnCompare";
            btnCompare.Size = new Size(100, 45);
            btnCompare.TabIndex = 9;
            btnCompare.Text = "Compare";
            btnCompare.UseVisualStyleBackColor = true;
            btnCompare.MouseDown += btnCompare_MouseDown;
            btnCompare.MouseUp += btnCompare_MouseUp;
            // 
            // btnFlipHorizontal
            // 
            btnFlipHorizontal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnFlipHorizontal.BackgroundImage = Properties.Resources.swap_horizontal_icon;
            btnFlipHorizontal.BackgroundImageLayout = ImageLayout.Stretch;
            btnFlipHorizontal.Location = new Point(0, 100);
            btnFlipHorizontal.Margin = new Padding(0, 0, 0, 5);
            btnFlipHorizontal.Name = "btnFlipHorizontal";
            btnFlipHorizontal.Size = new Size(50, 45);
            btnFlipHorizontal.TabIndex = 14;
            btnFlipHorizontal.UseVisualStyleBackColor = true;
            btnFlipHorizontal.Click += btnFlipHorizontal_Click;
            btnFlipHorizontal.MouseEnter += btnFlipHorizontal_MouseEnter;
            btnFlipHorizontal.MouseLeave += btnFlipHorizontal_MouseLeave;
            // 
            // btnFlipVertical
            // 
            btnFlipVertical.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnFlipVertical.BackgroundImage = Properties.Resources.swap_vertical_icon;
            btnFlipVertical.BackgroundImageLayout = ImageLayout.Stretch;
            btnFlipVertical.Location = new Point(50, 100);
            btnFlipVertical.Margin = new Padding(0, 0, 0, 5);
            btnFlipVertical.Name = "btnFlipVertical";
            btnFlipVertical.Size = new Size(50, 45);
            btnFlipVertical.TabIndex = 15;
            btnFlipVertical.UseVisualStyleBackColor = true;
            btnFlipVertical.Click += btnFlipVertical_Click;
            btnFlipVertical.MouseEnter += btnFlipVertical_MouseEnter;
            btnFlipVertical.MouseLeave += btnFlipVertical_MouseLeave;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(btnOriginalSize, 0, 8);
            tableLayoutPanel1.Controls.Add(btnFilters, 0, 7);
            tableLayoutPanel1.Controls.Add(btnCompare, 0, 8);
            tableLayoutPanel1.Controls.Add(btnColor, 0, 6);
            tableLayoutPanel1.Controls.Add(btnLight, 0, 5);
            tableLayoutPanel1.Controls.Add(btnMarkup, 0, 3);
            tableLayoutPanel1.Controls.Add(btnAddText, 0, 4);
            tableLayoutPanel1.Controls.Add(btnLoad, 0, 0);
            tableLayoutPanel1.Controls.Add(btnSave, 0, 1);
            tableLayoutPanel1.Controls.Add(btnCrop, 0, 2);
            tableLayoutPanel1.Location = new Point(10, 10);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(100, 500);
            tableLayoutPanel1.TabIndex = 16;
            // 
            // btnOriginalSize
            // 
            btnOriginalSize.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnOriginalSize.Location = new Point(0, 400);
            btnOriginalSize.Margin = new Padding(0, 0, 0, 5);
            btnOriginalSize.Name = "btnOriginalSize";
            btnOriginalSize.Size = new Size(100, 45);
            btnOriginalSize.TabIndex = 8;
            btnOriginalSize.Text = "Original";
            btnOriginalSize.UseVisualStyleBackColor = true;
            btnOriginalSize.Click += btnOriginalSize_Click;
            // 
            // btnFilters
            // 
            btnFilters.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnFilters.Location = new Point(0, 350);
            btnFilters.Margin = new Padding(0, 0, 0, 5);
            btnFilters.Name = "btnFilters";
            btnFilters.Size = new Size(100, 45);
            btnFilters.TabIndex = 7;
            btnFilters.Text = "Filters";
            btnFilters.UseVisualStyleBackColor = true;
            btnFilters.Click += btnFilters_Click;
            // 
            // btnColor
            // 
            btnColor.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnColor.Location = new Point(0, 300);
            btnColor.Margin = new Padding(0, 0, 0, 5);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(100, 45);
            btnColor.TabIndex = 6;
            btnColor.Text = "Color";
            btnColor.UseVisualStyleBackColor = true;
            btnColor.Click += btnColor_Click;
            // 
            // btnLight
            // 
            btnLight.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnLight.Location = new Point(0, 250);
            btnLight.Margin = new Padding(0, 0, 0, 5);
            btnLight.Name = "btnLight";
            btnLight.Size = new Size(100, 45);
            btnLight.TabIndex = 5;
            btnLight.Text = "Light";
            btnLight.UseVisualStyleBackColor = true;
            btnLight.Click += btnLight_Click;
            // 
            // btnMarkup
            // 
            btnMarkup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnMarkup.Location = new Point(0, 150);
            btnMarkup.Margin = new Padding(0, 0, 0, 5);
            btnMarkup.Name = "btnMarkup";
            btnMarkup.Size = new Size(100, 45);
            btnMarkup.TabIndex = 3;
            btnMarkup.Text = "Markup";
            btnMarkup.UseVisualStyleBackColor = true;
            btnMarkup.Click += btnMarkup_Click;
            // 
            // btnAddText
            // 
            btnAddText.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnAddText.Location = new Point(0, 200);
            btnAddText.Margin = new Padding(0, 0, 0, 5);
            btnAddText.Name = "btnAddText";
            btnAddText.Size = new Size(100, 45);
            btnAddText.TabIndex = 4;
            btnAddText.Text = "Text";
            btnAddText.UseVisualStyleBackColor = true;
            btnAddText.Click += btnAddText_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(btnUndo, 0, 0);
            tableLayoutPanel2.Controls.Add(btnRedo, 1, 0);
            tableLayoutPanel2.Controls.Add(btnFlipVertical, 1, 2);
            tableLayoutPanel2.Controls.Add(btnRotateLeft, 0, 1);
            tableLayoutPanel2.Controls.Add(btnFlipHorizontal, 0, 2);
            tableLayoutPanel2.Controls.Add(btnRotateRight, 1, 1);
            tableLayoutPanel2.Location = new Point(10, 510);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel2.Size = new Size(100, 150);
            tableLayoutPanel2.TabIndex = 17;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.WindowFrame;
            ClientSize = new Size(1182, 663);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(pictureBox1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "Form1";
            Text = "Picture Editor";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLoad;
        private Button btnSave;
        private Button btnCrop;
        private Button btnUndo;
        private Button btnRedo;
        private Button btnRotateLeft;
        private Button btnRotateRight;
        private PictureBox pictureBox1;
        private Button btnCompare;
        private Button btnFlipHorizontal;
        private Button btnFlipVertical;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnMarkup;
        private Button btnAddText;
        private Button btnColor;
        private Button btnLight;
        private Button btnFilters;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnOriginalSize;
    }
}
