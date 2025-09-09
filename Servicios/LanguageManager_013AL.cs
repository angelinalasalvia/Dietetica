using Servicios;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

public class LanguageManager_013AL : ISubject_013AL
{
    private List<IObserver_013AL> ListaFormularios_013AL = new List<IObserver_013AL>();
    private Dictionary<string, string> Diccionario_013AL;

    // Patrón singleton
    private static LanguageManager_013AL instancia_013AL;

    private LanguageManager_013AL() 
    {
        Diccionario_013AL = new Dictionary<string, string>();
        //SingletonSesion.Instance.IdiomaActual = "es";
    }

    public static LanguageManager_013AL ObtenerInstancia_013AL()
    {
        if (instancia_013AL == null)
        {
            instancia_013AL = new LanguageManager_013AL();
        }
        return instancia_013AL;
    }

    // Patrón observer
    public void Agregar_013AL(IObserver_013AL observer)
    {
        ListaFormularios_013AL.Add(observer);
    }

    public void Quitar_013AL(IObserver_013AL observer)
    {
        ListaFormularios_013AL.Remove(observer);
    }

    public void Notificar_013AL()
    {
        foreach (IObserver_013AL observer in ListaFormularios_013AL)
        {
            observer.ActualizarIdioma_013AL();
        }
    }

    public void CargarIdioma_013AL()
    {
        try
        {
            var NombreArchivo = $"{SingletonSession_013AL.Instance.IdiomaActual_013AL}.json";
            if (File.Exists(NombreArchivo))
            {
                var jsonString = File.ReadAllText(NombreArchivo);
                Diccionario_013AL = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }
            else
            {
                MessageBox.Show($"El archivo de idioma {NombreArchivo} no existe.");
            }
        }
        catch (JsonException ex)
        {
            MessageBox.Show($"Error al deserializar el JSON: {ex.Message}");
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Error al leer el archivo: {ex.Message}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error inesperado: {ex.Message}");
        }
    }

    public string ObtenerTexto_013AL(string key)
    {
        return Diccionario_013AL.ContainsKey(key) ? Diccionario_013AL[key] : key;
    }

    public void CambiarIdiomaControles_013AL(Control frm)
    {
        try
        {
            frm.Text = ObtenerTexto_013AL(frm.Name + ".Text");

            foreach (Control c in frm.Controls)
            {
                if (c is Button || c is Label || c is RadioButton || c is CheckBox)
                {
                    c.Text = ObtenerTexto_013AL(frm.Name + "." + c.Name);
                }

                if (c is MenuStrip m)
                {
                    foreach (ToolStripMenuItem item in m.Items)
                    {
                        item.Text = ObtenerTexto_013AL(frm.Name + "." + item.Name);
                        CambiarIdiomaMenuStrip_013AL(item.DropDownItems, frm);
                    }
                }

                if (c.Controls.Count > 0)
                {
                    CambiarIdiomaControles_013AL(c);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cambiar el idioma de los controles: {ex.Message}");
        }
    }

    private void CambiarIdiomaMenuStrip_013AL(ToolStripItemCollection items, Control frm)
    {
        foreach (ToolStripItem item in items)
        {
            if (item is ToolStripMenuItem item1)
            {
                item.Text = ObtenerTexto_013AL(frm.Name + "." + item.Name);
                CambiarIdiomaMenuStrip_013AL(item1.DropDownItems, frm);
            }
        }
    }
}

