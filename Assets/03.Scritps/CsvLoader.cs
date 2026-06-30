using DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Limbus.DataLayer
{
    public class CsvLoader : MonoBehaviour
    {
        private const string UNIT_TABLE_PATH = "DataTable/Unit_Table.csv";
        private const string CARD_TABLE_PATH = "DataTable/Card_Table.csv";
        private const string COIN_TABLE_PATH = "DataTable/Coin_Table.csv";
        private const string EFFECT_TABLE_PATH = "DataTable/Effect_Table.csv";

        // 구조체 저장 딕셔너리
        public Dictionary<string, UnitData> UnitDictionary = new Dictionary<string, UnitData>();
        public Dictionary<string, CardData> CardDictionary = new Dictionary<string, CardData>();
        public Dictionary<string, CoinData> CoinDictionary = new Dictionary<string, CoinData>();
        public Dictionary<string, EffectData> EffectDictionary = new Dictionary<string, EffectData>();

        private string[] _filePath = new string[4];

        private void Awake()
        {
            SetPath();
        }

        private void Start()
        {
            LoadUnitData();
            LoadCardData();
            LoadCoinData();
            LoadEffectData();
        }

        private void SetPath()
        {
            _filePath[0] = Path.Combine(Application.streamingAssetsPath, UNIT_TABLE_PATH);
            _filePath[1] = Path.Combine(Application.streamingAssetsPath, CARD_TABLE_PATH);
            _filePath[2] = Path.Combine(Application.streamingAssetsPath, COIN_TABLE_PATH);
            _filePath[3] = Path.Combine(Application.streamingAssetsPath, EFFECT_TABLE_PATH);
        }

        private void LoadCsvBase(string path, int expectedColumns, Action<string[]> onParseRow)
        {
            if (!File.Exists(path)) return;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string csvText = reader.ReadToEnd();
                    string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 2; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].Replace(",", "").Trim().Length == 0) continue;
                        string[] values = lines[i].Split(',');
                        if (values.Length < expectedColumns) continue;

                        try { onParseRow(values); }
                        catch (Exception ex) { Debug.LogError($"[파싱 에러!] {i + 1}행 오류: {ex.Message}"); }
                    }
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }

        private void LoadUnitData()
        {
            LoadCsvBase(_filePath[0], 9, (values) =>
            {
                UnitData newData = new UnitData(
                    id: values[0].Trim(),
                    type: (UnitType)int.Parse(values[1].Trim()), // Enum 캐스팅
                    name: values[2].Trim(),
                    maxHp: int.Parse(values[3].Trim()),
                    atkLevel: int.Parse(values[4].Trim()),
                    defLevel: int.Parse(values[5].Trim()),
                    resistPierce: float.Parse(values[6].Trim()),
                    resistSlash: float.Parse(values[7].Trim()),
                    resistBlunt: float.Parse(values[8].Trim())
                );
                UnitDictionary.Add(newData.Id, newData);
            });
            Debug.Log($"<color=green>Unit 파싱 완료 ({UnitDictionary.Count}개)</color>");

            // --- 추가된 상세 로그 출력 ---
            foreach (UnitData unit in UnitDictionary.Values)
            {
                Debug.Log($"<color=green>[Unit]</color> ID: {unit.Id} | 이름: {unit.Name} | 타입: {unit.Type} | HP: {unit.MaxHp} | 공/방: {unit.AtkLevel}/{unit.DefLevel} | 내성(관/참/타): {unit.ResistPierce:F1}/{unit.ResistSlash:F1}/{unit.ResistBlunt:F1}");
            }
        }

        private void LoadCardData()
        {
            LoadCsvBase(_filePath[1], 12, (values) =>
            {
                CardData newData = new CardData(
                    id: values[0].Trim(),
                    type: (CardType)int.Parse(values[1].Trim()), // Enum 캐스팅
                    name: values[2].Trim(),
                    costPride: int.Parse(values[3].Trim()),
                    basePower: int.Parse(values[4].Trim()),
                    coinCount: int.Parse(values[5].Trim()),
                    coinPower: int.Parse(values[6].Trim()),
                    atkType: (AttackType)int.Parse(values[7].Trim()), // Enum 캐스팅
                    boostType: values[8].Trim(),
                    boostValue: int.Parse(values[9].Trim()),
                    boostMax: int.Parse(values[10].Trim()),
                    description: values[11].Trim()
                );
                CardDictionary.Add(newData.Id, newData);
            });
            Debug.Log($"<color=cyan>Card 파싱 완료 ({CardDictionary.Count}개)</color>");

            // --- 추가된 상세 로그 출력 ---
            foreach (CardData card in CardDictionary.Values)
            {
                Debug.Log($"<color=cyan>[Card]</color> ID: {card.Id} | 이름: {card.Name} | 타입: {card.Type} | 소모: {card.CostPride} | 위력: {card.BasePower} | 코인수: {card.CoinCount}(+{card.CoinPower}) | 속성: {card.AtkType} | 특수: {card.BoostType}({card.BoostValue}씩/최대{card.BoostMax})");
            }
        }

        private void LoadCoinData()
        {
            LoadCsvBase(_filePath[2], 6, (values) =>
            {
                CoinData newData = new CoinData(
                    id: values[0].Trim(),
                    cardId: values[1].Trim(),
                    index: int.Parse(values[2].Trim()),
                    trigger: (TriggerType)int.Parse(values[3].Trim()), // Enum 캐스팅
                    effectDataId: values[4].Trim(),
                    description: values[5].Trim()
                );
                CoinDictionary.Add(newData.Id, newData);
            });
            Debug.Log($"<color=yellow>Coin 파싱 완료 ({CoinDictionary.Count}개)</color>");

            // --- 추가된 상세 로그 출력 ---
            foreach (CoinData coin in CoinDictionary.Values)
            {
                Debug.Log($"<color=yellow>[Coin]</color> ID: {coin.Id} | 부모카드: {coin.CardId} | 순서: {coin.Index} | 발동조건: {coin.Trigger} | 효과: {coin.EffectDataId} | 설명: {coin.Description}");
            }
        }

        private void LoadEffectData()
        {
            LoadCsvBase(_filePath[3], 4, (values) =>
            {
                EffectData newData = new EffectData(
                    id: values[0].Trim(),
                    name: values[1].Trim(),
                    type: (EffectType)int.Parse(values[2].Trim()), // Enum 캐스팅
                    description: values[3].Trim()
                );
                EffectDictionary.Add(newData.Id, newData);
            });
            Debug.Log($"<color=magenta>Effect 파싱 완료 ({EffectDictionary.Count}개)</color>");

            // --- 추가된 상세 로그 출력 ---
            foreach (EffectData effect in EffectDictionary.Values)
            {
                Debug.Log($"<color=magenta>[Effect]</color> ID: {effect.Id} | 이름: {effect.Name} | 타입: {effect.Type} | 설명: {effect.Description}");
            }
        }
    }
}