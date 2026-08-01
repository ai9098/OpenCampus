using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float CountTime;

    private float timer;

    private void Start()
    {
        // ゲーム開始時にタイマーを初期化
        timer = CountTime;
    }

    private void Update()
    {
        // ポーズモードならカウントダウンは減らない
        if (GameManager.Instance.pose) return;

        // タイマーの値が0以下にならないようにする
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0) timer = 0;
        }

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        uiFill.fillAmount = Mathf.InverseLerp(0, CountTime, timer);
        uiText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        // CountTime分耐えたらクリア
        if (timer <= 0)
        {
            GameManager.Instance.pose = true;  // クリアしたらブロックが増えないようにする
            GameManager.Instance.gameClear = true;
        }
    }
}
