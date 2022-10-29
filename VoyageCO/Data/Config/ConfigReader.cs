using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using VCO.Utility;
using SFML.System;

namespace VCO.Data.Config
{
    internal class ConfigReader
    {
        private const string TAG_NAME_START = "start";
        private const string TAG_NAME_START_MAP = "map";
        private const string TAG_NAME_START_POSITION = "position";
        private const string TAG_NAME_PARTY = "party";
        private const string TAG_NAME_PARTY_CHARACTER = "character";
        private const string ATTR_NAME_VALUE = "value";
        private const string ATTR_NAME_ID = "id";
        private const string ATTR_NAME_X = "x";
        private const string ATTR_NAME_Y = "y";
        private static ConfigReader instance;
        private Stack<ConfigReader.ReadState> stateStack;
        private string startingMap;
        private string debugMap;
        private Vector2i startingPosition;
        private List<CharacterType> partyList;

        public static ConfigReader Instance
        {
            get
            {
                if (ConfigReader.instance == null)
                    ConfigReader.instance = new ConfigReader();
                return ConfigReader.instance;
            }
        }

        public string StartingMapName => this.startingMap;
        public string DebugMapName => this.debugMap;

        public Vector2i StartingPosition => this.startingPosition;

        public CharacterType[] StartingParty => this.partyList.ToArray();

        public ConfigReader()
        {
            this.stateStack = new Stack<ConfigReader.ReadState>();
            this.stateStack.Push(ConfigReader.ReadState.Root);
            this.partyList = new List<CharacterType>();
            this.Load();
        }

        private void ReadStartElement(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case "map":
                    if (!reader.MoveToNextAttribute() || !(reader.Name == "value"))
                        break;
                    this.startingMap = reader.Value;
                    break;
                case "position":
                    while (reader.MoveToNextAttribute())
                    {
                        switch (reader.Name)
                        {
                            case "x":
                                int.TryParse(reader.Value, out this.startingPosition.X);
                                continue;
                            case "y":
                                int.TryParse(reader.Value, out this.startingPosition.Y);
                                continue;
                            default:
                                continue;
                        }
                    }
                    break;
            }
        }

        private void ReadDebugElement(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case "debugmap":
                    if (!reader.MoveToNextAttribute() || !(reader.Name == "value"))
                        break;
                    this.debugMap = reader.Value;
                    break;
      
            }
        }

        private void ReadPartyElement(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case "character":
                    if (!reader.MoveToNextAttribute() || !(reader.Name == "id"))
                        break;
                    switch (reader.Value)
                    {
                        case "travis":
                            this.partyList.Add(CharacterType.Travis);
                            return;
                        case "floyd":
                            this.partyList.Add(CharacterType.Floyd);
                            return;
                        case "meryl":
                            this.partyList.Add(CharacterType.Meryl);
                            return;
                        case "leo":
                            this.partyList.Add(CharacterType.Leo);
                            return;
                        case "zack":
                            this.partyList.Add(CharacterType.Zack);
                            return;
                        case "renee":
                            this.partyList.Add(CharacterType.Renee);
                            return;
                        case null:
                            return;
                        default:
                            return;
                    }
            }
        }

        private void HandleInnerElement(XmlTextReader reader)
        {
            switch (this.stateStack.Peek())
            {
                case ConfigReader.ReadState.Start:
                    this.ReadStartElement(reader);
                    break;
                case ConfigReader.ReadState.Party:
                    this.ReadPartyElement(reader);
                    break;
                case ConfigReader.ReadState.Debug:
                    ReadDebugElement(reader);
                    break;
            }
        }

        private void HandleElement(XmlTextReader reader)
        {
            switch (reader.Name)
            {
                case "start":
                    this.stateStack.Push(ConfigReader.ReadState.Start);
                    break;
                case "party":
                    this.stateStack.Push(ConfigReader.ReadState.Party);
                    break;
                case "debug":
                    this.stateStack.Push(ConfigReader.ReadState.Debug);
                    break;
                default:
                    this.HandleInnerElement(reader);
                    break;
            }
        }

        private void HandleEndElement(XmlTextReader reader)
        {
            int num = (int)this.stateStack.Pop();
        }

        private void Load()
        {
            using (Stream stream = EmbeddedResources.GetStream("VCO.Resources.config.xml"))
            {
                XmlTextReader reader = new XmlTextReader(stream);
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            this.HandleElement(reader);
                        }
                    }
                }
            }
        }

        private enum ReadState
        {
            Root,
            BaseStats,
            Start,
            Party,
            Debug
        }
    }
}
