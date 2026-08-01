using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // エフェクトが正しいブロックに触れているときに表示するUI
    [SerializeField] private GameObject RedUI;  // 画面が赤くなる
    [SerializeField] private GameObject BlueUI;  // 画面が青くなる
    [SerializeField] private GameObject ExclamationUI;  // 「！」マーク表示

    [SerializeField] private GameObject GameOverUI;  // GameOver用のUI
    [SerializeField] private GameObject GameClearUI;  // GameClear用のUI

    // ターゲット用のオブジェクトを管理する配列
    [SerializeField] private GameObject[] targetPrefabs;

    public bool fireCount = false;  // 木ブロックが炎に触れている
    public bool waterCount = false;  // 溶岩ブロックが水に触れている
    public bool lineCount = false;  // ブロックが赤線に触れている

    public bool pose = false;      // ポーズモードかどうか
    public bool gameClear = false;  // ゲームクリア判定
    public bool gameOver = false;  // ゲームオーバー判定

    private bool isEndProcessed = false; // クリア/ゲームオーバー処理が一度でも走ったか

    // SE用
    private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverSE;
    [SerializeField] private AudioClip gameClearSE;

    private int waveCount = 1;
    private float timer = 0f;

    public static GameManager Instance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Time.timeScale = 1f;

        // すでにInstanceが存在していたら自分を消す（重複防止）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // コンポーネントを取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // すでにクリアかゲームオーバーの処理が始まっていたら何もしない
        if (isEndProcessed) return;

        // 毎フレーム時間を足していく
        timer += Time.deltaTime;

        // ポーズモードでなければ、7秒ごとに生成
        if (!pose && timer >= 7.0f)
        {
            // ターゲットをスポーンさせる関数を呼ぶ
            SpawnTarget();

            // タイマーリセット
            timer = 0f;
        }

        // スペースキーで一時停止/再生
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // フラグ切り替え
            pose = !pose;
        }
        // Rキーでリロード
        if (Input.GetKeyDown(KeyCode.R))
        {
            // シーンを再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        // エスケイプキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ゲーム終了
            Application.Quit();
        }

        // 炎がブロックに触れているときはRedUI表示
        if (fireCount) RedUI.SetActive(true);
        else RedUI.SetActive(false);

        // 水がブロックに触れているときはBlueUI表示
        if (waterCount) BlueUI.SetActive(true);
        else BlueUI.SetActive(false);

        // ブロックが赤線に触れているときはExclamationUI表示
        if (lineCount) ExclamationUI.SetActive(true);
        else ExclamationUI.SetActive(false);

        // ブロックが赤線に触れ、5秒以上たったらGameOverUI表示
        if (gameOver) 
        {
            isEndProcessed = true;

            // UI表示後、5秒待ったらステージを再読み込み
            StartCoroutine(StageReload());
        }

        // 一定時間耐えたらクリア
        if (gameClear) 
        {
            isEndProcessed = true;

            // UI表示後、5秒待ったらステージを再読み込み
            StartCoroutine(StageClear());
        }
    }

    // ターゲットをスポーンさせる関数
    private void SpawnTarget()
    {
        // ウェーブごとに生成されるブロックは増加
        for (int i = 0; i <= waveCount; i++)
        {
            // 0~（配列の要素数-1）の間でランダムな数字を選ぶ
            int randomIndex = Random.Range(0, targetPrefabs.Length);
            // 配列からプレハブを取得
            GameObject selectedPrefab = targetPrefabs[randomIndex];

            // スポーン位置もランダムにする
            Vector3 spawnPosition = selectedPrefab.transform.position + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-3f, 3f));

            // 初期位置に生成
            Instantiate(selectedPrefab, spawnPosition, selectedPrefab.transform.rotation);   
        }

        // 6未満なら、次のウェーブに生成する数を増やす
        if (waveCount < 6) waveCount++;
    }

    // 5秒待ったらステージを再読み込みするコルーチン（GameOver）
    private IEnumerator StageReload()
    {
        GameOverUI.SetActive(true);
        audioSource.PlayOneShot(gameOverSE);

        // 3秒間待つ
        yield return new WaitForSeconds(5.0f);

        // シーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 5秒待ったらステージを再読み込みするコルーチン（GameClear）
    private IEnumerator StageClear()
    {
        GameClearUI.SetActive(true);
        audioSource.PlayOneShot(gameClearSE);

        // 3秒間待つ
        yield return new WaitForSeconds(5.0f);

        // シーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
