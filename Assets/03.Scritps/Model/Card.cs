using DataLayer;

namespace CoreModelLayer
{
    public class Card
    {
        public CardData OriginData { get; private set; }

        public int ModifildePride;               // 카드 코스트 값 증감을 위해 사용되는 변수

        public Card(CardData data)
        {
            data = OriginData;

            ModifildePride = data.CostPride;
        }

        public int GetRequiredPride()                // 현재 사용되는 코스트(오만)을 반환하는 함수
        {
            return ModifildePride;
        }
    }    
}

