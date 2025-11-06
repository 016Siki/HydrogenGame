# すいそかゲーム

> **学校内ゲームジャム制作作品（2025年7月）**  
> 元素を繋げてスコアを競う、スマートフォン向けのカジュアルパズルゲーム。  
> Google Apps Script版 と Laravel版の二種のオンラインランキング機能を実装しています。

---

##  概要

- **ゲーム制作期間**：2024年7月（3日間）
- **GAS版ランキング制作期間**:2025年5月
- **Laravel版ランキング制作期間**:2025年7月~9月
- **チーム構成**：5名（プランナー1名、デザイン1名、プログラマー3名）  
- **ジャンル**：スマートフォン向けパズルゲーム  
- **特徴**：
  - Google Sheetsを利用したサーバーレスなランキング機能（GAS＋API通信）  
  - Laravelフレームワークを用いたWebでも閲覧可能なランキング機能  
  - スマートフォンUIに最適化した操作設計  
  - オンラインアカウント管理とスコア送信処理を自作APIで実装  

---

## 開発環境

| 項目 | 内容 |
|------|------|
| **OS** | Windows / AlmaLinux |
| **開発環境** | Unity 2022.3.22f1 / Laravel / Visual Studio / VS Code |
| **開発言語** | C#, PHP, HTML / CSS / JavaScript, Google Apps Script |

---
## クライアントとサーバーの通信構成図

![通信構成図](./img/image.png)

---

## 担当箇所（主なスクリプト）

| スクリプト名 | 内容 |
| ---- | ---- |
| **LoadAnima.cs** | 通信時のロードアニメーション制御 |
| **ObjectManager.cs** | オブジェクトプールによるオブジェクト管理 |
| **RankingDisplay.cs** | GASを使用したランキング表示処理 |
| **ScoreSender.cs** | GASを使用したスコア送信処理 |
| **AccountLogin.cs** | Laravelを使用したアカウントログインAPI連携 |
| **LaravelRankingDisplay.cs** | Laravelを使用したランキング表示 |
| **LaravelScoreSender.cs** | Laravelを使用したスコア送信 |
| **TokenCrypto.cs** | トークン暗号化・復号処理（Laravel APIとの安全通信） |
| **Json.cs** | GAS通信で扱うJSONデータ定義群 |

---

## Google Apps Script版ランキング機能

Google Sheets を簡易データベースとして利用し、  
サーバーを持たずにスコア登録とランキング取得を実現。

### 構成
- **scoreシート**：スコアデータ（名前・スコア）  
- **min_scoreシート**：最低スコアを保持（10位を超える場合のみ更新）  

### GASコード抜粋

```javascript
// スコア登録（POST）
function doPost(e) {
  const sheet = SpreadsheetApp.openById('<<スプレッドシートID>>');
  const scoreSheet = sheet.getSheetByName('score');
  const minSheet = sheet.getSheetByName('min_score');

  const name = e.parameter.name || "NoName";
  const score = Number(e.parameter.score);

  // 最低スコアを超える場合のみ登録
  const currentMin = Number(minSheet.getRange("A1").getValue());
  if (score > currentMin) {
    scoreSheet.appendRow([name, score]);
    scoreSheet.getRange(2,1,scoreSheet.getLastRow()-1,2).sort({ column: 2, ascending: false });
    minSheet.getRange("A1").setValue(scoreSheet.getRange("B10").getValue());
  }

  return ContentService.createTextOutput(JSON.stringify({ result: "updated" }))
    .setMimeType(ContentService.MimeType.JSON);
}

// ランキング取得（GET）
function doGet() {
  const sheet = SpreadsheetApp.openById('<<スプレッドシートID>>');
  const scoreSheet = sheet.getSheetByName('score');
  const data = scoreSheet.getDataRange().getValues();

  const top10 = data.slice(1)
    .filter(row => typeof row[1] === 'number')
    .sort((a,b) => b[1]-a[1])
    .slice(0,10)
    .map((row,i)=>({ rank:i+1, name:row[0], score:row[1] }));

  return ContentService.createTextOutput(JSON.stringify(top10))
    .setMimeType(ContentService.MimeType.JSON);
}
```

## Laravel版ランキング機能について
Laravelを用いて構築したサーバー側APIを通じて、
アカウント管理・トークン認証・スコア保存・ランキング表示を行います。
## Unityクライアント機能
- メールアドレスによるアカウント作成・ログイン
- Laravel Sanctum による APIトークン発行
- トークンをUnityクライアント内でAES暗号化して保持
- データベース上にスコアを記録
- 上位50件を表示可能

## Webクライアント機能
- クライアントで作成したアカウントでログイン可能
- 総合・月間・デイリーランキングの切り替え表示
- 自分の順位をハイライト表示
- アカウント削除機能
- 最大100件のランキング閲覧対応

## Laravelアプリケーションgithub
Laravelのgitはこちらから⇒**https://github.com/016Siki/HydrogenGameLaravel**
## ポートフォリオ詳細ページ
詳しくはこちらから⇒**https://shigetahiroki-portfolio.netlify.app/project2**
