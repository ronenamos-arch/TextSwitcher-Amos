using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

public class QuickAccessForm : Form
{
    private TemplateManager templateManager;
    private TextSwitcherApp mainApp;
    private ListBox templateListBox;
    private TextBox previewTextBox;
    private Button convertButton;
    private Button caseSwapButton;
    private Button pasteTemplateButton;
    private Button addTemplateButton;
    private Button editTemplateButton;
    private Button deleteTemplateButton;
    private Button refreshButton;

    public QuickAccessForm(TextSwitcherApp app, TemplateManager manager)
    {
        mainApp = app;
        templateManager = manager;
        InitializeComponents();
        LoadTemplates();
    }

    private void InitializeComponents()
    {
        // Form settings
        this.Text = "TextSwitcher - Quick Access";
        this.Size = new Size(400, 550);
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        this.StartPosition = FormStartPosition.Manual;
        this.TopMost = true;
        this.ShowInTaskbar = false;

        // Position in bottom-right corner
        Rectangle screen = Screen.PrimaryScreen.WorkingArea;
        this.Location = new Point(screen.Right - this.Width - 20, screen.Bottom - this.Height - 20);

        // Create main panel
        Panel mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        // Quick Actions Group
        GroupBox actionsGroup = new GroupBox
        {
            Text = "פעולות מהירות",
            Location = new Point(10, 10),
            Size = new Size(360, 100),
            RightToLeft = RightToLeft.Yes
        };

        convertButton = new Button
        {
            Text = "המר טקסט (F10)",
            Location = new Point(15, 25),
            Size = new Size(150, 30),
            BackColor = Color.FromArgb(44, 82, 130),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        convertButton.Click += (s, e) => mainApp.PerformTextConversion();

        caseSwapButton = new Button
        {
            Text = "החלף אותיות (F6)",
            Location = new Point(180, 25),
            Size = new Size(150, 30),
            BackColor = Color.FromArgb(104, 211, 145),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        caseSwapButton.Click += (s, e) => mainApp.SwapCaseClipboard();

        Button copyFromClipboard = new Button
        {
            Text = "📋 העתק מהלוח",
            Location = new Point(15, 60),
            Size = new Size(150, 30),
            BackColor = Color.FromArgb(66, 153, 225),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        copyFromClipboard.Click += CopyFromClipboard_Click;

        actionsGroup.Controls.AddRange(new Control[] { convertButton, caseSwapButton, copyFromClipboard });

        // Templates Group
        GroupBox templatesGroup = new GroupBox
        {
            Text = "תבניות שמורות",
            Location = new Point(10, 120),
            Size = new Size(360, 300),
            RightToLeft = RightToLeft.Yes
        };

        templateListBox = new ListBox
        {
            Location = new Point(15, 25),
            Size = new Size(330, 180),
            RightToLeft = RightToLeft.Yes
        };
        templateListBox.SelectedIndexChanged += TemplateListBox_SelectedIndexChanged;

        previewTextBox = new TextBox
        {
            Location = new Point(15, 210),
            Size = new Size(330, 40),
            Multiline = true,
            ReadOnly = true,
            BackColor = Color.FromArgb(247, 250, 252),
            RightToLeft = RightToLeft.Yes
        };

        // Template action buttons
        pasteTemplateButton = new Button
        {
            Text = "הדבק תבנית",
            Location = new Point(15, 255),
            Size = new Size(100, 30),
            BackColor = Color.FromArgb(72, 187, 120),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        pasteTemplateButton.Click += PasteTemplate_Click;

        addTemplateButton = new Button
        {
            Text = "➕ הוסף",
            Location = new Point(125, 255),
            Size = new Size(70, 30),
            BackColor = Color.FromArgb(44, 82, 130),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        addTemplateButton.Click += AddTemplate_Click;

        editTemplateButton = new Button
        {
            Text = "✏️ ערוך",
            Location = new Point(205, 255),
            Size = new Size(60, 30),
            BackColor = Color.FromArgb(237, 137, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        editTemplateButton.Click += EditTemplate_Click;

        deleteTemplateButton = new Button
        {
            Text = "🗑️ מחק",
            Location = new Point(275, 255),
            Size = new Size(70, 30),
            BackColor = Color.FromArgb(229, 62, 62),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        deleteTemplateButton.Click += DeleteTemplate_Click;

        templatesGroup.Controls.AddRange(new Control[] {
            templateListBox, previewTextBox, pasteTemplateButton,
            addTemplateButton, editTemplateButton, deleteTemplateButton
        });

        // Bottom buttons
        refreshButton = new Button
        {
            Text = "🔄 רענן",
            Location = new Point(10, 430),
            Size = new Size(100, 35),
            BackColor = Color.FromArgb(66, 153, 225),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        refreshButton.Click += (s, e) => LoadTemplates();

        Button importButton = new Button
        {
            Text = "📥 ייבוא",
            Location = new Point(120, 430),
            Size = new Size(80, 35),
            BackColor = Color.FromArgb(159, 122, 234),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        importButton.Click += ImportTemplates_Click;

        Button exportButton = new Button
        {
            Text = "📤 ייצוא",
            Location = new Point(210, 430),
            Size = new Size(80, 35),
            BackColor = Color.FromArgb(159, 122, 234),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        exportButton.Click += ExportTemplates_Click;

        Button closeButton = new Button
        {
            Text = "✖ סגור",
            Location = new Point(300, 430),
            Size = new Size(70, 35),
            BackColor = Color.FromArgb(113, 128, 150),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        closeButton.Click += (s, e) => this.Hide();

        mainPanel.Controls.AddRange(new Control[] {
            actionsGroup, templatesGroup, refreshButton,
            importButton, exportButton, closeButton
        });

        this.Controls.Add(mainPanel);
    }

    private void LoadTemplates()
    {
        templateListBox.Items.Clear();
        var templates = templateManager.GetAllTemplates();
        foreach (var template in templates)
        {
            templateListBox.Items.Add($"{template.Name} ({template.UseCount} שימושים)");
        }
    }

    private void TemplateListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (templateListBox.SelectedIndex >= 0)
        {
            string selectedText = templateListBox.SelectedItem.ToString();
            string templateName = selectedText.Substring(0, selectedText.LastIndexOf(" ("));

            string content = templateManager.GetTemplateContent(templateName);
            previewTextBox.Text = content?.Length > 100 ? content.Substring(0, 100) + "..." : content;

            pasteTemplateButton.Enabled = true;
            editTemplateButton.Enabled = true;
            deleteTemplateButton.Enabled = true;
        }
        else
        {
            previewTextBox.Text = "";
            pasteTemplateButton.Enabled = false;
            editTemplateButton.Enabled = false;
            deleteTemplateButton.Enabled = false;
        }
    }

    private void PasteTemplate_Click(object sender, EventArgs e)
    {
        if (templateListBox.SelectedIndex >= 0)
        {
            string selectedText = templateListBox.SelectedItem.ToString();
            string templateName = selectedText.Substring(0, selectedText.LastIndexOf(" ("));
            string content = templateManager.GetTemplateContent(templateName);

            if (!string.IsNullOrEmpty(content))
            {
                Clipboard.SetText(content);
                MessageBox.Show("התבנית הועתקה ללוח!", "הצלחה", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTemplates(); // Refresh to update use count
            }
        }
    }

    private void AddTemplate_Click(object sender, EventArgs e)
    {
        using (var dialog = new TemplateDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                templateManager.AddTemplate(dialog.TemplateName, dialog.TemplateContent);
                LoadTemplates();
            }
        }
    }

    private void EditTemplate_Click(object sender, EventArgs e)
    {
        if (templateListBox.SelectedIndex >= 0)
        {
            string selectedText = templateListBox.SelectedItem.ToString();
            string templateName = selectedText.Substring(0, selectedText.LastIndexOf(" ("));
            string content = templateManager.GetTemplateContent(templateName);

            using (var dialog = new TemplateDialog(templateName, content))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    templateManager.UpdateTemplate(templateName, dialog.TemplateName, dialog.TemplateContent);
                    LoadTemplates();
                }
            }
        }
    }

    private void DeleteTemplate_Click(object sender, EventArgs e)
    {
        if (templateListBox.SelectedIndex >= 0)
        {
            string selectedText = templateListBox.SelectedItem.ToString();
            string templateName = selectedText.Substring(0, selectedText.LastIndexOf(" ("));

            var result = MessageBox.Show($"האם למחוק את התבנית '{templateName}'?",
                "אישור מחיקה", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                templateManager.DeleteTemplate(templateName);
                LoadTemplates();
            }
        }
    }

    private void CopyFromClipboard_Click(object sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            string text = Clipboard.GetText();
            using (var dialog = new TemplateDialog("", text))
            {
                dialog.Text = "שמור טקסט מהלוח כתבנית";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    templateManager.AddTemplate(dialog.TemplateName, dialog.TemplateContent);
                    LoadTemplates();
                }
            }
        }
        else
        {
            MessageBox.Show("אין טקסט בלוח!", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ImportTemplates_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.Title = "ייבוא תבניות";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                templateManager.ImportTemplates(openFileDialog.FileName);
                LoadTemplates();
                MessageBox.Show("התבניות יובאו בהצלחה!", "הצלחה", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void ExportTemplates_Click(object sender, EventArgs e)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.Title = "ייצוא תבניות";
            saveFileDialog.FileName = "TextSwitcher_Templates.json";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                templateManager.ExportTemplates(saveFileDialog.FileName);
                MessageBox.Show("התבניות יוצאו בהצלחה!", "הצלחה", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
        }
        base.OnFormClosing(e);
    }
}

// Template Dialog for adding/editing templates
public class TemplateDialog : Form
{
    private TextBox nameTextBox;
    private TextBox contentTextBox;
    public string TemplateName => nameTextBox.Text;
    public string TemplateContent => contentTextBox.Text;

    public TemplateDialog(string name = "", string content = "")
    {
        this.Text = string.IsNullOrEmpty(name) ? "תבנית חדשה" : "ערוך תבנית";
        this.Size = new Size(450, 350);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;

        Label nameLabel = new Label
        {
            Text = "שם התבנית:",
            Location = new Point(20, 20),
            Size = new Size(100, 20)
        };

        nameTextBox = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(400, 25),
            Text = name,
            RightToLeft = RightToLeft.Yes
        };

        Label contentLabel = new Label
        {
            Text = "תוכן התבנית:",
            Location = new Point(20, 80),
            Size = new Size(100, 20)
        };

        contentTextBox = new TextBox
        {
            Location = new Point(20, 105),
            Size = new Size(400, 150),
            Multiline = true,
            Text = content,
            RightToLeft = RightToLeft.Yes,
            ScrollBars = ScrollBars.Vertical
        };

        Button saveButton = new Button
        {
            Text = "שמור",
            Location = new Point(250, 270),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK,
            BackColor = Color.FromArgb(72, 187, 120),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        Button cancelButton = new Button
        {
            Text = "ביטול",
            Location = new Point(340, 270),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel,
            BackColor = Color.FromArgb(113, 128, 150),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        this.Controls.AddRange(new Control[] {
            nameLabel, nameTextBox, contentLabel, contentTextBox,
            saveButton, cancelButton
        });

        this.AcceptButton = saveButton;
        this.CancelButton = cancelButton;
    }
}
