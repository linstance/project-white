using System;
using System.Collections.Generic;

namespace CoreModelLayer
{
    public class BattleContext
    {
        public int CurrentPride { get; private set; }                                   // 현재 플레이어의 오만(코스트) 정보를 담고 있는 프로퍼티 
        public List<Card> Deck { get; private set; } = new List<Card>();                // 플레이어의 덱 정보를 담당하는 리스트
        public List<Card> Hand { get; private set; } = new List<Card>();                // 플레이어의 손패 정보를 담당하는 리스트
        
        public event Action<int> OnPrideChanged;                                        // 플레이어의 오만(코스트)를 변경하는 이벤트
        public event Action<Card> OnCardDrawn;                                          // 플레이어어의 손패에 덱에서 카드를 추가하는 이벤트

        public void AddPride(int amount)
        {
            CurrentPride += amount;
            OnPrideChanged?.Invoke(CurrentPride);
        }
        
        public void DrawCard()
        {
            if (Deck.Count == 0) return; 

            Card drawn = Deck[0];
            Deck.RemoveAt(0);
            Hand.Add(drawn);
            
            // UI 매니저에게 알림
            OnCardDrawn?.Invoke(drawn); 
        }
        
    }
}
