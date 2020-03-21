﻿using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AKSoftware.Localization.MultiLanguages
{
    public class LanguageContainerInAssembly : ILanguageContainerService
    {
        private Assembly _resourcesAssembly;

        /// <summary>
        /// Create instance of the container that languages exists in a specific folder, initialized with the sepecific culture
        /// </summary>
        /// <param name="folderName">Folder that contains the language files</param>
        public LanguageContainerInAssembly(Assembly assembly, CultureInfo culture)
        {
            _resourcesAssembly = assembly;
            SetLanguage(culture, true);
        }

        /// <summary>
        /// Create instance of the container that languages exists in a specific folder, initialized with the default culture
        /// </summary>
        /// <param name="folderName">Folder that contains the language files</param>
        public LanguageContainerInAssembly(Assembly assembly)
        {
            _resourcesAssembly = assembly;
            SetLanguage(CultureInfo.CurrentCulture, true);
        }

        /// <summary>
        /// Keys of the language values
        /// </summary>
        public Keys Keys { get; private set; }

        /// <summary>
        /// Current Culture related to the selected language
        /// </summary>
        public CultureInfo CurrentCulture { get; private set; }

        /// <summary>
        /// Set language manually based on a specific culture
        /// </summary>
        /// <param name="cultureName">The required culture</param>
        /// <param name="isDefault">To indicates if this is the initial function</param>
        /// <exception cref="FileNotFoundException">If there is no culture file exists in the resoruces folder</exception>
        private void SetLanguage(CultureInfo culture, bool isDefault)
        {
            CurrentCulture = culture;
            string[] languageFileNames = _resourcesAssembly.GetManifestResourceNames().Where(s => s.Contains("Resources") && s.Contains(".yml") && s.Contains("-")).ToArray();

            Keys = GetKeysFromCulture(culture.Name, languageFileNames.SingleOrDefault(n => n.Contains($"{culture.Name}.yml")));

            if (Keys == null && culture.Name != "en-US")
                Keys = GetKeysFromCulture("en-US", languageFileNames.SingleOrDefault(n => n.Contains($"en-US.yml")));

            if(Keys == null)
                Keys = GetKeysFromCulture("en-US", languageFileNames.FirstOrDefault());

            if (Keys == null)
                throw new FileNotFoundException($"There is no language files existing the Resource folder within '{_resourcesAssembly.GetName().Name}' assembly");
        }

        /// <summary>
        /// Set language manually based on a specific culture
        /// </summary>
        /// <param name="cultureName">The required culture</param>
        /// <exception cref="FileNotFoundException">If the required culture langage file is not exist</exception>
        public void SetLanguage(CultureInfo culture)
        {
            CurrentCulture = culture;
            string fileName = $"{_resourcesAssembly.GetName().Name}.Resources.{culture.Name}.yml";

            Keys = GetKeysFromCulture(culture.Name, fileName);

            if(Keys == null)
                throw new FileNotFoundException($"There is no language files for '{culture.Name}' existing in the Resources folder within '{_resourcesAssembly.GetName().Name}' assembly");
        }

        private Keys GetKeysFromCulture(string culture, string fileName)
        {
            try
            {
                // Read the file 
                using (var fileStream = _resourcesAssembly.GetManifestResourceStream(fileName))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        return new Keys(streamReader.ReadToEnd());
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
