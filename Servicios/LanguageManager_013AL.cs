using BE;
using Servicios;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

public class LanguageManager_013AL : ISubject_013AL
{
    private List<IObserver_013AL> ListaFormularios_013AL = new List<IObserver_013AL>();
    private Dictionary<string, string> Diccionario_013AL = new Dictionary<string, string>();

    // 🔹 Delegado para obtener traducciones desde la capa BLL
    public Func<int, List<Traduccion_013AL>> ObtenerTraduccionesPorIdioma { get; set; }

    // 🔹 Singleton
    private static LanguageManager_013AL instancia_013AL;
    private LanguageManager_013AL() { }

    public static LanguageManager_013AL ObtenerInstancia_013AL()
    {
        if (instancia_013AL == null)
            instancia_013AL = new LanguageManager_013AL();

        return instancia_013AL;
    }

    // === Variables internas de idioma ===
    private int idIdiomaActual_013AL = IDIOMA_ESPANOL;
    public const int IDIOMA_ESPANOL = 1; // o el ID que tengas en la BD para español
    public int IdiomaActual_013AL
    {
        get { return idIdiomaActual_013AL; }
    }
    // === Métodos del patrón Observer ===
    public void Agregar_013AL(IObserver_013AL observer)
    {
        if (!ListaFormularios_013AL.Contains(observer))
            ListaFormularios_013AL.Add(observer);
    }

    public void Quitar_013AL(IObserver_013AL observer)
    {
        ListaFormularios_013AL.Remove(observer);
    }

    public void Notificar_013AL()
    {
        foreach (IObserver_013AL observer in ListaFormularios_013AL)
            observer.ActualizarIdioma_013AL();
    }

    // === Cambio de idioma ===
    public void CambiarIdioma_013AL(int nuevoIdIdioma)
    {
        idIdiomaActual_013AL = nuevoIdIdioma;
        CargarIdioma_013AL();
        Notificar_013AL();
    }

    // === Carga de idioma desde BD ===
    public void CargarIdioma_013AL()
    {
        Diccionario_013AL.Clear();

        if (ObtenerTraduccionesPorIdioma != null /*&& idIdiomaActual_013AL != IDIOMA_ESPANOL*/)
        {
            var traducciones = ObtenerTraduccionesPorIdioma(idIdiomaActual_013AL);

            foreach (var t in traducciones)
            {
                string clave = t.Etiqueta_013AL.Nombre_013AL; // viene de la tabla Etiqueta
                if (!Diccionario_013AL.ContainsKey(clave))
                    Diccionario_013AL.Add(clave, t.Texto_013AL);
            }
        }
    }

    // === Obtener texto traducido ===
    public string ObtenerTexto_013AL(string key)
    {
        return Diccionario_013AL.ContainsKey(key)
            ? Diccionario_013AL[key]
            : key;
    }

    // === Aplicar idioma a los controles ===
    public void CambiarIdiomaControles_013AL(Control frm)
    {
        try
        {
            frm.Text = ObtenerTexto_013AL(frm.Name + ".Text");

            foreach (Control c in frm.Controls)
            {
                if (c is Button || c is Label || c is RadioButton || c is CheckBox)
                    c.Text = ObtenerTexto_013AL(frm.Name + "." + c.Name);

                if (c is MenuStrip m)
                {
                    foreach (ToolStripMenuItem item in m.Items)
                    {
                        item.Text = ObtenerTexto_013AL(frm.Name + "." + item.Name);
                        CambiarIdiomaMenuStrip_013AL(item.DropDownItems, frm);
                    }
                }

                if (c.Controls.Count > 0)
                    CambiarIdiomaControles_013AL(c);
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