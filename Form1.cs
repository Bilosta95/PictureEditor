using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Forms;
using OpenCvSharp;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private IEditor editor;
        private Label lblHover;

        public Form1()
        {
            InitializeComponent();
            lblHover = new Label();
            lblHover.Visible = false;
            lblHover.AutoSize = true;
            lblHover.BackColor = Color.LightYellow;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            editor = new Editor();
            // Kreiranje OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Postavljanje filtera za dijalog - prikazuju se samo slike
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Select an Image File";

            // Prikazivanje dijaloga i provera da li je korisnik odabrao fajl
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Ucitavanje odabrane slike u PictureBox
                editor.LoadImage(openFileDialog.FileName);
                pictureBox1.Image = editor.CompareImage;

                // Automatsko podesavanje velicine slike unutar PictureBox-a
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                // Prikazivanje naziva forme sa imenom i velicinom slike
                this.Text = "Picture Editor" + " | "
                    + Path.GetFileName(openFileDialog.FileName) + " | "
                    + editor.CompareImage.Width + $" px x " + editor.CompareImage.Height + $" px";

                btnCrop.BackColor = SystemColors.ButtonFace; // Resetuj boju
                btnMarkup.BackColor = SystemColors.ButtonFace; // Resetuj boju
                btnAddText.BackColor = SystemColors.ButtonFace; // Resetuj boju
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Ako je slika ucitana u PictureBox
            if (pictureBox1.Image != null)
            {
                // Kreiranje dijaloga za cuvanje slike
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                // Postavljanje filtera
                saveFileDialog.Filter = "JPG Image|*.jpg|JPEG Image|*.jpeg|PNG Image|*.png|Bitmap Image|*.bmp";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.FileName = "image";
                // Prikazivanje dijaloga i provera rezultata
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Cuvanje slike
                    pictureBox1.Image.Save(saveFileDialog.FileName);
                    // Omogucava automatsko prikazivanje poruke
                    saveFileDialog.OverwritePrompt = true;
                }
            }
            else
            {
                // Error poruka ako slika ne postoji u PictureBox-u
                MessageBox.Show("No image to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Rukovanje MouseDown dogadjajem
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && editor.IsSelecting && !editor.IsSelectActive && e.Button == MouseButtons.Left)
            {
                editor.StartPoint = editor.GetScaledPoint(e.Location, pictureBox1);

                editor.IsSelectActive = true;
                editor.IsDragging = true;
                editor.IsDraggingActive = false;
            }
            if (pictureBox1.Image != null && editor.IsTextActive && e.Button == MouseButtons.Left)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                editor.TextPosition = editor.GetScaledPoint(e.Location, pictureBox1);

                pictureBox1.Image = editor.DrawTextOnImage(editor.TextPosition, pictureBox1);
                editor.IsDragging = true;
                editor.IsDraggingActive = false;
            }
            if (pictureBox1.Image != null && editor.IsMarking && e.Button == MouseButtons.Left)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                editor.StartMarkupPoint = editor.GetScaledPoint(e.Location, pictureBox1);
                editor.IsDragging = true;
                editor.IsDraggingActive = false;
            }
            if (pictureBox1.Image != null && !editor.IsDragging && !editor.IsDraggingActive && e.Button == MouseButtons.Left)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                pictureBox1.Cursor = Cursors.Hand;
                editor.DragStartPoint = editor.GetScaledPoint(e.Location, pictureBox1);
                editor.IsDragging = true;
                editor.IsDraggingActive = true;
            }
        }

        // Rukovanje MouseMove dogadjajem
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && editor.IsSelecting && editor.IsSelectActive)
            {
                System.Drawing.Point currentPoint = editor.GetScaledPoint(e.Location, pictureBox1);

                // Kreiramo selekcioni pravougaonik
                int x = Math.Min(editor.StartPoint.X, currentPoint.X);
                int y = Math.Min(editor.StartPoint.Y, currentPoint.Y);
                int width = Math.Abs(editor.StartPoint.X - currentPoint.X);
                int height = Math.Abs(editor.StartPoint.Y - currentPoint.Y);

                // Provera da selekcija ne izlazi izvan granica slike
                if (x < 0) { width -= Math.Abs(x); x = 0; }
                if (y < 0) { height -= Math.Abs(y); y = 0; }
                if (x + width > pictureBox1.Image.Width) width = pictureBox1.Image.Width - x;
                if (y + height > pictureBox1.Image.Height) height = pictureBox1.Image.Height - y;

                editor.SelectionRectangle = new Rectangle(x, y, width, height);

                // Crtamo selekciju na slici
                Bitmap tempImage = new Bitmap(editor.OriginalImage);
                System.Drawing.Point point = editor.GetScaledPoint(new System.Drawing.Point(tempImage.Width, tempImage.Height), pictureBox1);
                using (Graphics g = Graphics.FromImage(tempImage))
                {
                    Pen pen = new Pen(Color.Red, Math.Max(point.X, point.Y) / 1000);
                    g.DrawRectangle(pen, editor.SelectionRectangle);
                }

                pictureBox1.Image = tempImage; // Azuriramo PictureBox
            }
            if (pictureBox1.Image != null && editor.IsMarking && e.Button == MouseButtons.Left)
            {
                editor.EndMarkupPoint = editor.GetScaledPoint(e.Location, pictureBox1);

                float adjustedSize = editor.GetFontSizeBasedOnImageSize(editor.MarkupSize, pictureBox1);
                Pen pen = new Pen(editor.MarkupColor, adjustedSize);

                // Pozovite funkciju za crtanje na privremenoj slici
                pictureBox1.Image = editor.DrawShapeOnImage(editor.StartMarkupPoint, editor.EndMarkupPoint, pen);
            }
            if (pictureBox1.Image != null && editor.IsDragging && editor.IsDraggingActive && e.Button == MouseButtons.Left)
            {
                //Preracunavanje pozicije slike tokom prevlacenja
                float deltaX = e.X - editor.DragStartPoint.X;
                float deltaY = e.Y - editor.DragStartPoint.Y;
                editor.DragStartPoint = editor.GetScaledPoint(e.Location, pictureBox1);

                editor.OffsetX += deltaX;
                editor.OffsetY += deltaY;

                pictureBox1.Image = editor.RedrawImage(pictureBox1);
                pictureBox1.Refresh();
            }
        }

        // Rukovanje MouseUp dogadjajem
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && editor.IsSelecting)
            {
                editor.IsSelecting = false;
                editor.IsSelectActive = true;
            }
            if (pictureBox1.Image != null && editor.IsMarking)
            {
                editor.OriginalImage = (Bitmap)pictureBox1.Image;
            }
            if (pictureBox1.Image != null && editor.IsDragging && editor.IsDraggingActive)
            {
                editor.IsDragging = false;
                editor.IsDraggingActive = false;
                pictureBox1.Cursor = Cursors.Default;
            }
        }

        // Pritiskom na dugme Cut vrsi se odsecanje selektovane oblasti
        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.IsDraggingActive = !editor.IsDraggingActive; // Prebaci stanje aktivacije
                if (pictureBox1.Image != null && editor.IsTextActive || editor.IsMarking)
                {
                    editor.IsMarking = false;
                    editor.IsTextActive = false;
                    btnMarkup.BackColor = SystemColors.ButtonFace; // Resetuj boju
                    btnAddText.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
                if (pictureBox1.Image != null && !editor.IsSelectActive && !editor.IsSelecting)
                {
                    // Postavi boju
                    btnCrop.BackColor = Color.LightGreen;

                    // Resetovanje selekcije
                    editor.SelectionRectangle = Rectangle.Empty;
                    // Fokusiramo PictureBox za interakciju sa misem
                    pictureBox1.Focus();
                    editor.IsSelecting = true;
                }
                if (pictureBox1.Image != null && editor.IsSelectActive && !editor.IsSelecting)
                {
                    // Resetuj boju
                    btnCrop.BackColor = SystemColors.ButtonFace;

                    // Nakon zavrsene selekcije, vratite originalnu sliku bez selekcije 
                    // Ovo je pictureBox1.Image = editor.OriginalImage; da se ne bi video crveni pravougaonik
                    pictureBox1.Image = editor.OriginalImage;
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.CropImage(editor.SelectionRectangle, pictureBox1.Image);
                    editor.IsSelectActive = false;
                }
            }
        }

        // Undo funkcija
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (editor.UndoStack.Count > 0)
                {
                    if (editor.OriginalImageBeforeCrop != null)
                    {
                        editor.OriginalImage = (Bitmap)editor.OriginalImageBeforeCrop; // Obnovi sliku
                        editor.OriginalImageBeforeCrop = null; // Obrisi sacuvanu sliku
                    }
                    // Cuvamo trenutno stanje slike na redo stek pre nego sto napravimo undo
                    editor.RedoStack.Push(new Bitmap(pictureBox1.Image));

                    // Vracamo prethodnu sliku iz undo steka
                    editor.OriginalImage = editor.UndoStack.Pop();
                    pictureBox1.Image = editor.OriginalImage;
                }
                else
                {
                    MessageBox.Show("No more states to undo.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Redo funkcija
        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (editor.RedoStack.Count > 0)
                {
                    // Cuvamo trenutno stanje slike na undo stek pre nego sto napravimo redo
                    editor.UndoStack.Push(new Bitmap(pictureBox1.Image));

                    // Vracamo sliku iz redo steka
                    pictureBox1.Image = editor.RedoStack.Pop();
                }
                else
                {
                    MessageBox.Show("No more states to redo.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap originalBitmap = new Bitmap(pictureBox1.Image);

                editor.SaveStateForUndo(pictureBox1.Image);
                // Pozovite funkciju za rotaciju sa uglom od -90 stepeni
                pictureBox1.Image = editor.RotateImage(originalBitmap, -90);
                editor.OriginalImage = (Bitmap)pictureBox1.Image;
            }
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap originalBitmap = new Bitmap(pictureBox1.Image);

                editor.SaveStateForUndo(pictureBox1.Image);
                // Pozovite funkciju za rotaciju sa uglom od 90 stepeni
                pictureBox1.Image = editor.RotateImage(originalBitmap, 90);
                editor.OriginalImage = (Bitmap)pictureBox1.Image;
            }
        }

        private void btnCompare_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && editor.CurrentImage == null) // Ako ne postoji trenutna slika
            {
                editor.CurrentImage = (Bitmap)pictureBox1.Image; // Pamtimo trenutnu sliku
                pictureBox1.Image = editor.CompareImage; // Prikazujemo originalnu sliku
            }
        }

        private void btnCompare_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && editor.CurrentImage != null) // Ako postoji trenutna slika
            {
                pictureBox1.Image = editor.CurrentImage; // Vracamo trenutnu sliku
                editor.CurrentImage = null;
            }
        }

        private void btnAddText_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                editor.IsDraggingActive = !editor.IsDraggingActive; // Prebaci stanje aktivacije
                editor.IsTextActive = !editor.IsTextActive; // Prebaci stanje aktivacije
                if (pictureBox1.Image != null && editor.IsTextActive && editor.IsMarking)
                {
                    editor.IsMarking = false;
                    btnMarkup.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
                if (pictureBox1.Image != null && editor.IsSelectActive)
                {
                    editor.IsSelectActive = false;
                    btnCrop.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
                if (pictureBox1.Image != null && editor.IsTextActive)
                {
                    btnAddText.BackColor = Color.LightGreen; // Promeni boju dugmeta kada je aktivno
                    // Kreiraj formu za unos teksta
                    using (Form inputForm = new Form())
                    {
                        inputForm.Text = "Text";
                        inputForm.AutoSize = true;
                        inputForm.AutoSizeMode = AutoSizeMode.GrowOnly;
                        inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        Label label = new Label() { Text = "Enter Text:", Dock = DockStyle.Top };
                        TextBox textBox = new TextBox() { Dock = DockStyle.Top };
                        textBox.Text = "Enter Text";
                        textBox.Click += new EventHandler(textBox_Click);
                        // Metoda za selektovanje teksta
                        void textBox_Click(object sender, EventArgs e)
                        {
                            textBox.SelectAll(); // Selektuje sav tekst u TextBox-u
                        }
                        Button okButton = new Button() { Text = "OK", Dock = DockStyle.Bottom };
                        okButton.AutoSize = true;
                        okButton.AutoSizeMode = AutoSizeMode.GrowOnly;
                        okButton.DialogResult = DialogResult.OK;

                        // Dodaj ComboBox za izbor velicine fonta
                        Label sizeLabel = new Label() { Text = "Select size:", Dock = DockStyle.Top };
                        ComboBox sizeComboBox = new ComboBox() { Dock = DockStyle.Top };
                        sizeComboBox.Items.AddRange(new object[] { "10", "12", "14", "16", "18", "20", "22", "24", "28", "32" });
                        sizeComboBox.SelectedItem = "14"; // Podrazumevana velicina
                        sizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                        // Dodaj dugme za izbor boje
                        Button colorButton = new Button() { Text = "Select Color", Dock = DockStyle.Top };
                        colorButton.Height = 100;
                        colorButton.BackColor = editor.TextColor;

                        colorButton.Click += (s, args) =>
                        {
                            using (ColorDialog colorDialog = new ColorDialog())
                            {
                                if (colorDialog.ShowDialog() == DialogResult.OK)
                                {
                                    editor.TextColor = colorDialog.Color; // Postavi izabranu boju
                                    colorButton.BackColor = editor.TextColor; // Azuriraj boju dugmeta
                                }
                            }
                        };

                        inputForm.Controls.Add(sizeComboBox);
                        inputForm.Controls.Add(sizeLabel);
                        inputForm.Controls.Add(textBox);
                        inputForm.Controls.Add(label);
                        inputForm.Controls.Add(colorButton);
                        inputForm.Controls.Add(okButton);
                        inputForm.AcceptButton = okButton;

                        if (inputForm.ShowDialog() == DialogResult.OK)
                        {
                            editor.InputText = textBox.Text; // Postavi uneti tekst
                            editor.TextSize = int.Parse(sizeComboBox.SelectedItem.ToString()); // Postavi velicinu
                        }
                    }
                }
                else
                {
                    btnAddText.BackColor = SystemColors.ButtonFace; // Resetuj boju
                    editor.IsTextActive = false; // Prebaci stanje aktivacije
                }
            }
        }

        private void btnMarkup_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                editor.IsDraggingActive = !editor.IsDraggingActive; // Prebaci stanje aktivacije
                editor.IsMarking = !editor.IsMarking; // Prebaci stanje aktivacije
                if (pictureBox1.Image != null && editor.IsMarking && editor.IsTextActive)
                {
                    editor.IsTextActive = false;
                    btnAddText.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
                if (pictureBox1.Image != null && editor.IsSelectActive)
                {
                    editor.IsSelectActive = false;
                    btnCrop.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
                if (pictureBox1.Image != null && editor.IsMarking)
                {
                    btnMarkup.BackColor = Color.LightGreen; // Promeni boju dugmeta kada je aktivno
                                                            // Kreiraj formu za odabir oblika
                    using (Form inputForm = new Form())
                    {
                        inputForm.Text = "Markup";
                        inputForm.AutoSize = true;
                        inputForm.AutoSizeMode = AutoSizeMode.GrowOnly;
                        inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        Button okButton = new Button() { Text = "OK", Dock = DockStyle.Bottom };
                        okButton.AutoSize = true;
                        okButton.AutoSizeMode = AutoSizeMode.GrowOnly;
                        okButton.DialogResult = DialogResult.OK;

                        // Dodaj ComboBox za izbor oblika
                        Label shapeLabel = new Label() { Text = "Select shape:", Dock = DockStyle.Top };
                        ComboBox shapeComboBox = new ComboBox() { Dock = DockStyle.Top };
                        shapeComboBox.Items.AddRange(new object[] { "Line", "Rectangle", "Ellipse" });
                        shapeComboBox.SelectedItem = "Line"; // Podrazumevan oblik
                        shapeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                        // Dodaj ComboBox za izbor velicine olovke
                        Label sizeLabel = new Label() { Text = "Select size:", Dock = DockStyle.Top };
                        ComboBox sizeComboBox = new ComboBox() { Dock = DockStyle.Top };
                        sizeComboBox.Items.AddRange(new object[] { "2", "4", "6", "8", "10", "12", "14", "16", "18", "20" });
                        sizeComboBox.SelectedItem = "6"; // Podrazumevana velicina
                        sizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                        // Dodaj dugme za izbor boje
                        Button colorButton = new Button() { Text = "Select Color", Dock = DockStyle.Top };
                        colorButton.Height = 100;
                        colorButton.BackColor = editor.MarkupColor;

                        colorButton.Click += (s, args) =>
                        {
                            using (ColorDialog colorDialog = new ColorDialog())
                            {
                                if (colorDialog.ShowDialog() == DialogResult.OK)
                                {
                                    editor.MarkupColor = colorDialog.Color; // Postavi izabranu boju
                                    colorButton.BackColor = editor.MarkupColor; // Azuriraj boju dugmeta
                                }
                            }
                        };

                        inputForm.Controls.Add(shapeComboBox);
                        inputForm.Controls.Add(shapeLabel);
                        inputForm.Controls.Add(sizeComboBox);
                        inputForm.Controls.Add(sizeLabel);
                        inputForm.Controls.Add(colorButton);
                        inputForm.Controls.Add(okButton);
                        inputForm.AcceptButton = okButton;

                        if (inputForm.ShowDialog() == DialogResult.OK)
                        {
                            editor.CurrentShape = shapeComboBox.SelectedItem.ToString(); // Postavi oblik
                            editor.MarkupSize = int.Parse(sizeComboBox.SelectedItem.ToString()); // Postavi velicinu
                        }
                    }
                }
                else
                {
                    btnMarkup.BackColor = SystemColors.ButtonFace; // Resetuj boju
                }
            }
        }

        private void btnFlipHorizontal_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Okreni sliku horizontalno
                pictureBox1.Image = editor.FlipImageHorizontally(pictureBox1.Image);
            }
        }

        private void btnFlipVertical_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Okreni sliku vertikalno
                pictureBox1.Image = editor.FlipImageVertically(pictureBox1.Image);
            }
        }

        // Funkcija za rukovanje MouseWheel dogadjajem
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Azuriranje faktora zumiranja
                editor.ZoomFactor += e.Delta > 0 ? 0.1f : -0.1f;
                // Ogranicenje faktora zumiranja
                editor.ZoomFactor = Math.Clamp(editor.ZoomFactor, 0.1f, 5.0f);

                pictureBox1.Image = editor.RedrawImage(pictureBox1);
                pictureBox1.Refresh();
            }
        }

        // Funkcija za podesavanje osvetljenja slike
        private void btnLight_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Kreiramo novu formu
                editor.LightForm = new Form
                {
                    Text = "Adjust Lighting",
                    AutoSize = true,
                    BackColor = Color.DimGray,
                    StartPosition = FormStartPosition.Manual,
                    Location = new System.Drawing.Point(this.Left + pictureBox1.Left, this.Top + 40 + pictureBox1.Top),
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Opacity = 0.8
                };

                // Kreiramo slajdere
                TrackBar brightnessSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };
                TrackBar exposureSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };
                TrackBar contrastSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };

                // Kreiramo labele za slajdere
                Label brightnessLabel = new Label { Text = "Brightness", Dock = DockStyle.Top };
                Label brightnessLabelValue = new Label { Text = "" + brightnessSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };
                Label exposureLabel = new Label { Text = "Exposure", Dock = DockStyle.Top };
                Label exposureLabelValue = new Label { Text = "" + exposureSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };
                Label contrastLabel = new Label { Text = "Contrast", Dock = DockStyle.Top };
                Label contrastLabelValue = new Label { Text = "" + contrastSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };

                // Kreiramo reset dugme
                Button btnReset = new Button();
                btnReset.Text = "Reset";
                btnReset.UseVisualStyleBackColor = true;
                btnReset.AutoSize = true;
                btnReset.Location = new System.Drawing.Point(editor.LightForm.Width / 2 - btnReset.Width / 2, editor.LightForm.Height);
                
                btnReset.Click += (sender, e) =>
                {
                    brightnessSlider.Value = 0;
                    exposureSlider.Value = 0;
                    contrastSlider.Value = 0;
                };

                var timer = new System.Windows.Forms.Timer { Interval = 30 };

                brightnessSlider.ValueChanged += (sender, e) =>
                {
                    if (brightnessSlider.Value < 0)
                    {
                        brightnessLabelValue.Text = Math.Abs(brightnessSlider.Value) + "-";
                    }
                    else
                    {
                        brightnessLabelValue.Text = Math.Abs(brightnessSlider.Value) + "";
                    }

                    timer.Stop();
                    timer.Start();
                };

                exposureSlider.ValueChanged += (sender, e) =>
                {
                    if (exposureSlider.Value < 0)
                    {
                        exposureLabelValue.Text = Math.Abs(exposureSlider.Value) + "-";
                    }
                    else
                    {
                        exposureLabelValue.Text = Math.Abs(exposureSlider.Value) + "";
                    }

                    timer.Stop();
                    timer.Start();
                };

                contrastSlider.ValueChanged += (sender, e) =>
                {
                    if (contrastSlider.Value < 0)
                    {
                        contrastLabelValue.Text = Math.Abs(contrastSlider.Value) + "-";
                    }
                    else
                    {
                        contrastLabelValue.Text = Math.Abs(contrastSlider.Value) + "";
                    }

                    timer.Stop();
                    timer.Start();
                };

                timer.Tick += (s, e) =>
                {
                    pictureBox1.Image = editor.ApplyLightAdjustments(brightnessSlider.Value, exposureSlider.Value, contrastSlider.Value, pictureBox1.Image);

                    timer.Stop();
                };

                // Dodavanje slajdera i labeli formi
                editor.LightForm.Controls.Add(btnReset);
                editor.LightForm.Controls.Add(contrastSlider);
                editor.LightForm.Controls.Add(contrastLabelValue);
                editor.LightForm.Controls.Add(contrastLabel);
                editor.LightForm.Controls.Add(exposureSlider);
                editor.LightForm.Controls.Add(exposureLabelValue);
                editor.LightForm.Controls.Add(exposureLabel);
                editor.LightForm.Controls.Add(brightnessSlider);
                editor.LightForm.Controls.Add(brightnessLabelValue);
                editor.LightForm.Controls.Add(brightnessLabel);

                editor.LightForm.ShowDialog();
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Kreiramo novu formu
                editor.ColorForm = new Form
                {
                    Text = "Adjust Color",
                    AutoSize = true,
                    BackColor = Color.DimGray,
                    StartPosition = FormStartPosition.Manual,
                    Location = new System.Drawing.Point(this.Left + pictureBox1.Left, this.Top + 40 + pictureBox1.Top),
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Opacity = 0.8
                };

                // Kreiramo slajdere
                TrackBar saturationSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };
                TrackBar warmthSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };
                TrackBar tintSlider = new TrackBar { Minimum = -100, Maximum = 100, TickFrequency = 10, Dock = DockStyle.Top };

                // Kreiramo labele za slajdere
                Label saturationLabel = new Label { Text = "Saturation", Dock = DockStyle.Top };
                Label saturationLabelValue = new Label { Text = "" + saturationSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };
                Label warmthLabel = new Label { Text = "Warmth", Dock = DockStyle.Top };
                Label warmthLabelValue = new Label { Text = "" + warmthSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };
                Label tintLabel = new Label { Text = "Tint", Dock = DockStyle.Top };
                Label tintLabelValue = new Label { Text = "" + tintSlider.Value, Dock = DockStyle.Top, RightToLeft = RightToLeft.Yes };

                // Kreiramo reset dugme
                Button btnReset = new Button();
                btnReset.Text = "Reset";
                btnReset.UseVisualStyleBackColor = true;
                btnReset.AutoSize = true;
                btnReset.Location = new System.Drawing.Point(editor.ColorForm.Width / 2 - btnReset.Width / 2, editor.ColorForm.Height);

                btnReset.Click += (sender, e) =>
                {
                    saturationSlider.Value = 0;
                    warmthSlider.Value = 0;
                    tintSlider.Value = 0;
                };

                var timer = new System.Windows.Forms.Timer { Interval = 30 };

                saturationSlider.ValueChanged += (sender, e) =>
                {
                    if (saturationSlider.Value < 0)
                    {
                        saturationLabelValue.Text = Math.Abs(saturationSlider.Value) + "-";
                    }
                    else
                    {
                        saturationLabelValue.Text = Math.Abs(saturationSlider.Value) + "";
                    }

                    timer.Stop();
                    timer.Start();
                };

                warmthSlider.ValueChanged += (sender, e) =>
                {
                    if (warmthSlider.Value < 0)
                    {
                        warmthLabelValue.Text = Math.Abs(warmthSlider.Value) + "-";
                    }
                    else
                    {
                        warmthLabelValue.Text = Math.Abs(warmthSlider.Value) + "";
                    }

                    timer.Stop();
                    timer.Start();
                };

                tintSlider.ValueChanged += (sender, e) =>
                {
                    if (tintSlider.Value < 0)
                    {
                        tintLabelValue.Text = Math.Abs(tintSlider.Value) + "-";
                    }
                    else
                    {
                        tintLabelValue.Text = Math.Abs(tintSlider.Value) + "";
                    }
                    timer.Stop();
                    timer.Start();
                };

                timer.Tick += (s, e) =>
                {

                    pictureBox1.Image = editor.ApplyColorAdjustments(saturationSlider.Value, warmthSlider.Value, tintSlider.Value, pictureBox1.Image);

                    timer.Stop();
                };

                // Dodavanje slajdera i labeli formi
                editor.ColorForm.Controls.Add(btnReset);
                editor.ColorForm.Controls.Add(tintSlider);
                editor.ColorForm.Controls.Add(tintLabelValue);
                editor.ColorForm.Controls.Add(tintLabel);
                editor.ColorForm.Controls.Add(warmthSlider);
                editor.ColorForm.Controls.Add(warmthLabelValue);
                editor.ColorForm.Controls.Add(warmthLabel);
                editor.ColorForm.Controls.Add(saturationSlider);
                editor.ColorForm.Controls.Add(saturationLabelValue);
                editor.ColorForm.Controls.Add(saturationLabel);

                editor.ColorForm.ShowDialog();
            }
        }

        private void btnFilters_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                editor.SaveStateForUndo(pictureBox1.Image);
                // Kreiramo novu formu
                editor.FilterForm = new Form
                {
                    Text = "Choose a filter",
                    AutoSize = true,
                    BackColor = Color.DimGray,
                    StartPosition = FormStartPosition.Manual,
                    Location = new System.Drawing.Point(this.Left + pictureBox1.Left, this.Top + 40 + pictureBox1.Top),
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Opacity = 0.8
                };

                Button btnBokeh = new Button { Text = "Bokeh", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(0, 0) };
                Button btnVintage = new Button { Text = "Vintage", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(100, 0) };
                Button btnSepia = new Button { Text = "Sepia", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(200, 0) };

                Button btnBlackandWhite = new Button { Text = "Black and White", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(0, 100) };
                Button btnGlow = new Button { Text = "Glow", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(100, 100) };
                Button btnBlur = new Button { Text = "Blur", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(200, 100) };

                Button btnVignette = new Button { Text = "Vignette", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(0, 200) };
                Button btnContrast = new Button { Text = "Contrast", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(100, 200) };
                Button btnSaturation = new Button { Text = "Saturation", Size = new System.Drawing.Size(100, 100), Location = new System.Drawing.Point(200, 200) };

                btnBokeh.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnBokeh.Text);
                    pictureBox1.Refresh();
                };

                btnVintage.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnVintage.Text);
                    pictureBox1.Refresh();
                };

                btnSepia.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnSepia.Text);
                    pictureBox1.Refresh();
                };

                btnBlackandWhite.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnBlackandWhite.Text);
                    pictureBox1.Refresh();
                };

                btnGlow.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnGlow.Text);
                    pictureBox1.Refresh();
                };

                btnBlur.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnBlur.Text);
                    pictureBox1.Refresh();
                };

                btnVignette.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnVignette.Text);
                    pictureBox1.Refresh();
                };

                btnContrast.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnContrast.Text);
                    pictureBox1.Refresh();
                };

                btnSaturation.Click += (s, e) =>
                {
                    editor.SaveStateForUndo(pictureBox1.Image);
                    pictureBox1.Image = editor.ApplyFilter(btnSaturation.Text);
                    pictureBox1.Refresh();
                };

                editor.FilterForm.Controls.Add(btnBokeh);
                editor.FilterForm.Controls.Add(btnVintage);
                editor.FilterForm.Controls.Add(btnSepia);
                editor.FilterForm.Controls.Add(btnBlackandWhite);
                editor.FilterForm.Controls.Add(btnGlow);
                editor.FilterForm.Controls.Add(btnBlur);
                editor.FilterForm.Controls.Add(btnVignette);
                editor.FilterForm.Controls.Add(btnContrast);
                editor.FilterForm.Controls.Add(btnSaturation);

                editor.FilterForm.ShowDialog();

            }
        }

        private void btnUndo_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Undo";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnUndo.Left,
                tableLayoutPanel2.Top + btnUndo.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnUndo_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnRedo_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Redo";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnRedo.Left,
                tableLayoutPanel2.Top + btnRedo.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnRedo_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnRotateLeft_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Rotate Left";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnRotateLeft.Left,
                tableLayoutPanel2.Top + btnRotateLeft.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnRotateLeft_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnRotateRight_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Rotate Right";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnRotateRight.Left,
                tableLayoutPanel2.Top + btnRotateRight.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnRotateRight_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnFlipHorizontal_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Flip Horizontally";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnFlipHorizontal.Left,
                tableLayoutPanel2.Top + btnFlipHorizontal.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnFlipHorizontal_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnFlipVertical_MouseEnter(object sender, EventArgs e)
        {
            lblHover.Text = "Flip Vertically";
            lblHover.Visible = true;
            lblHover.Location = new System.Drawing.Point(
                tableLayoutPanel2.Left + btnFlipVertical.Left,
                tableLayoutPanel2.Top + btnFlipVertical.Top - 20);
            this.Controls.Add(lblHover);
            lblHover.BringToFront();
        }

        private void btnFlipVertical_MouseLeave(object sender, EventArgs e)
        {
            lblHover.Visible = false;
        }

        private void btnOriginalSize_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image = editor.OriginalImage;
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }
    }
}

