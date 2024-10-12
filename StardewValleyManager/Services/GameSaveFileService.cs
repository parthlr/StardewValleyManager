using StardewValleyManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace StardewValleyManager.Services;
public class GameSaveFileService
{

    XmlDocument saveFileDoc;        // Save file
    XmlDocument saveGameInfoDoc;    // SaveGameInfo file

    private string[] farmTypes = { "Standard Farm","Riverland Farm", "Forest Farm", "Hilltop Farm", "Wilderness Farm", "Four Corners Farm", "Beach Farm", "Meadowlands Farm" };

    private string[] seasons = { "Spring", "Summer", "Fall", "Winter" };

    private string[] itemQualities = { "Normal", "Silver", "Gold", "", "Iridium" };

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

    public List<PlayerRelationshipModel> GetPlayerRelationships()
    {
        List<PlayerRelationshipModel> relationshipStatus = new List<PlayerRelationshipModel>();

        XmlNodeList? friendshipData = saveGameInfoDoc.SelectNodes("Farmer/friendshipData/item");

        if (friendshipData == null)
        {
            return relationshipStatus; 
        }

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

            XmlNode? friendshipStatusNode = friendshipItem.SelectSingleNode("value/Friendship/Status");
            if (friendshipStatusNode == null)
            {
                continue;
            }

            PlayerRelationshipModel prm = new PlayerRelationshipModel();
            prm.NpcName = npcNameNode.InnerText;
            prm.FriendshipPoints = Convert.ToInt32(relationshipPointsNode.InnerText);
            prm.FriendshipStatus = friendshipStatusNode.InnerText;

            relationshipStatus.Add(prm);
        }

        return relationshipStatus;
    }

    public int GetPlayerFarmingLevel()
    {
        XmlNode? farmingLevelNode = saveGameInfoDoc.SelectSingleNode("Farmer/farmingLevel");
        if (farmingLevelNode == null)
        {
            return 0;
        }

        int farmingLevel = Convert.ToInt32(farmingLevelNode.InnerText);
        return farmingLevel;
    }

    public int GetPlayerMiningLevel()
    {
        XmlNode? miningLevelNode = saveGameInfoDoc.SelectSingleNode("Farmer/miningLevel");
        if (miningLevelNode == null)
        {
            return 0;
        }

        int miningLevel = Convert.ToInt32(miningLevelNode.InnerText);
        return miningLevel;
    }

    public int GetPlayerCombatLevel()
    {
        XmlNode? combatLevelNode = saveGameInfoDoc.SelectSingleNode("Farmer/combatLevel");
        if (combatLevelNode == null)
        {
            return 0;
        }

        int combatLevel = Convert.ToInt32(combatLevelNode.InnerText);
        return combatLevel;
    }

    public int GetPlayerForagingLevel()
    {
        XmlNode? foragingLevelNode = saveGameInfoDoc.SelectSingleNode("Farmer/foragingLevel");
        if (foragingLevelNode == null)
        {
            return 0;
        }

        int foragingLevel = Convert.ToInt32(foragingLevelNode.InnerText);
        return foragingLevel;
    }

    public int GetPlayerFishingLevel()
    {
        XmlNode? fishingLevelNode = saveGameInfoDoc.SelectSingleNode("Farmer/fishingLevel");
        if (fishingLevelNode == null)
        {
            return 0;
        }

        int fishingLevel = Convert.ToInt32(fishingLevelNode.InnerText);
        return fishingLevel;
    }

    public List<InventoryItemModel> GetPlayerInventory()
    {
        List<InventoryItemModel> inventory = new List<InventoryItemModel>();

        XmlNodeList? inventoryData = saveGameInfoDoc.SelectNodes("Farmer/items/Item");

        if (inventoryData == null)
        {
            return inventory;
        }

        foreach (XmlNode inventoryItem in inventoryData)
        {
            string itemName = "";
            int itemStackCount = 0;
            string itemQuality = "Normal";
            string itemPrice = "-";

            XmlAttributeCollection? itemAttributes = inventoryItem.Attributes;
            if (itemAttributes != null)
            {
                bool isNullItem = false;

                foreach (XmlAttribute itemAttribute in itemAttributes)
                {
                    if (itemAttribute.Name.Equals("xsi:nil") && itemAttribute.Value.Equals("true"))
                    {
                        isNullItem = true;
                        break;
                    }
                }

                if (isNullItem)
                {
                    continue;
                }
            }

            XmlNode? itemNameNode = inventoryItem.SelectSingleNode("name");
            if (itemNameNode != null)
            {
                itemName = itemNameNode.InnerText;
            }

            XmlNode? itemStackNode = inventoryItem.SelectSingleNode("stack");
            if (itemStackNode != null)
            {
                itemStackCount = Convert.ToInt32(itemStackNode.InnerText);
            }

            XmlNode? itemQualityNode = inventoryItem.SelectSingleNode("quality");
            if (itemQualityNode != null)
            {
                int itemQualityIndex = Convert.ToInt32(itemQualityNode.InnerText);
                itemQuality = itemQualities[itemQualityIndex];
            }

            XmlNode? itemPriceNode = inventoryItem.SelectSingleNode("price");
            if (itemPriceNode != null)
            {
                itemPrice = itemPriceNode.InnerText;
            }

            XmlNode? itemIdNode = inventoryItem.SelectSingleNode("itemId");
            if (itemIdNode != null)
            {
                string itemId = itemIdNode.InnerText;
                int itemIdInt;

                bool isItemIdInt = int.TryParse(itemId, out itemIdInt);

                if (!isItemIdInt)
                {
                    itemName = Regex.Replace(itemId, "(\\B[A-Z])", " $1");
                }
            }

            InventoryItemModel item = new InventoryItemModel();
            item.Name = itemName;
            item.StackCount = itemStackCount;
            item.Quality = itemQuality;
            item.Price = itemPrice;

            inventory.Add(item);
        }

        return inventory;
    }
}
