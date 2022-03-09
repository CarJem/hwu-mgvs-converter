using System;

namespace GvasFormat.Serialization
{
    public class CustomFormatDataEntry
    {

        public CustomFormatDataEntry() { }

        public CustomFormatDataEntry(Guid id, int value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id;
        public int Value;
    }
}