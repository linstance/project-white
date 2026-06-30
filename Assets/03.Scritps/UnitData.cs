

namespace DataLayer
{
    public enum UnitType
    {
        Player = 0,
        Enemy = 1,
        Boss = 2
    }

    public readonly struct UnitData
    {
        public readonly string Id;
        public readonly UnitType Type;
        public readonly string Name;
        public readonly int MaxHp;
        public readonly int AtkLevel;
        public readonly int DefLevel;
        public readonly float ResistPierce;
        public readonly float ResistSlash;
        public readonly float ResistBlunt;

        public UnitData(string id, UnitType type, string name, int maxHp, int atkLevel, int defLevel,
                        float resistPierce, float resistSlash, float resistBlunt)
        {
            Id = id;
            Type = type;
            Name = name;
            MaxHp = maxHp;
            AtkLevel = atkLevel;
            DefLevel = defLevel;
            ResistPierce = resistPierce;
            ResistSlash = resistSlash;
            ResistBlunt = resistBlunt;
        }
    }
}