using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Controls;

namespace StardewValleyManager.Services;

public class SettingsService
{

    private XmlDocument settingsFile;

    private string settingsFileName = "settings.xml";

    private XmlNode? settingsRoot;

    public SettingsService()
    {
        settingsFile = new XmlDocument();

        string settingsFileLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), settingsFileName);
        settingsFile.Load(settingsFileLocation);

        settingsRoot = settingsFile.SelectSingleNode("settings");

        LoadPlatformSaveLocation();
    }

    private void LoadPlatformSaveLocation()
    {
        string path = GetSettingsValue("savesLocation");
        if (path.Length == 0)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley/Saves");
                path = path.Replace("\\", "/");
                UpdateSettingsValue("savesLocation", path, true);
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // TODO
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // TODO
            }
        }
    }

    public string GetSettingsValue(string key)
    {
        XmlNode? settingsNode = settingsFile.SelectSingleNode($"settings/{key}");
        if (settingsNode == null)
        {
            return "";
        }

        return settingsNode.InnerText;
    }
    
    public void UpdateSettingsValue(string key, string value, bool saveOnUpdate)
    {
        XmlNode? settingsNode = settingsFile.SelectSingleNode($"settings/{key}");
        if (settingsNode != null)
        {
            settingsNode.InnerText = value;
        }

        if (saveOnUpdate)
        {
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        string settingsFileLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), settingsFileName);
        settingsFile.Save(settingsFileLocation);
    }

}
