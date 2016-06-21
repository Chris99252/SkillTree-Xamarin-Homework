# SkillTree-Xamarin-Homework
突破語言限制 Xamarin 開發全平台 APP 新手體驗村
作業：使用 Xamarin.Forms 實戰開發著名便利商店 店鋪查詢App

因為我只有 Android 可以跑模擬器，功能以 Android 為主

目前功能是正常的，但遇到以下問題，再麻煩 James Tsai 老師解惑了

1. 縣市與鄉鎮區的選單，我有利用老師給的 api 做 picker 下拉選單，想要拿到 api 資料，初始化台北市的鄉鎮區選單項目
   但我沒辦法在，MyListViewPage Constructor 呼叫 async 非同步的資料，有上網爬了 async 與 await 的資訊，但還是沒找到解答
   目前預設值是寫死加入。
   
2. 選擇縣市選項，鄉鎮區動態變動項目，有遇到一個問題是，不知道怎麼把 Picker 的項目全部刪除
   使用 RemoveAt() 透過 index 刪除，要刪最後一個項目，都會報錯。
   
   Eror : Index was out of range. Must be non-negative and less than the size of the collection. Parameter name: index
   
   目前我是刪道剩下最後一個，加新項目之後，再把最後一個舊的項目刪除。
