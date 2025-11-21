using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class TextTemplate
{
    public string Name { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUsed { get; set; }
    public int UseCount { get; set; }

    public TextTemplate()
    {
        CreatedDate = DateTime.Now;
        LastUsed = DateTime.Now;
        UseCount = 0;
    }
}

public class TemplateManager
{
    private List<TextTemplate> templates;
    private static readonly string TemplatePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TextSwitcher", "templates.json");

    public TemplateManager()
    {
        templates = LoadTemplates();
    }

    public List<TextTemplate> GetAllTemplates()
    {
        return templates.OrderByDescending(t => t.UseCount).ToList();
    }

    public void AddTemplate(string name, string content)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
            return;

        var existing = templates.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            existing.Content = content;
            existing.LastUsed = DateTime.Now;
        }
        else
        {
            templates.Add(new TextTemplate
            {
                Name = name,
                Content = content
            });
        }
        SaveTemplates();
    }

    public void DeleteTemplate(string name)
    {
        templates.RemoveAll(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        SaveTemplates();
    }

    public string GetTemplateContent(string name)
    {
        var template = templates.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (template != null)
        {
            template.UseCount++;
            template.LastUsed = DateTime.Now;
            SaveTemplates();
            return template.Content;
        }
        return null;
    }

    public void UpdateTemplate(string oldName, string newName, string content)
    {
        var template = templates.FirstOrDefault(t => t.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
        if (template != null)
        {
            template.Name = newName;
            template.Content = content;
            template.LastUsed = DateTime.Now;
            SaveTemplates();
        }
    }

    private List<TextTemplate> LoadTemplates()
    {
        try
        {
            if (File.Exists(TemplatePath))
            {
                string json = File.ReadAllText(TemplatePath);
                return JsonSerializer.Deserialize<List<TextTemplate>>(json) ?? new List<TextTemplate>();
            }
        }
        catch { }
        return new List<TextTemplate>();
    }

    private void SaveTemplates()
    {
        try
        {
            string dir = Path.GetDirectoryName(TemplatePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TemplatePath, json);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error saving templates: {ex.Message}", "Template Manager");
        }
    }

    public void ExportTemplates(string filePath)
    {
        try
        {
            string json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error exporting templates: {ex.Message}", "Template Manager");
        }
    }

    public void ImportTemplates(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var importedTemplates = JsonSerializer.Deserialize<List<TextTemplate>>(json);
                if (importedTemplates != null)
                {
                    foreach (var template in importedTemplates)
                    {
                        AddTemplate(template.Name, template.Content);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error importing templates: {ex.Message}", "Template Manager");
        }
    }
}
