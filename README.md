# すいそかゲーム: ReadMe
---
## 内容
- 学校で行われたゲームジャムにて制作
- 元素を繋げるパズルゲーム
- スマホユーザーをターゲットとしたゲーム
- Google Sheets を用いたランキング機能を実装

## 開発環境
- Unity 2022.3.22f1
- C#
- Google Apps Script / Google Sheets API (スコア保存・取得用)

## 開発担当箇所

| スクリプト名 | 内容                           |
| ---- | ---------------------------- |
| LoadAnima.cs   | 通信を行う際のロードアニメーション        |
| ObjectManager.cs   | オブジェクトプールでのオブジェクト管理                | 
| RankingDisplay.cs    |GASを使用したランキングの表示         |
| ScoreSender.cs   |GASを使用したランキングの登録              |
| Json.cs   | GASを使用するときに扱うJsonデータ群             |

---
## ランキング機能について
- ゲーム内でスコアを送信すると、Google Sheets にデータが保存されます。
- ランキングは Google Sheets をスプレッドシートを簡易DBとして利用し、スコアを集計・表示しています。
- サーバーを自前で構築せずに、Google Apps Script とスプレッドシートを用いた簡易バックエンドとして実装しました。
- これにより、外部データベースを用意しなくてもオンラインランキングを実現しています。
- 2枚のシートを使用して通信処理を最小化  
  - 1枚目: 名前とスコアのDB  
  - 2枚目: 最低スコアの保持  
  - 新規スコアが最低スコアを上回った場合のみランキングを更新  

### Google Apps Script 抜粋

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
