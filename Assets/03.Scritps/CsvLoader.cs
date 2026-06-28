using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    public string Unit_Id;              // 유닛 아이디 string
    public string Unit_Name;            // 유닛 이름

    public int Unit_Type;               // 유닛 타입 int로 받아서 enum으로 캐스팅 예정 -> 0:Player, 1:Enemy, 2:Boss
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
    public string Card_Name;            // 카드 이름
    public string Card_Description;     // UI에 표시될 카드 설명
    public string Card_BoostType;       // 코인이 앞면 판정일 때, 추가 위력 증가 조건의 ID를 저장하는 부분

    public int Card_Type;               // 카드의 속성 int로 받아서 enum으로 캐스팅 예정
    public int Card_CostPride;          // 카드 소모값
    public int Card_BasePower;          // 카드 기본 값
    public int Card_CoinCount;          // 카드 코인 갯수
    public int Card_CoinPower;          // 코인이 앞면일때 코인 갯수 당 증가할 위력
    public int Card_AtkType;            // 카드의 공격 유형 int로 받아서 enum으로 캐스팅 예정

    public int Card_BoostValue;         // 코인이 앞면 판정일때 앞면인 코인 갯수 별로 증가될 카드 위력
    public int Card_BoostMax;           // 코인이 앞면 판정일때 증가 할수 있는 최대 위력
}

public class CoinData
{
    public string Coin_Id;
    public string Coin_CardId;
    public string Coin_EffectData;
    public string Coin_Description;

    public int Coin_Index;
    public int Coin_TrggerType;

}

public class EffectData
{
    public string Effect_Id;
    public string Effect_Name;
    public string Effect_Description;

    public int Effect_Type;
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
                        // 넘겨받은 람다식(조립 설명서)을 실행하여 데이터를 파싱
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
        Debug.Log("<color=green>Unit_Table 로드 및 파싱 시작!</color>");

        // 공통 함수에 경로, 기대 컬럼 수(9개), 그리고 어떻게 조립할지(람다식)만 넘겨줍니다.
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

        Debug.Log($"<color=cyan>Unit 데이터 파싱 완료! 총 {UnitDictionary.Count}개 등록.</color>");
    }

    private void LoadCardData()
    {
        Debug.Log("<color=green>Card_Table 로드 및 파싱 시작!</color>");

        // 카드 테이블은 컬럼이 12개입니다.
        LoadCsvBase(_filePath[1], 12, (values) =>
        {
            CardData newData = new CardData
            {
                Card_Id = values[0].Trim(),
                Card_Name = values[1].Trim(),
                Card_Description = values[2].Trim(),
                Card_BoostType = values[3].Trim(),
                Card_Type = int.Parse(values[4].Trim()),
                Card_CostPride = int.Parse(values[5].Trim()),
                Card_BasePower = int.Parse(values[6].Trim()),
                Card_CoinCount = int.Parse(values[7].Trim()),
                Card_CoinPower = int.Parse(values[8].Trim()),
                Card_AtkType = int.Parse(values[9].Trim()),
                Card_BoostValue = int.Parse(values[10].Trim()),
                Card_BoostMax = int.Parse(values[11].Trim())
            };
            CardDictionary.Add(newData.Card_Id, newData);
        });

        Debug.Log($"<color=cyan>Card 데이터 파싱 완료! 총 {CardDictionary.Count}개 등록.</color>");
    }

    private void LoadCoinData()
    {
        Debug.Log("<color=gold>Coin_Table 로드 및 파싱 시작!</color>");

        //LoadCsvBase(_filePath[2])
    }
}