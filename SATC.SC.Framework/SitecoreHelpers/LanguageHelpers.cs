using System.Collections.Generic;
using System.Linq;
using SATC.SC.Framework.SitecoreHelpers.Models;
using Sitecore.Globalization;
using Sitecore;
using Sitecore.Links;

namespace SATC.SC.Framework.SitecoreHelpers
{

    /// <summary>
    /// these are all the helpers we use to get anything related to languages
    /// </summary>
    public class LanguageHelpers
    {

        /// <summary>
        /// get all the available languages as language models
        /// </summary>
        /// <returns></returns>
        public virtual List<LanguageModel> GetAllLanguages()
        {
            //initiate the list of language models to return later
            var languageModelsList = new List<LanguageModel>();

            //get all the languages from the database
            var languages = Context.Database.GetLanguages();
            if (languages.Any())
            {
                //create a language model from each language
                foreach (var language in languages)
                {
                    languageModelsList.Add(CreateLanguage(language));
                }
            }

            //return the populated list of language models
            return languageModelsList;
        }

        /// <summary>
        /// get the language model from the current context language
        /// </summary>
        /// <returns></returns>
        public virtual LanguageModel GetActiveLanguageModel()
        {
            //call the create language model with the current context language
            return CreateLanguage(Context.Language);
        }

        /// <summary>
        /// create a language model to display the settings of a sitecore language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public virtual LanguageModel CreateLanguage(Language language)
        {
            //create and return a new language model
            return new LanguageModel
            {
                Name = language.Name,
                NativeName = language.CultureInfo.NativeName,
                Url = GetItemUrlByLanguage(language),
                Icon = string.Concat("/~/icon/", language.GetIcon(Context.Database)),
                TwoLetterCode = language.CultureInfo.TwoLetterISOLanguageName
            };
        }

        /// <summary>
        /// get the url to use by the language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public virtual string GetItemUrlByLanguage(Language language)
        {
            //create a language url from a language 
            using (new LanguageSwitcher(language))
            {
                var options = new UrlOptions
                {
                    AlwaysIncludeServerUrl = true,
                    LanguageEmbedding = LanguageEmbedding.Always,
                    LowercaseUrls = true
                };
                var url = LinkManager.GetItemUrl(Context.Item, options);
                return StringUtil.EnsurePostfix('/', url).ToLower();
            }
        }


    }
}
