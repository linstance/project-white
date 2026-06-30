

namespace DataLayer
{
    public enum CardType
    {
        Common = 0,            // 공통
        WhiteFixer = 1,        // 하얀 해결사
        RedFixer = 2,          // 붉은 해결사
        BlackFixer = 3,        // 검은 해결사
        BlueFixer = 4          // 푸른 해결사
    }

    public enum AttackType
    {
        Pierce = 0,    // 관통
        Slash = 1,     // 참격
        Blunt = 2      // 타격
    }

    public readonly struct CardData
    {
        public readonly string Id;
        public readonly CardType Type;          // Enum (공통/해결사 종류)
        public readonly string Name;
        public readonly int CostPride;
        public readonly int BasePower;
        public readonly int CoinCount;
        public readonly int CoinPower;
        public readonly AttackType AtkType;     // Enum (관/참/타)

        public readonly string BoostType;
        public readonly int BoostValue;
        public readonly int BoostMax;
        public readonly string Description;

        // 생성자 (CsvLoader에서 이 구조체로 데이터를 넘길 때 사용)
        public CardData(string id, CardType type, string name, int costPride, int basePower,
                        int coinCount, int coinPower, AttackType atkType,
                        string boostType, int boostValue, int boostMax, string description)
        {
            Id = id;
            Type = type;
            Name = name;
            CostPride = costPride;
            BasePower = basePower;
            CoinCount = coinCount;
            CoinPower = coinPower;
            AtkType = atkType;
            BoostType = boostType;
            BoostValue = boostValue;
            BoostMax = boostMax;
            Description = description;
        }
    }
}

