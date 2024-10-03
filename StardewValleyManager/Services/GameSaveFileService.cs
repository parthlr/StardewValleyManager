using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StardewValleyManager.Services;
public class GameSaveFileService
{

    XmlDocument saveFileDoc;        // Save file
    XmlDocument saveGameInfoDoc;    // SaveGameInfo file

    private string[] farmTypes = { "Standard Farm","Riverland Farm", "Forest Farm", "Hilltop Farm", "Wilderness Farm", "Four Corners Farm", "Beach Farm", "Meadowlands Farm" };

    private string[] seasons = { "Spring", "Summer", "Fall", "Winter" };

    public GameSaveFileService()
    {
        saveFileDoc = new XmlDocument();
        saveGameInfoDoc = new XmlDocument();
    }

    public void LoadSaveGameInfoXML(string XMLContent)
    {
        saveGameInfoDoc.LoadXml(XMLContent);
    }

    public void LoadSaveFile(string SavePath)
    {
        saveFileDoc.Load(SavePath);
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
        XmlNode? farmType = saveFileDoc.SelectSingleNode("SaveGame/whichFarm");
        if (farmType == null)
        {
            return "Unknown";
        }
        int farmIndex = Convert.ToInt32(farmType.InnerText);
        return farmTypes[farmIndex];
    }

    public int GetPlayerMoney()
    {
        XmlNode? playerMoney = saveGameInfoDoc.SelectSingleNode("Farmer/money");
        if (playerMoney == null)
        {
            return 0;
        }
        int playerMoneyValue = Convert.ToInt32(playerMoney.InnerText);
        return playerMoneyValue;
    }

    public int GetTotalEarnedMoney()
    {
        XmlNode? earnedMoney = saveGameInfoDoc.SelectSingleNode("Farmer/totalMoneyEarned");
        if (earnedMoney == null)
        {
            return 0;
        }
        int totalEarnedMoney = Convert.ToInt32(earnedMoney.InnerText);
        return totalEarnedMoney;
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

    public Dictionary<string, int> GetPlayerRelationships()
    {
        Dictionary<string, int> relationshipStatus = new Dictionary<string, int>();

        XmlNodeList? friendshipData = saveGameInfoDoc.SelectNodes("Farmer/friendshipData");

        foreach (XmlNode friendshipItem in friendshipData)
        {
            XmlNode? npcNameNode = friendshipItem.SelectSingleNode("key/string");
            if (npcNameNode == null)
            {
                continue;
            }

            XmlNode? relationshipPointsNode = friendshipItem.SelectSingleNode("value/Friendship/Points");
            if (relationshipPointsNode == null)
            {
                continue;
            }

            relationshipStatus[npcNameNode.InnerText] = Convert.ToInt32(relationshipPointsNode.InnerText);
        }

        return relationshipStatus;
    }

}
