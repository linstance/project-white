using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    public string Unit_Id;              // 유닛 아이디 string
    public int Unit_Type;               // 유닛 타입 int로 받아서 enum으로 캐스팅 예정 -> 0:Player, 1:Enemy, 2:Boss
    public string Unit_Name;            // 유닛 이름
    public int Unit_MaxHp;              // 유닛 최대 체력
    public int Unit_AtkLevel;           // 유닛 공격 레벨
    public int Unit_DefLevel;           // 유닛 방어 레벨

    public float Unit_ResistPierce;     // 유닛 관통 약점 배율 1.0 ~ 1.5
    public float Unit_ResistSlash;      // 유닛 참격 약점 배율 1.0 ~ 1.5
    public float Unit_ResistBlunt;      // 유닛 타격 약점 배율 1.0 ~ 1.5
}

public class CardData
{
    public string Card_Id;              // 카드 ID 값
    public int Card_Type;               // 카드의 속성 int로 받아서 enum으로 캐스팅 예정
    public string Card_Name;            // 카드 이름
    public int Card_CostPride;          // 카드 소모값
    public int Card_BasePower;          // 카드 기본 값
    public int Card_CoinCount;          // 카드 코인 갯수
    public int Card_CoinPower;          // 코인이 앞면일때 코인 갯수 당 증가할 위력
    public int Card_AtkType;            // 카드의 공격 유형 int로 받아서 enum으로 캐스팅 예정
    
    public string Card_BoostType;       // 코인이 앞면 판정일 때, 추가 위력 증가 조건의 ID를 저장하는 부분
    public int Card_BoostValue;         // 코인이 앞면 판정일때 앞면인 코인 갯수 별로 증가될 카드 위력
    public int Card_BoostMax;           // 코인이 앞면 판정일때 증가 할수 있는 최대 위력
    public string Card_Description;     // UI에 표시될 카드 설명
}

public class CoinData
{
    public string Coin_Id;              // 코인 Id 값
    public string Coin_CardId;          // 코인이 속해 있는 카드 Id 값
    public int Coin_Index;              // 코인 순서 어떤 카드의 몇번째 코인인지
    
    public int Coin_TriggerType;        // 코인 효과 발동 속성 값 int로 받아서 enum으로 캐스팅 예정
                                        // 0이면 On_Use (카드 사용 시)
                                        // 1이면 On_Hit (적중 시 - 앞/뒷면 상관없이 무조건)
                                        // 2이면 On_Heads_Hit (앞면 적중 시)
                                        // 3이면 On_Clash_Win (합 승리 시)
                                    
    public string Coin_EffectData;      // 코인 별로 판정이 True 일때 부여할 효과 Id
    public string Coin_Description;     // 코인 별 설명
}

public class EffectData
{
    public string Effect_Id;            // 이펙트 ID 값
    public string Effect_Name;          // 이펙트 이름
    public int Effect_Type;             // 이펙트 타입 속성 값
                                        // 0이면 Buff: 아군 유닛에게 이로운 효과
                                        // 1이면 Debuff: 피격 유닛에게 해로운 효과
                                        // 2이면 System: 해당 유닛에게 이미 부여된 효과 차감 혹은 시스템적으로 처리할 +-  수치 값.
    public string Effect_Description;   // 이펙트 설명
}

public class CsvLoader : MonoBehaviour
{
    // Table Path 상대 경로
    private const string UNIT_TABLE_PATH = "DataTable/Unit_Table.csv";
    private const string CARD_TABLE_PATH = "DataTable/Card_Table.csv";
    private const string COIN_TABLE_PATH = "DataTable/Coin_Table.csv";
    private const string EFFECT_TABLE_PATH = "DataTable/Effect_Table.csv";

    // 파싱된 데이터를 보관할 딕셔너리
    public Dictionary<string, UnitData> UnitDictionary = new Dictionary<string, UnitData>();
    public Dictionary<string, CardData> CardDictionary = new Dictionary<string, CardData>();
    public Dictionary<string, CoinData> CoinDictionary = new Dictionary<string, CoinData>();
    public Dictionary<string, EffectData> EffectDictionary = new Dictionary<string, EffectData>();
    
    // 절대 경로를 저장하는 배열
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
        if (!File.Exists(path))
        {
            Debug.LogError($"[CsvLoader] 파일이 존재하지 않습니다: {path}");
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string csvText = reader.ReadToEnd();
                string[] lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                // i = 2부터 시작 (헤더 및 주석 행 스킵)
                for (int i = 2; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].Replace(",", "").Trim().Length == 0)
                        continue;

                    string[] values = lines[i].Split(',');

                    if (values.Length < expectedColumns)
                    {
                        Debug.LogWarning($"[CsvLoader] {i + 1}번째 행 데이터 부족 (현재 {values.Length}/{expectedColumns}개). 스킵됨.\n내용: {lines[i]}");
                        continue;
                    }

                    try
                    {
                        onParseRow(values);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[파싱 에러!] 엑셀의 {i + 1}번째 행 확인 요망.\n원본: {lines[i]}\n상세: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CsvLoader] {path} 파일 접근 중 치명적인 오류 발생!");
            Debug.LogException(e);
        }
    }


    private void LoadUnitData()
    {
        Debug.Log("<color=green>================ Unit_Table 파싱 시작 ================</color>");

        LoadCsvBase(_filePath[0], 9, (values) =>
        {
            UnitData newData = new UnitData
            {
                Unit_Id = values[0].Trim(),
                Unit_Type = int.Parse(values[1].Trim()),
                Unit_Name = values[2].Trim(),
                Unit_MaxHp = int.Parse(values[3].Trim()),
                Unit_AtkLevel = int.Parse(values[4].Trim()),
                Unit_DefLevel = int.Parse(values[5].Trim()),
                Unit_ResistPierce = float.Parse(values[6].Trim()),
                Unit_ResistSlash = float.Parse(values[7].Trim()),
                Unit_ResistBlunt = float.Parse(values[8].Trim())
            };
            UnitDictionary.Add(newData.Unit_Id, newData);
        });

        Debug.Log($"<color=green>Unit 데이터 파싱 완료! (총 {UnitDictionary.Count}개)</color>");
        
        foreach (UnitData unit in UnitDictionary.Values)
        {
            Debug.Log($"<color=green>[Unit]</color> ID: {unit.Unit_Id} | 이름: {unit.Unit_Name} | 타입: {unit.Unit_Type} | HP: {unit.Unit_MaxHp} | 공/방: {unit.Unit_AtkLevel}/{unit.Unit_DefLevel} | 내성(관/참/타): {unit.Unit_ResistPierce:F1}/{unit.Unit_ResistSlash:F1}/{unit.Unit_ResistBlunt:F1}");
        }
    }

    private void LoadCardData()
    {
        Debug.Log("<color=cyan>================ Card_Table 파싱 시작 ================</color>");

        LoadCsvBase(_filePath[1], 12, (values) =>
        {
            CardData newData = new CardData
            {
                Card_Id = values[0].Trim(),
                Card_Type = int.Parse(values[1].Trim()),
                Card_Name = values[2].Trim(),
                Card_CostPride = int.Parse(values[3].Trim()),
                Card_BasePower = int.Parse(values[4].Trim()),
                Card_CoinCount = int.Parse(values[5].Trim()),
                Card_CoinPower = int.Parse(values[6].Trim()),
                Card_AtkType = int.Parse(values[7].Trim()),
                Card_BoostType = values[8].Trim(),
                Card_BoostValue = int.Parse(values[9].Trim()),
                Card_BoostMax = int.Parse(values[10].Trim()),
                Card_Description = values[11].Trim()
            };
            CardDictionary.Add(newData.Card_Id, newData);
        });

        Debug.Log($"<color=cyan>Card 데이터 파싱 완료! (총 {CardDictionary.Count}개)</color>");
        
        // 파싱된 데이터 출력
        foreach (CardData card in CardDictionary.Values)
        {
            Debug.Log($"<color=cyan>[Card]</color> ID: {card.Card_Id} | 이름: {card.Card_Name} | 소모: {card.Card_CostPride} | 위력: {card.Card_BasePower} | 코인수: {card.Card_CoinCount}(+{card.Card_CoinPower}) | 속성: {card.Card_AtkType} | 특수: {card.Card_BoostType}({card.Card_BoostValue}씩/최대{card.Card_BoostMax})");
        }
    }

    private void LoadCoinData()
    {
        Debug.Log("<color=yellow>================ Coin_Table 파싱 시작 ================</color>");

        LoadCsvBase(_filePath[2], 6, (values) =>
        {
            CoinData newData = new CoinData
            {
                Coin_Id = values[0].Trim(),
                Coin_CardId = values[1].Trim(),
                Coin_Index = int.Parse(values[2].Trim()),
                Coin_TriggerType = int.Parse(values[3].Trim()),
                Coin_EffectData = values[4].Trim(),
                Coin_Description = values[5].Trim(),
            };
            CoinDictionary.Add(newData.Coin_Id, newData);
        });
        
        Debug.Log($"<color=yellow>Coin 데이터 파싱 완료! (총 {CoinDictionary.Count}개)</color>");
        
        // 파싱된 데이터 출력
        foreach (CoinData coin in CoinDictionary.Values)
        {
            Debug.Log($"<color=yellow>[Coin]</color> ID: {coin.Coin_Id} | 부모카드: {coin.Coin_CardId} | 순서: {coin.Coin_Index} | 발동조건: {coin.Coin_TriggerType} | 효과: {coin.Coin_EffectData} | 설명: {coin.Coin_Description}");
        }
    }

    private void LoadEffectData()
    {
        Debug.Log("<color=magenta>================ Effect_Table 파싱 시작 ================</color>");

        LoadCsvBase(_filePath[3], 4, (values) =>
        {
            EffectData newData = new EffectData
            {
                Effect_Id = values[0].Trim(),
                Effect_Name = values[1].Trim(),
                Effect_Type = int.Parse(values[2].Trim()),
                Effect_Description = values[3].Trim()
            };
            EffectDictionary.Add(newData.Effect_Id, newData);
        });

        Debug.Log($"<color=magenta>Effect 데이터 파싱 완료! (총 {EffectDictionary.Count}개)</color>");
        
        // 파싱된 데이터 출력
        foreach (EffectData effect in EffectDictionary.Values)
        {
            Debug.Log($"<color=magenta>[Effect]</color> ID: {effect.Effect_Id} | 이름: {effect.Effect_Name} | 타입: {effect.Effect_Type} | 설명: {effect.Effect_Description}");
        }
    }
}