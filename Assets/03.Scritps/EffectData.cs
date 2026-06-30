namespace DataLayer
{
    public enum EffectType
    {
        Buff = 0,   // 아군 유닛에게 이로운 효과
        Debuff = 1, // 피격 유닛에게 해로운 효과
        System = 2  // 시스템적으로 처리할 스탯 가감 효과
    }

    public readonly struct EffectData
    {
        public readonly string Id;
        public readonly string Name;
        public readonly EffectType Type;
        public readonly string Description;

        public EffectData(string id, string name, EffectType type, string description)
        {
            Id = id;
            Name = name;
            Type = type;
            Description = description;
        }
    }
}