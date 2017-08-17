using Sitecore.Data.Items;

namespace SATC.SC.Framework.SitecoreHelpers
{

    using Sitecore.Data;

    /// <summary>
    /// This is the base namespace for all SATC SC Framework's helpers methods
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// These are all the general helper methods used withing Sitecore to perform general things e.g. validating item ids etc.
    /// </summary>
    public class StandardHelpers
    {
         /// <summary>
         /// Create the instance of the master database to use within the class.
         /// </summary>
         public Database MasterDatabase;

        /// <summary>
        /// Assign the items to use later in the constructor, currently set the master database
        /// </summary>
        public StandardHelpers()
        {
            MasterDatabase = Sitecore.Configuration.Factory.GetDatabase("master");
        }

        /// <summary>
        /// Use this method to retrieve the sitecore's item ID from the config setting's name. 
        /// The config file will be set in the includes area of the project, typically in the App_config/Include/ area of your project.
        /// The config file will extend the main web config and contains setting in the structure 
        /// &lt;setting name="HomePageID" value="{00000000-0000-0000-0000-000000000000}"/ &gt;
        /// and then you pass in the name and the method will pull out the value and validate it against the current sitecore instance,
        /// checks if the item from the id is in the database before returning back the id, if its not valid it will return a null id.
        /// </summary>
        /// <param name="settingName">
        /// This is the name of the config setting you want to get the value for e.g HomePageID
        /// </param>
        /// <param name="contextItem"></param>
        /// <returns>
        /// This will return the verified GUID from that setting name e.g. {00000000-0000-0000-0000-000000000000}
        /// </returns>
        public virtual ID GetItemIdFromConfig(string settingName , Item contextItem = null)
        {
            //if we have a context item passed in then get the database from the item
            if (contextItem != null)
            {
                MasterDatabase = contextItem.Database;
            }

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
                        var idDatabaseItem = MasterDatabase.GetItem(sitecoreItemId);
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

        /// <summary>
        /// Use this method to retrieve the sitecore's item Template ID from the config setting's name.
        /// The config file will be set in the includes area of the project, typically in the App_config/Include/ area of your project.
        /// The config file will extend the main web config and contains setting in the structure 
        /// &lt;setting name="HomeTemplateID" value="{00000000-0000-0000-0000-000000000000}"/ &gt;
        /// and then you pass in the name and the method will pull out the value and validate it against the current sitecore instance,
        /// once the template is validated, this is returned back (otherwise return a null id) then the item can be retrieved by that template id
        /// </summary>
        /// <param name="settingName">
        /// </param>
        /// <param name="contextItem"></param>
        /// <returns>
        /// </returns>
        public virtual TemplateID GetTemplateIdFromConfig(string settingName, Item contextItem = null)
        {
            //if we have a context item passed in then get the database from the item
            if (contextItem != null)
            {
                MasterDatabase = contextItem.Database;
            }

            //set the default return ID as null
            var itemTemplateId = new TemplateID();
            //check the string value sent in is not null
            if (!string.IsNullOrWhiteSpace(settingName))
            {
                var configSetting = Sitecore.Configuration.Settings.GetSetting(settingName);
                // check if we have any settings
                if (!string.IsNullOrWhiteSpace(configSetting))
                {
                    var sitecoreItemTemplateId = new TemplateID(new ID(configSetting));
                    //validate the template id
                    if (sitecoreItemTemplateId.ID != new TemplateID())
                    {
                        //once our id is validated , check if the template is in the master database
                        var databaseTemplateItem = MasterDatabase.GetTemplate(sitecoreItemTemplateId);
                        //check if the database template is not null then return the validated template id
                        if (databaseTemplateItem != null)
                        {
                            itemTemplateId = sitecoreItemTemplateId;
                        }
                    }
                }
            }
            // return the template id either a new one or a verified one
            return itemTemplateId;
        }
    }
}
