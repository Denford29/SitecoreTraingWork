using Sitecore.Data;
using System;

namespace SATC.Framework.SitecoreHelpers
{
    public class StandardHelper
    {
        /// <summary>
        /// get the sitecore's item ID from the config settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static ID GetItemIDFromConfig(string settingName)
        {
            //set the default return ID as null
            var itemId = ID.Null;
            //check the string value sent in is not null
            if (!string.IsNullOrWhiteSpace(settingName))
            {
                var configSetting = Sitecore.Configuration.Settings.GetSetting(settingName);
                // check if we have any settings
                if (!string.IsNullOrWhiteSpace(configSetting))
                {
                    var sitecoreItemId = new ID(configSetting);
                    //validate the item id
                    if (!ID.IsNullOrEmpty(sitecoreItemId))
                    {
                        //once our id is validated , check if the item is in the master database
                        var masterDatabase = Sitecore.Configuration.Factory.GetDatabase("master");
                        var idDatabaseItem = masterDatabase.GetItem(sitecoreItemId);
                        //check if the database item is not null then set that id to the returned one
                        if(idDatabaseItem != null)
                        {
                            itemId = idDatabaseItem.ID;
                        }
                    }
                }
            }
            // return the id either a null one or a verified one
            return itemId;
        }
    }
}
