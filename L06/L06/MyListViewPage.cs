using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using FamilyMartStore.Services;
using Xamarin.Forms;
using L06.Model;
using System.Threading.Tasks;

namespace L06
{
    public class MyListViewPage : ContentPage
    {
        private Button searchButton;
        private readonly WebApiService myWebApiService;
        private Picker cityPicker;
        private Picker areaPicker;

        // 縣市下拉選單項目
        Dictionary<string, int> nameToCity = new Dictionary<string, int>()
        {
            { "台北市", 1 }, { "基隆市", 2 },
            { "新北市", 3 }, { "宜蘭縣", 4 },
            { "新竹市", 5 }, { "新竹縣", 6 },
            { "桃園市", 7 }, { "苗栗縣", 8 },
            { "台中市", 9 }, { "彰化縣", 10 },
            { "南投縣", 11 }, { "嘉義市", 12 },
            { "嘉義縣", 13 }, { "雲林縣", 14 },
            { "台南市", 15 }, { "高雄市", 16 },
            { "澎湖縣", 17 }, { "屏東縣", 18 },
            { "台東縣", 19 }, { "花蓮縣", 20 },
            { "金門縣", 21 }, { "連江縣", 22 }
        };

        // 預設台北市的鄉鎮區下拉選單項目
        List<AreaData> defaultAreaList = new List<AreaData>()
        {
            new AreaData() { post = 100, city = "台北市", town = "中正區" },
            new AreaData() { post = 103, city = "台北市", town = "大同區" },
            new AreaData() { post = 104, city = "台北市", town = "中山區" },
            new AreaData() { post = 105, city = "台北市", town = "松山區" },
            new AreaData() { post = 106, city = "台北市", town = "大安區" },
            new AreaData() { post = 108, city = "台北市", town = "萬華區" },
            new AreaData() { post = 110, city = "台北市", town = "信義區" },
            new AreaData() { post = 111, city = "台北市", town = "士林區" },
            new AreaData() { post = 112, city = "台北市", town = "北投區" },
            new AreaData() { post = 114, city = "台北市", town = "內湖區" },
            new AreaData() { post = 115, city = "台北市", town = "南港區" },
            new AreaData() { post = 116, city = "台北市", town = "文山區" }
        };

        public MyListViewPage(string title)
        {
            myWebApiService = new WebApiService();

            // 搜尋店鋪 Button 按鈕

            searchButton = new Button { Text = "Search" };

            // 建立縣市 Picker 下拉選單
            cityPicker = new Picker
            {
                Title = "縣市選單",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // 初始化選項
            foreach (string cityName in nameToCity.Keys)
            {
                cityPicker.Items.Add(cityName);
            }

            // 預設選取第一個項目
            cityPicker.SelectedIndex = 0;

            areaPicker = new Picker
            {
                Title = "鄉鎮市區選單",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            foreach (var item in defaultAreaList)
            {
                areaPicker.Items.Add(item.town);
            }

            areaPicker.SelectedIndex = 4;

            // 註冊 cityPicker SelectedIndexChanged 事件
            cityPicker.SelectedIndexChanged += async (sender, args) =>
            {
                if (cityPicker.SelectedIndex != -1)
                {
                    // 將舊有的鄉鎮區項目刪除
                    // Error : 因為沒辦法全部刪除，會顯示 Index was out of range. Must be non-negative and less than the size of the collection. Parameter name: index，所以留下一個，加新項目後在刪掉
                    int count = areaPicker.Items.Count - 1;

                    for (int i = 0; i < count; i++)
                    {
                        areaPicker.Items.RemoveAt(0);
                    }

                    // 透過 api 拿到對應縣市的鄉鎮區項目
                    var areaResult = await myWebApiService.GetAreaDataAsync(cityPicker.Items[cityPicker.SelectedIndex]);

                    // 將 Json 轉為 List<AreaData>
                    var myAreaDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AreaData>>(areaResult);

                    // 替鄉鎮區下拉選單，增加對應縣市的新項目
                    foreach (var item in myAreaDataList)
                    {
                        areaPicker.Items.Add(item.town);
                    }

                    // 將之前留下來最後一個舊項目刪除
                    areaPicker.Items.RemoveAt(0);

                    // 預設選取第一個項目
                    areaPicker.SelectedIndex = 0;
                }
            };

            // 註冊 areaPicker SelectedIndexChanged 事件
            areaPicker.SelectedIndexChanged += (sender, args) =>
            {
                // log 記錄
                if (areaPicker.SelectedIndex != -1)
                {
                    Debug.WriteLine("AreaName is " + areaPicker.Items[areaPicker.SelectedIndex]);
                }
            };

            Title = title;

            var listView = new ListView
            {
                IsPullToRefreshEnabled = true,
                RowHeight = 80,
                ItemsSource = new List<StoreData>()
                {
                    new StoreData {Name = "全家大安店", Address = "台北市大安區大安路一段20號", Tel = "02-27117896"},
                    new StoreData {Name = "全家仁慈店", Address = "台北市大安區仁愛路四段48巷6號", Tel = "02-27089002"},
                    new StoreData {Name = "全家明曜店", Address = "台北市大安區仁愛路四段151巷34號", Tel = "02-27780326"},
                    new StoreData {Name = "全家國泰店", Address = "台北市大安區仁愛路四段266巷15弄10號", Tel = "02-27542056"},
                    new StoreData {Name = "全家忠愛店", Address = "台北市大安區仁愛路四段27巷43號", Tel = "02-27314580"},
                },
                ItemTemplate = new DataTemplate(typeof(MyListViewCell))
            };

            listView.ItemTapped += (sender, e) =>
            {
                var baseUrl = "https://www.google.com.tw/maps/place/";
                var storeData = e.Item as StoreData;

                if (storeData != null)
                    Device.OpenUri(new Uri($"{baseUrl}{storeData.Address}"));

                ((ListView)sender).SelectedItem = null;
            };

            // 註冊 searchButton Clicked 事件
            searchButton.Clicked += async (sender, e) =>
            {
                // 透過 api 拿到對應區域的全家店鋪資料
                var resultData = await myWebApiService.GetFamilyStoreDataAsync(cityPicker.Items[cityPicker.SelectedIndex], areaPicker.Items[areaPicker.SelectedIndex]);

                // 將 Json 轉為 List<FamilyStore>
                var familyStoreList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FamilyStore>>(resultData);

                // 將 List<FamilyStore> 轉為 List<StoreData> 
                var myStoreDataList = familyStoreList.Select(p => new StoreData()
                {
                    Address = p.addr,
                    Name = p.NAME,
                    Tel = p.TEL
                }).ToList();

                // 更換店鋪資訊
                listView.ItemsSource = myStoreDataList;
                Debug.WriteLine(myStoreDataList.Count);
            };

            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    cityPicker,
                    areaPicker,
                    searchButton,
                    new Label
                    {
                        HorizontalTextAlignment= TextAlignment.Center,
                        Text = Title,
                        FontSize = 30
                    },
                    listView
                }
            };
        }
    }
}