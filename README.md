# Break Blocks with Magic
手のジェスチャーで炎・水の魔法を操り、ブロックを破壊するゲームです。

## 概要
MediaPipeUnityPluginを利用してUnityに手認識機能を組み込み、リアルタイムでジェスチャーをゲームに反映させるシステムを構築しました。
ジェスチャーによって、炎・水エフェクトを手のひらに表示でき、エフェクトを降り続けるブロックに当てることでブロックを破壊できます。

なお、この作品は大学のオープンキャンパスにて展示し、画像処理を用いた技術を楽しく体験してもらうことを目的に制作しました。

<p align="center">
  <img src="Images/gamegif.gif" width="640">
</p>

以下はクリア演出の様子です。
ゲーム開始からクリアまで一連の流れが完結するように制作しました。

<p align="center">
  <img src="Images/cleargif.gif" width="640">
</p>

## 開発環境
- Windows 11
- Unity 6.3
- MediaPipeUnityPlugin v0.16.3
- C#
- Logi C310 HD WebCam（Web カメラ）

## 操作方法
| 操作 | 内容 |
|------|------|
| グー | 予備動作（力を溜める） |
| パー | 炎エフェクトを出す |
| チョキ | 水エフェクトを出す |
| R | ゲームのリロード |
| Space | 一時停止 |
| Esc | ゲームを終了する |

## 実装した機能
- MediaPipeによる手の認識
- 炎・水エフェクト
- 降り続けるブロック
- ゲームクリア・ゲームオーバー処理

## 工夫した点
- 指が「伸びているか」を認識する際には、第一関節を頂点とし、指先と第二関節がなす角度を用いて判定した。
- エフェクトを出すには一度「グー」を挟む必要があり、認識の安定化と、力を溜めるという演出による没入感の向上につながった。
- 手のひらや人差し指を認識し、エフェクトを手の動きに追従するようにした。
- 木材・溶岩ブロックの二種類が存在し、木材には炎を、溶岩には水のエフェクトを当て続けることで破壊できるようにした。

## 今後の改善点
- 手の角度がある程度大きくついても認識できるよう、より精密な認識を行えるようにする。
- 操作権を取られてしまわないよう、複数人の手を読み取れるようにするか、一番近い手を認識するように改善する。
- 3DモデルとMediaPipeを組み合わせ、ジェスチャーに応じてモデルが魔法を放つような演出を実装する。

## 動画
[https://youtu.be/FqWzrbda8uA](https://youtu.be/FqWzrbda8uA)

## 制作ブログ
[https://note.com/aa02/m/mf854b5c0880c](https://note.com/aa02/m/mf854b5c0880c)

## 参考動画
Assets/Scripts/HandDataProcessor.csは、以下の動画のコードを使用させていただきました。
[Unity + MediaPipe: Access Hand Landmark X, Y, Z & Track Multiple Hands (Beginner Friendly)](https://www.youtube.com/watch?v=1k80P0d8_AE&t=272s)

## 使用したアセット
[MediaPipeUnityPlugin](https://github.com/homuler/MediaPipeUnityPlugin/releases)

[25+ Free Stylized Textures - Grass, Ground, Floors, Walls & More](https://assetstore.unity.com/packages/2d/textures-materials/25-free-stylized-textures-grass-ground-floors-walls-more-241895?clickref=1100lDgHpAYi&utm_source=partnerize&utm_medium=affiliate&utm_campaign=unity_affiliate)

