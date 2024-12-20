﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Barotrauma
{
    class CharacterPrefab : PrefabWithUintIdentifier, IImplementsVariants<CharacterPrefab>
    {
        public readonly static PrefabCollection<CharacterPrefab> Prefabs = new PrefabCollection<CharacterPrefab>();

        public override void Dispose()
        {
            Character.RemoveByPrefab(this);
        }

        public string Name => Identifier.Value;
        public Identifier VariantOf { get; }
        public CharacterPrefab ParentPrefab { get; set; }
        
        public Identifier GetBaseCharacterSpeciesName(Identifier speciesName)
        {
            if (!VariantOf.IsEmpty)
            {
                speciesName = VariantOf;
                if (ParentPrefab is { VariantOf.IsEmpty: false } parentPrefab)
                {
                    speciesName = parentPrefab.GetBaseCharacterSpeciesName(speciesName);
                }   
            }
            return speciesName;
        }

        public bool HasCharacterInfo { get; private set; }
        
        public Identifier Group { get; private set; }
        
        public bool MatchesSpeciesNameOrGroup(Identifier speciesNameOrGroup) => Identifier == speciesNameOrGroup || Group == speciesNameOrGroup;

        public void InheritFrom(CharacterPrefab parent)
        {
            ConfigElement = CharacterParams.CreateVariantXml(originalElement, parent.ConfigElement).FromPackage(ConfigElement.ContentPackage);
            ParseConfigElement();
        }

        private void ParseConfigElement()
        {
            var headsElement = ConfigElement.GetChildElement("Heads");
            var varsElement = ConfigElement.GetChildElement("Vars");
            var menuCategoryElement = ConfigElement.GetChildElement("MenuCategory");
            var pronounsElement = ConfigElement.GetChildElement("Pronouns");

            HasCharacterInfo = headsElement != null || ConfigElement.GetAttributeBool(nameof(HasCharacterInfo), false);
            if (HasCharacterInfo)
            {
                CharacterInfoPrefab = new CharacterInfoPrefab(this, headsElement, varsElement, menuCategoryElement, pronounsElement);
            }
            Group = ConfigElement.GetAttributeIdentifier(nameof(Group), Identifier.Empty);
        }

        private readonly ContentXElement originalElement;
        public ContentXElement ConfigElement { get; private set; }

        public CharacterInfoPrefab CharacterInfoPrefab { get; private set; }

        public static IEnumerable<ContentXElement> ConfigElements => Prefabs.Select(p => p.ConfigElement);

        public static readonly Identifier HumanSpeciesName = "human".ToIdentifier();
        public static readonly Identifier HumanGroup = "human".ToIdentifier();

        public static CharacterFile HumanConfigFile => HumanPrefab.ContentFile as CharacterFile;
        public static CharacterPrefab HumanPrefab => FindBySpeciesName(HumanSpeciesName);

        public static CharacterPrefab FindBySpeciesName(Identifier speciesName)
        {
            if (!Prefabs.ContainsKey(speciesName)) { return null; }
            return Prefabs[speciesName];
        }

        public static CharacterPrefab FindByFilePath(string filePath)
        {
            return Prefabs.Find(p => p.ContentFile.Path == filePath);
        }

        public static CharacterPrefab Find(Predicate<CharacterPrefab> predicate)
        {
            return Prefabs.Find(predicate);
        }

        public CharacterPrefab(ContentXElement mainElement, CharacterFile file) : base(file, ParseName(mainElement, file))
        {
            originalElement = mainElement;
            ConfigElement = mainElement;
            VariantOf = mainElement.VariantOf();

            ParseConfigElement();
        }

        public static Identifier ParseName(XElement element, CharacterFile file)
        {
            string name = element.GetAttributeString("name", null);
            if (!string.IsNullOrEmpty(name))
            {
                DebugConsole.NewMessage($"Error in {file.Path}: 'name' is deprecated! Use 'speciesname' instead.", Color.Orange);
            }
            else
            {
                name = element.GetAttributeString("speciesname", string.Empty);
            }
            return new Identifier(name);
        }

        public static bool CheckSpeciesName(XElement mainElement, CharacterFile file, out Identifier name)
        {
            name = ParseName(mainElement, file);
            if (name == Identifier.Empty)
            {
                DebugConsole.ThrowError($"No species name defined for: {file.Path}",
                    contentPackage: file.ContentPackage);
                return false;
            }
            return true;
        }
    }
}
