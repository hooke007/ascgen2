//---------------------------------------------------------------------------------------
// <copyright file="Resource.cs" company="Jonathan Mathews Software">
//     ASCII Generator dotNET - Image to ASCII Art Conversion Program
//     Copyright (C) 2009 Jonathan Mathews Software. All rights reserved.
// </copyright>
// <author>Jonathan Mathews</author>
// <email>info@jmsoftware.co.uk</email>
// <email>jmsoftware@gmail.com</email>
// <website>http://www.jmsoftware.co.uk/</website>
// <website>http://ascgen2.sourceforge.net/</website>
// <license>
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the license, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
// </license>
//---------------------------------------------------------------------------------------
namespace JMSoftware.AsciiGeneratorDotNet
{
    using System;
    using System.Collections.Generic;
    using System.Resources;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// Class to handle accessing of the programs resources
    /// </summary>
    public abstract class Resource
    {
        #region�Fields

        /// <summary>
        /// The root location of the localization resources
        /// </summary>
        private static string location = "AscGenDotNet.Resources.Localization.Localization";

        /// <summary>
        /// Have we attempted to load the translation file?
        /// </summary>
        private static bool translationFileChecked = false;

        /// <summary>
        /// The translations
        /// </summary>
        private static Dictionary<string, string> translations = null;

        #endregion�Fields

        #region�Properties

        /// <summary>
        /// Gets or sets the root location of the localization resources
        /// </summary>
        public static string Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we have attempted to load the resource file.
        /// </summary>
        /// <value>
        /// <c>true</c> if translation file has been checked; otherwise, <c>false</c>.
        /// </value>
        public static bool TranslationFileChecked
        {
            get
            {
                return translationFileChecked;
            }

            set
            {
                translationFileChecked = value;
            }
        }

        /// <summary>Gets the translated strings</summary>
        public static Dictionary<string, string> Translations
        {
            get
            {
                if (translations != null)
                {
                    return translations;
                }

                translations = new Dictionary<string, string>();

                if (TranslationFileChecked)
                {
                    return translations;
                }

                XmlDocument doc = new XmlDocument();

                try
                {
                    doc.Load(Settings.Default.TranslationFile);

                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        translations.Add(node.Attributes[0].InnerText, node.InnerText);
                    }
                }
                catch (XmlException ex)
                {
                    MessageBox.Show(ex.Message, string.Format(GetString("Error with settings file '{0}'"), Settings.Default.TranslationFile), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.IO.FileNotFoundException)
                {
                    if (Settings.Default.TranslationFile != "translation.xml")
                    {
                        MessageBox.Show(
                                    string.Format(
                                            GetString("Could not load translation file '{0}'"), Settings.Default.TranslationFile),
                                            GetString("Error"),
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                    }
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    TranslationFileChecked = true;
                }

                return translations;
            }
        }

        #endregion�Properties

        #region�Public�methods

        /// <summary>
        /// Get the string named 'key' from the resource file
        /// </summary>
        /// <param name="key">Name of the string to return</param>
        /// <returns>Specified string value from the resource file</returns>
        public static string GetString(string key)
        {
            ResourceManager resourceManager = new ResourceManager(
                                                        Location,
                                                        System.Reflection.Assembly.GetExecutingAssembly());

            if (Translations.ContainsKey(key))
            {
                return Translations[key];
            }

            string value = resourceManager.GetString(key, Settings.Default.Culture);

            if (value == null || value.Length == 0)
            {
                // TODO: Log the missing value
                value = key;
            }

            return value;
        }

        #endregion�Public�methods
    }
}