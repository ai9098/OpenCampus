using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using TMPro;
using System.Collections;

public class HandDataProcessor : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text lm0Text;

    // 手首座標を保存
    private Vector3 lm0;
    // 新しいデータが来たかどうか
    private bool hasData;

    // オブジェクトが有効になったら
    void OnEnable()
    {
        StartCoroutine(Refister());
    }

    IEnumerator Refister()
    {
        // 1フレーム待つ
        yield return null;

        // 結果があればProcessHandDataを登録
        if (HandLandmarkerRunner.Instance != null)
        {
            HandLandmarkerRunner.Instance.OnResult += ProcessHandData;
        }
    }

    void OnDisable()
    {
        // 登録解除
        if (HandLandmarkerRunner.Instance != null)
        {
            HandLandmarkerRunner.Instance.OnResult -= ProcessHandData;
        }
    }

    private void ProcessHandData(HandLandmarkerResult result)
    {
        // 手が見つからなかったら終了
        if (result.handLandmarks == null || result.handLandmarks.Count == 0) return;

        // 手首の位置を取得(最初の手の手首)
        var wrist = result.handLandmarks[0].landmarks[0];

        // ベクターに変換
        lm0 = new Vector3(wrist.x, wrist.y, wrist.z);
        hasData = true;
    }

    void Update()
    {
        if (!hasData) return;

        // データがあれば表示する
        lm0Text.text =
            $"LM0 (Wrist)\n" +
            $"x: {lm0.x:F3}\n" +
            $"y: {lm0.y:F3}\n" +
            $"z: {lm0.z:F3}";
        
        hasData = false;
    }
}
