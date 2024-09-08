using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StardewValleyManager.Services;
public class SaveFileService
{

    XmlDocument doc = new XmlDocument();
    XmlDocument saveGameInfoDoc = new XmlDocument();

    private string[] farmTypes = { "Standard Farm","Riverland Farm", "Forest Farm", "Hilltop Farm", "Wilderness Farm", "Four Corners Farm", "Beach Farm", "Meadowlands Farm" };

    private string[] seasons = { "Spring", "Summer", "Fall", "Winter" };

    public SaveFileService()
    {
        
    }

    public void LoadSaveGameInfoXML(string XMLContent)
    {
        saveGameInfoDoc.LoadXml(XMLContent);
    }

    public void LoadSaveFile(string SavePath)
    {
        doc.Load(SavePath);
    }

    public void LoadSaveGameInfoFile(string SavePath)
    {
        saveGameInfoDoc.Load(SavePath);
    }

    public string GetPlayerName()
    {
        XmlNode? farmerName = saveGameInfoDoc.SelectSingleNode("Farmer/name");
        if (farmerName == null)
        {
            return "Unknown";
        }
        return farmerName.InnerText;
    }

    public string GetFarmType()
    {
        XmlNode? farmType = doc.SelectSingleNode("SaveGame/whichFarm");
        if (farmType == null)
        {
            return "Unknown";
        }
        int farmIndex = Convert.ToInt32(farmType.InnerText);
        return farmTypes[farmIndex];
    }

    public string GetYear()
    {
        XmlNode? year = saveGameInfoDoc.SelectSingleNode("Farmer/yearForSaveGame");
        if (year == null)
        {
            return "Unknown";
        }
        return year.InnerText;
    }

    public string GetSeason()
    {
        XmlNode? season = saveGameInfoDoc.SelectSingleNode("Farmer/seasonForSaveGame");
        if (season == null)
        {
            return "Unknown";
        }
        int seasonIndex = Convert.ToInt32(season.InnerText);
        return seasons[seasonIndex];
    }

    public string GetDay()
    {
        XmlNode? day = saveGameInfoDoc.SelectSingleNode("Farmer/dayOfMonthForSaveGame");
        if (day == null)
        {
            return "Unknown";
        }
        return day.InnerText;
    }

}
