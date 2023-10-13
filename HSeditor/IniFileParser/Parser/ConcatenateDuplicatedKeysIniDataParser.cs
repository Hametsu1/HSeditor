using HSeditor.Model;
using HSeditor.Model.Configuration;

namespace HSeditor.Parser
{

    public class ConcatenateDuplicatedKeysIniDataParser : IniDataParser
    {
        public new ConcatenateDuplicatedKeysIniParserConfiguration Configuration
        {
            get
            {
                return (ConcatenateDuplicatedKeysIniParserConfiguration)base.Configuration;
            }
            set
            {
                base.Configuration = value;
            }
        }

        public ConcatenateDuplicatedKeysIniDataParser()
            : this(new ConcatenateDuplicatedKeysIniParserConfiguration())
        { }

        public ConcatenateDuplicatedKeysIniDataParser(ConcatenateDuplicatedKeysIniParserConfiguration parserConfiguration)
            : base(parserConfiguration)
        { }

        protected override void HandleDuplicatedKeyInCollection(string key, string value, KeyDataCollection keyDataCollection, string sectionName)
        {
            keyDataCollection[key] += Configuration.ConcatenateSeparator + value;
        }
    }

}
