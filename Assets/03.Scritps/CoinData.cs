namespace DataLayer
{
    public enum TriggerType
    {
        On_Use = 0,         // 카드 사용 시
        On_Hit = 1,         // 적중 시 (앞/뒷면 무관)
        On_Heads_Hit = 2,   // 앞면 적중 시
        On_Clash_Win = 3    // 합 승리 시
    }

    public readonly struct CoinData
    {
        public readonly string Id;
        public readonly string CardId;
        public readonly int Index;
        public readonly TriggerType Trigger;
        public readonly string EffectDataId;
        public readonly string Description;

        public CoinData(string id, string cardId, int index, TriggerType trigger, string effectDataId, string description)
        {
            Id = id;
            CardId = cardId;
            Index = index;
            Trigger = trigger;
            EffectDataId = effectDataId;
            Description = description;
        }
    }
}