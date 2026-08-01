using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using TMPro;
using System.Collections;

public class HandGestureController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;  // メインカメラを格納する変数
    [SerializeField] private GameObject Fire;  // 炎のオブジェクト
    [SerializeField] private GameObject Water;  // 水のオブジェクト

    // SE用
    [SerializeField] private AudioClip fireSE;
    [SerializeField] private AudioClip waterSE;

    [SerializeField] private float Depth = 2.0f;  // カメラから何m前に変換する
    [SerializeField] private float waterOffsetY = 2.0f;  // 水エフェクトをどれくらい浮かすか

    // ジェスチャーの状態を記憶する変数
    private bool isRockDetected = false;  // グーであることを記憶
    private bool isScissorsDetected = false;  // チョキであることを記憶
    private bool isPaperDetected = false;  // パーであることを記憶

    // 一度グーにしてからフラグを変更
    private bool preparatoryMovement = false;

    // リロード時・破棄時のスレッド安全用フラグを追加
    private bool isDestroyed = false;

    private Vector2[] rawNormalizedLandmarks;  // 正規化された座標を格納する変数
    private Vector2 index_RawNormalizedLandmarks;  // 正規化された座標を格納する変数（人差し指）
    private readonly object lockObject = new object();  // スレッド間の安全用ロック

    private AudioSource audioSource;

    private void Start()
    {
        if (HandLandmarkerRunner.Instance != null)
        {
            // 念のため一旦解除してから登録する（重複登録の防止）
            HandLandmarkerRunner.Instance.OnResult -= CheckhandSign;
            // 通知を受け取る
            HandLandmarkerRunner.Instance.OnResult += CheckhandSign;
        }

        // コンポーネントを取得
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        // 破棄フラグを立てる
        isDestroyed = true;
        
        if (HandLandmarkerRunner.Instance != null)
        {
            // 重複防止
            HandLandmarkerRunner.Instance.OnResult -= CheckhandSign;
        }
    }

    void Update()
    {
        Vector2[] currentLandmarks = null;
        Vector2 indexLandmark = Vector2.zero;
        
        lock (lockObject)
        {
            // CheckhandSign()で保存したデータを読み込む（競合防止）
            if (rawNormalizedLandmarks != null)
            {
                // コピーを作ることで安全に保存
                currentLandmarks = (Vector2[])rawNormalizedLandmarks.Clone();  // 炎
                indexLandmark = index_RawNormalizedLandmarks;  // 水（単体のVector2にCloneは使えない）
            }
        }

        // 手の座標が取得できていれば移動（炎）
        if (Fire.activeSelf && currentLandmarks != null && currentLandmarks.Length > 3)
        {
            Vector3 sum = Vector3.zero;

            // 手のひらを構成するマークすべてを足す
            foreach (var lm in currentLandmarks)
            {
                Vector3 screenPoint = new Vector3(
                    lm.x * Screen.width,            // 画面の大きさに合わせた座標にする
                    (1.0f - lm.y) * Screen.height,  // MediaPipeとUnityの上下は逆なので反転
                    Depth                           // 画面からどれくらい離れているか
                );

                // ワールド座標へ変換し、足し合わせる
                sum += mainCamera.ScreenToWorldPoint(screenPoint);
            }

            // 平均座標を計算して移動
            Vector3 centerPosition = sum / currentLandmarks.Length;
            Fire.transform.position = centerPosition;
        }

        // 手の座標が取得できていれば移動（水）
        if (Water.activeSelf && indexLandmark != null)
        {
            Vector3 screenPoint = new Vector3(
                indexLandmark.x * Screen.width,            // 画面の大きさに合わせた座標にする
                (1.0f - indexLandmark.y) * Screen.height,  // MediaPipeとUnityの上下は逆なので反転
                Depth                                      // 画面からどれくらい離れているか
            );

            // 人差し指ワールド座標を計算
            Vector3 indexPosition = mainCamera.ScreenToWorldPoint(screenPoint);
            // 人差し指の先端 ＋ 高さオフセット位置へ移動
            Water.transform.position = indexPosition + (Vector3.up * waterOffsetY);
        }

        // グーだったら
        if (isRockDetected)
        {
            // 炎を出していたら
            if (Fire.activeSelf)
            {
                // 消す
                Fire.SetActive(false);
            }
            // 水を出していたら
            if (Water.activeSelf)
            {
                // 消す
                Water.SetActive(false);
            }

            preparatoryMovement = true;  // 予備動作セット
            isRockDetected = false;  // フラグは戻す
        }

        // チョキだったら
        if (isScissorsDetected)
        {
            // 水を出す
            Water.SetActive(true);

            // SE再生
            audioSource.PlayOneShot(waterSE);

            isScissorsDetected = false;  // フラグは戻す
            preparatoryMovement = false;  // 予備動作リセット
        }

        // パーだったら
        if (isPaperDetected)
        {
            // 炎を出す
            Fire.SetActive(true);

            // SE再生
            audioSource.PlayOneShot(fireSE);

            // フラグリセット
            isPaperDetected = false;  // パーフラグをリセット
            preparatoryMovement = false;  // 予備動作リセット
        }
    }

    // 角度の計算
    private float GetAngle(Vector2 a, Vector2 b, Vector2 c)
    {
        // bを中心に角度を計算
        Vector2 ba = a - b; 
        Vector2 bc = c - b;

        return Vector2.Angle(ba, bc);
    }

    public void CheckhandSign(HandLandmarkerResult result)
    {
        // リロード中や破棄中に呼び出されたら即処理を抜ける（カメラフリーズ防止）
        if (isDestroyed) return;
    
        // エラー対策
        if (result.handLandmarks == null || result.handLandmarks.Count == 0) return;

        // 一つ目の手のデータを取得
        var landmarks = result.handLandmarks[0];

        // 手のひらの平均座標を取得する
        int[] KeyIndices = new int[] { 0, 5, 9, 17 };  // どの点を使用するか
        Vector2[] tempPositions = new Vector2[KeyIndices.Length];  // 点の数を要素数にする（zは固定なのでとらない）

        for (int i = 0; i < KeyIndices.Length; i++)
        {
            // i点の3次元座標を取得（正規化座標）
            var lm = landmarks.landmarks[KeyIndices[i]];
            tempPositions[i] = new Vector2(lm.x, lm.y);
        }

        // 人差し指の座標を取得する
        var indexLm = landmarks.landmarks[8];
        Vector2 indexTempPositions = new Vector2(indexLm.x, indexLm.y);

        // メインスレッドへ渡すために座標を保存
        lock (lockObject)
        {
            rawNormalizedLandmarks = tempPositions;
            index_RawNormalizedLandmarks = indexTempPositions;
        }


        // 親指
        float thumbAngle = GetAngle(
            new Vector2(landmarks.landmarks[2].x, landmarks.landmarks[2].y),
            new Vector2(landmarks.landmarks[3].x, landmarks.landmarks[3].y),
            new Vector2(landmarks.landmarks[4].x, landmarks.landmarks[4].y)
        );
        // 人差し指
        float indexAngle = GetAngle(
            new Vector2(landmarks.landmarks[6].x, landmarks.landmarks[6].y),
            new Vector2(landmarks.landmarks[7].x, landmarks.landmarks[7].y),
            new Vector2(landmarks.landmarks[8].x, landmarks.landmarks[8].y)
        );
        // 中指
        float middleAngle = GetAngle(
            new Vector2(landmarks.landmarks[10].x, landmarks.landmarks[10].y),
            new Vector2(landmarks.landmarks[11].x, landmarks.landmarks[11].y),
            new Vector2(landmarks.landmarks[12].x, landmarks.landmarks[12].y)
        );
        // 薬指
        float ringAngle = GetAngle(
            new Vector2(landmarks.landmarks[14].x, landmarks.landmarks[14].y),
            new Vector2(landmarks.landmarks[15].x, landmarks.landmarks[15].y),
            new Vector2(landmarks.landmarks[16].x, landmarks.landmarks[16].y)
        );
        // 小指
        float pinkyAngle = GetAngle(
            new Vector2(landmarks.landmarks[18].x, landmarks.landmarks[18].y),
            new Vector2(landmarks.landmarks[19].x, landmarks.landmarks[19].y),
            new Vector2(landmarks.landmarks[20].x, landmarks.landmarks[20].y)
        );

        // 一定以上の角度がついていたら開いていると判定
        //bool isThumbOpen = thumbAngle > 150f;    // 親指
        bool isIndexOpen = indexAngle > 160f;    // 人差し指
        bool isMiddleOpen = middleAngle > 160f;  // 中指
        bool isRingOpen = ringAngle > 160f;      // 薬指
        bool isPinkyOpen = pinkyAngle > 160f;    // 小指

        //Debug.Log("親指:" + isThumbOpen);
        Debug.Log("人差し指:" + isIndexOpen);
        Debug.Log("中指:" + isMiddleOpen);
        Debug.Log("薬指:" + isRingOpen);
        Debug.Log("小指:" + isPinkyOpen);

        // グーの判定
        if (!isIndexOpen && !isMiddleOpen && !isRingOpen && !isPinkyOpen)
        {
            //Debug.Log("グーですよ");

            // グーになったと記憶
            isRockDetected = true;
        }
        // チョキの判定
        else if (isIndexOpen && isMiddleOpen && !isRingOpen && !isPinkyOpen)
        {
            //Debug.Log("チョキですよ");

            // 予備動作後であれば
            if (preparatoryMovement)
            {
                // チョキになったと記憶
                isScissorsDetected = true;
            }
        }
        // パーの判定
        else if (isIndexOpen && isMiddleOpen && isRingOpen && isPinkyOpen)
        {
            //Debug.Log("パーですよ");

            // 予備動作後であれば
            if (preparatoryMovement)
            {
                // パーになったと記憶
                isPaperDetected = true;
            }
        }

    }
}
