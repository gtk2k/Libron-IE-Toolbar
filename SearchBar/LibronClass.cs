// ==UserScript==
// @name          Libron
// @namespace     http://libron.net
// @description   Amazon のページから最寄りの図書館の蔵書を検索
// @author        Junya Ishihara(http://champierre.com)
// @include       http://www.amazon.*
// @license       MIT License(http://en.wikipedia.org/wiki/MIT_License)
// ==/UserScript==
//
// == ライセンス ==
// 本ソフトウェアの著作権は、
// コードに特に明記がない物は Junya Ishihara(http://champierre.com) に帰属します。
// また、本ソフトウェアのライセンスは MIT License(http://en.wikipedia.org/wiki/MIT_License) とします。
// 
//
// == 謝辞 ==
//
// Libronには以下の方々が貢献してくださいました。
//
// [ libron 1.x Osaka, Hyogo version ] Mutsutoshi Yoshimoto(http://github.com/negachov)
// [ libron 1.x Kyoto version ] Takanobu Nishimura(http://github.com/takanobu)
// [ libron 1.x Kanagawa version ] Yukinori Suda(http://github.com/sudabon)
// [ libron 1.x Gifu version ] Gifuron(http://github.com/gifuron)
// [ libron 1.x Saitama version ] MIKAMI Yoshiyuki(http://github.com/yoshuki)
//                                Akira Yoshida(acura1971@gmail.com)
// [ libron 1.x Mie version ] naoki.iimura(http://github.com/amatubu)
// [ libron 1.x Niigata version ] Shinichiro Oguma(http://github.com/ogumashin)
// [ libron 1.x Miyazaki version ] Seiya ISHIMARU (http://github.com/ishimaru-s)
// [ libron 1.x Shiga version ] sowt (http://sowt.on-air.ne.jp/)
// [ libron 1.x Aichi version ] noir.pur(noir.pur@gmail.com)
// [ カーリルAPI対応、 Chrome対応 ] koji miyata(http://twitter.com/kojimiya)
// 
//
// バージョン2.0より図書館の蔵書チェックに、日本最大の図書館検索サイト「カーリル」(http://calil.jp)のAPIを使用するようにしました。
// このAPIのおかげで対応図書館を一気に全国4300以上に増やすことができました。
// 「図書館をもっと便利に利用したい」という共通の目的を持つLibronは「カーリル」を応援していくことを表明します。
//
//
// 更新通知の仕組みについてはnoriaki氏のブログ記事
//
// Greasemonkeyスクリプトに自動更新機能をお手軽に付ける - We Ain't Seen Nothin' Yet.
// http://blog.fulltext-search.biz/archives/2008/08/update-checker-4-greasemonkey.html
//
// を参考にさせていただきました。(Greasemonkey版)
//
//
// Chromeに対応するためumezo氏が作られたgmWrapperを使わせていただきました。
//
// gmWrapperのソースをgithubで公開しました
// http://d.hatena.ne.jp/umezo/20091121/1258819422
//
// グリモンuserscriptをChromeに移植するときに使う、GM APIのラッパっぽいものとプロジェクトテンプレートを書いた
// http://d.hatena.ne.jp/umezo/20091121/1258819422
//
//
// == 重要なお知らせ ==
//
// 2.0.4以前のバージョンをご使用の場合は、お手数ですが2.0.5以降のバージョンにアップデートしてください。
// Amazonの検索結果に表示される各書籍の個別ページへのリンクにLibronのアソシエイトIDを付加していたところ、ユーザーの方から
// 規約違反ではないかというご指摘を受け、Amazonに確認したところ、「未申請サイトでのリンクの使用は禁止」という回答を得たので、
// 2.0.5にてアソシエイトIDを付加しないようにしました。
//
//
// == リリースノート ==
// 2.1.1
// - 図書館選択用プルダウンに新たに「図書館(大学)」カテゴリを用意しました。
// - Twitterへのつぶやき機能改善(長すぎるタイトルの省略。不要なHTMLタグの削除)
//
// 2.1.0
// - Firefox Add-on イニシャルバージョン
//
// 2.0.5
// - [重要な変更]Amazonの検索結果に表示される各書籍の個別ページへのリンクにLibronのアソシエイトIDを付加しないようにしました。
//
// 2.0.4
// - 「ほしい物リスト」(Wishlist)からも図書館の蔵書検索ができるようにしました。
//
// 2.0.3
// - 新潟県新潟市を始め、一部の地域で図書館が選択できなかった問題を修正しました。
//
// 2.0.2
// - Twitterにつぶやくときの文言に入る本のタイトルに、HTMLタグが含まれてしまうときがある問題を修正しました。
//
// 2.0.1
// - Twitter を使って、読みたい本を図書館にリクエストする Libreq(リブレク) と連携しました。 
// - 図書館を選択するときのプルダウンメニューで、あいうえお順に図書館が並ぶように修正しました。
//
// 2.0
// - カーリルAPIに対応しました。
// - UIを一新しました。

using System;
using System.Collections.Generic;
using System.Text;
using mshtml;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Xml;
using System.Collections;
using SHDocVw;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Data;
//using Libron;

namespace LibronToolbar
{
    class LibronClass
    {
        public string libronversion = "2.1.1";

        // http://ja.wikipedia.org/wiki/都道府県 の並び順
        public string[] prefectures = new[] {"北海道",
            "青森県", "岩手県", "宮城県", "秋田県", "山形県", "福島県",
            "茨城県", "栃木県", "群馬県", "埼玉県", "千葉県", "東京都", "神奈川県",
            "新潟県", "富山県", "石川県", "福井県", "山梨県", "長野県", "岐阜県", "静岡県", "愛知県",
            "三重県", "滋賀県", "京都府", "大阪府", "兵庫県", "奈良県", "和歌山県",
            "鳥取県", "島根県", "岡山県", "広島県", "山口県",
            "徳島県", "香川県", "愛媛県", "高知県",
            "福岡県", "佐賀県", "長崎県", "熊本県", "大分県", "宮崎県", "鹿児島県",
            "沖縄県"};

        //string libronLogo = "data:image/png;base64," +
        //   "iVBORw0KGgoAAAANSUhEUgAAAC0AAAAUCAMAAAAusUTNAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJ" +
        //   "bWFnZVJlYWR5ccllPAAAAYBQTFRFdHJu49/WjouGFBMTzcnB2tfObmxoWllWtbKraGZiwb637erj" +
        //   "hIJ+ZWRgp6SeXl1ZDAwLl5SPHBwbj42JxcO9ubaxYF5bAgICcG5rraulYmFd1dHJRkVDQkA+ZGJf" +
        //   "gX97pKKcJiYkkY6JLi4sXFtYNDMxLCsp6eXc6+jg6ebd6OTb7Onh7OjhKSgn7Oni6+ffRENAUlBN" +
        //   "7uvk7eni6uffsa6n6OTc6ubdnpyVPDs5IyMh4NzT6+fgnJmTqaagVVNQ2NTMxcG6ysa/V1ZSmZaQ" +
        //   "U1JPz8zET05L5ODY5eHY6OXcamhlbWtnSEdFx8S83dnR19TNfnx539zUfnt36ufh5+Td5uPciYeD" +
        //   "dnVweHZxNzY1y8jBSkhGm5iTIB8enpuW0s/G7uvlpKGb3tvUk5GLlJKM5uLbOTg24N3W4d7XrKmi" +
        //   "uLWu3drTMTAug4F8vLmzhYN/i4mDop+ZeXhze3l0JCQi5OHZ4t/Yw7+47urkOjo4Pz47hIJ96eXd" +
        //   "6ube6OTb4C1nnAAAAIB0Uk5T////////////////////////////////////////////////////" +
        //   "////////////////////////////////////////////////////////////////////////////" +
        //   "/////////////////////////////////////////wA4BUtnAAAB30lEQVR42rST13PaQBDGBULG" +
        //   "mEAE2DqIDQ6mnLqsQu8lbuCS3p3E6Z1Up93tv26JJC+Z8dh+yD7tffObvW/39hg4SzD/md45A/3Q" +
        //   "1ACflq7WUcGQTl27wf9Q6O9DL32y79zSH3oYL55M1y6prpPFSAhW4okB+wq4q53bWmcXfP48y6ZL" +
        //   "ANWCN4yh5tGrHt2PoHOkut+EwUyvhm5V+uhgbSmgKAsFNIzWEJqDgPlF9OjzoiQDpGO0fQ+6xVkK" +
        //   "6zHo17lsSlFFh8uxXxt8McsB5HWPFpoefeEBLqV/1flQAwS/v7A2ixYdXAZoobuQfbFd8wEzrW1u" +
        //   "eR0IMShp8+prPgIdc8B32yg17ayNXsJ++FBb3/xMPfq5z1ODEyi9xRgyY9lc4dgZjtcanp4cKcC+" +
        //   "MXq8OXbpzTqKhz6Muz8jlRYfbEWXD8lqUn+Kvn1E2Wog8C54zdlFF0X5CRq5TnbuJ/zCzcd7k8SN" +
        //   "xERYntfFjU+Xr1Mhe+V9JncnkworJDpJqQTnkzYjEzFsWWHFEHVVtK3vikNIU1fpxqiwp1iPLFus" +
        //   "YKrqBibPFhRGxqRCKSWYSIRME3nLTbFkHGwzc6pBsewiroolhx6731gSbXv6EH9DLpeP/w1l9ybp" +
        //   "n1U+EmAAVC9EkhaYTkMAAAAASUVORK5CYII=";

        string okIcon = "data:image/png;base64," +
           "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0" +
           "U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAKfSURBVDjLpZPrS1NhHMf9O3bOdmwDCWRE" +
           "IYKEUHsVJBI7mg3FvCxL09290jZj2EyLMnJexkgpLbPUanNOberU5taUMnHZUULMvelCtWF0sW/n" +
           "7MVMEiN64AsPD8/n83uucQDi/id/DBT4Dolypw/qsz0pTMbj/WHpiDgsdSUyUmeiPt2+V7SrIM+b" +
           "Sss8ySGdR4abQQv6lrui6VxsRonrGCS9VEjSQ9E7CtiqdOZ4UuTqnBHO1X7YXl6Daa4yGq7vWO1D" +
           "40wVDtj4kWQbn94myPGkCDPdSesczE2sCZShwl8CzcwZ6NiUs6n2nYX99T1cnKqA2EKui6+TwphA" +
           "5k4yqMayopU5mANV3lNQTBdCMVUA9VQh3GuDMHiVcLCS3J4jSLhCGmKCjBEx0xlshjXYhApfMZRP" +
           "5CyYD+UkG08+xt+4wLVQZA1tzxthm2tEfD3JxARH7QkbD1ZuozaggdZbxK5kAIsf5qGaKMTY2lAU" +
           "/rH5HW3PLsEwUYy+YCcERmIjJpDcpzb6l7th9KtQ69fi09ePUej9l7cx2DJbD7UrG3r3afQHOyCo" +
           "+V3QQzE35pvQvnAZukk5zL5qRL59jsKbPzdheXoBZc4saFhBS6AO7V4zqCpiawuptwQG+UAa7Ct3" +
           "UT0hh9p9EnXT5Vh6t4C22QaUDh6HwnECOmcO7K+6kW49DKqS2DrEZCtfuI+9GrNHg4fMHVSO5kE7" +
           "nAPVkAxKBxcOzsajpS4Yh4ohUPPWKTUh3PaQEptIOr6BiJjcZXCwktaAGfrRIpwblqOV3YKdhfXO" +
           "IvBLeREWpnd8ynsaSJoyESFphwTtfjN6X1jRO2+FxWtCWksqBApeiFIR9K6fiTpPiigDoadqCEag" +
           "5YUFKl6Yrciw0VOlhOivv/Ff8wtn0KzlebrUYwAAAABJRU5ErkJggg==";

        string ngIcon = "data:image/png;base64," +
           "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAAK/INwWK6QAAABl0RVh0" +
           "U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAHdSURBVDjLpZNraxpBFIb3a0ggISmmNISW" +
           "XmOboKihxpgUNGWNSpvaS6RpKL3Ry//Mh1wgf6PElaCyzq67O09nVjdVlJbSDy8Lw77PmfecMwZg" +
           "/I/GDw3DCo8HCkZl/RlgGA0e3Yfv7+DbAfLrW+SXOvLTG+SHV/gPbuMZRnsyIDL/OASziMxkkKkU" +
           "QTJJsLaGn8/iHz6nd+8mQv87Ahg2H9Th/BxZqxEkEgSrq/iVCvLsDK9awtvfxb2zjD2ARID+lVVl" +
           "babTgWYTv1rFL5fBUtHbbeTJCb3EQ3ovCnRC6xAgzJtOE+ztheYIEkqbFaS3vY2zuIj77AmtYYDu" +
           "sPy8/zuvunJkDKXM7tYWTiyGWFjAqeQnAD6+7ueNx/FLpRGAru7mcoj5ebqzszil7DggeF/DX1nB" +
           "N82rzPqrzbRayIsLhJqMPT2N83Sdy2GApwFqRN7jFPL0tF+10cDd3MTZ2AjNUkGCoyO6y9cRxfQo" +
           "wFUbpufr1ct4ZoHg+Dg067zduTmEbq4yi/UkYidDe+kaTcP4ObJIajksPd/eyx3c+N2rvPbMDPbU" +
           "FPZSLKzcGjKPrbJaDsu+dQO3msfZzeGY2TCvKGYQhdSYeeJjUt21dIcjXQ7U7Kv599f4j/oF55W4" +
           "g/2e3b8AAAAASUVORK5CYII=";

        string loadingIcon = "data:image/gif;base64," +
           "R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh" +
           "/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklr" +
           "E2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAA" +
           "EAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUk" +
           "KhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9" +
           "HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYum" +
           "CYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzII" +
           "unInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAA" +
           "ACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJ" +
           "ibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFG" +
           "xTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdce" +
           "CAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==";

        string calilIcon = "data:image/png;base64," +
           "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAAZiS0dEAP8A" +
           "/wD/oL2nkwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAAd0SU1FB9oEBg4oF3uKapIAAACRSURBVDjL" +
           "nVPbDcAgCDyJMziEC9URXaju0C5hPxoMUXzy40UOOFBMzhkAENL7g0WL3hkAoJNgGUMMonc71UsS" +
           "0hwrwWxWI4T0NkTtrsxgVmWkjmQFPhnXCiSnSSArcBs9+RKb6362X0GqoBlJa2c4gxrP/KUF5ZN0" +
           "sbQmwWgemjK7SuzOgLfqxKJ3huRqnqzzBwE6Xrqxh9tpAAAAAElFTkSuQmCC";

        string twitterIcon = "data:image/png;base64," +
           "iVBORw0KGgoAAAANSUhEUgAAABIAAAASCAYAAABWzo5XAAAACXBIWXMAAAsTAAALEwEAmpwYAAAD" +
           "iUlEQVQ4EU1TS28cRRis7umZ2Ze9uzbGa4wjHAeFCwKExIETAnKAC7lz4HFEypVzfgF3ToDE4waB" +
           "gxFEKGdugIWxsGSxhMS7Wcc7493ZeXX3UD1LJHr17fRjvuqq+r4R4Hhj/zjcWW++Pmg03/aEfMwa" +
           "Uwl3wCHxaLZc1/+eFFZUD+O0+PLobPPH798UuXjl5h31zLWr72122x+tBF5nnpewqGoAhyb4k4+w" +
           "qiVYxfN24GOe68U4Sj/c1Ecfq+0rGAzawQ0/CDpHZxEzBZ7thbjU8THJDA6iHLmtuE20/4Csu2qe" +
           "4am1XuvxjvngwbD9nVJ6cVlVevt0liCvKrzQC/DO3iravgSX2L83x7cMOHpcO3Yew3I9Ys6WMptJ" +
           "MtmVInnY1UbL3Fowl2wagJI4KSpMLPDyRhNPNBTWAol+KKEIUpChk695U6m1J6K4q+ZF7i+MQckD" +
           "n9f5vC4ywJSRMeFK4OHG1T4Mk4iBMeV+MbzAKNVQ3EutQZKXSkVpgVGS4y43WmSU6g5KMskYmpnH" +
           "ZNbyvNoeTWk7XYUX+wE+m85RpAKlLBHlCyjFlyyRF9qgqCwysksIMjVLBqPaYFcngCRRNGQtMys1" +
           "0krC+BVCeFABTz2P2i0XNFBQ3oIgMZkwB1v0ho8aiFNse8CdOCeIQBgoWlHBU7SVKKyCAHuMuOwZ" +
           "hit3TB0NOvtSiwkOicMx/2mcYH+UQErJfb7NIBUHxJI6EMeqLq9jBMwcEF+5X1g4Jm64x6DlY3cl" +
           "wCH7y+VJsTwkKQfkQOgVJaXGosu9pOScYJ+PnTNkw5Bk/mpP4a2tFfyz0LAEcWBu1HBOms8orMDh" +
           "rMTlEHiSelICnxPQxZQxyS3G9K6pPIS0xH0FNRCnqrJSKF7VIKvMSvwcFdg9W+DaehOvsctd/7gG" +
           "co+AiYrcPhnmmJFoN1SUrVkfYrSjsyhOs+n6Rr+bmQs2psA3pykO2QN7bR8tdnldfAIZdv8JJf1y" +
           "UdaF2Og0EZ9fxK3pg1g17v/x9+L46R9W2833n99Y8+fa1gbm7PDj0oPvupI+VKykofSCy72exArl" +
           "RVGkz/88uN386/e7ikaP1g9uf1XMJ/l0sPOcUYpflmCrCWj6kFEOLaib0ZJR/eVXlSisye3p8Lf+" +
           "ya9fJ8aMnOXi3evXu2p19VIjDLe1Ma1lGk8cwv/HsoA02K+M1lkg5b3Z+fnw01u34n8BTwW3oCOo" +
           "nOgAAAAASUVORK5CYII=";

        public string selectedSystemId;
        public string selectedSystemName;
        public string selectedPrefecture;
        Hashtable libraryNames = new Hashtable();
        Hashtable libraries = new Hashtable();
        Hashtable systemNames = new Hashtable();
        public HTMLDocument document = null;
        public SHDocVw.WebBrowser Explorer;
        public delegate void SavedataRecievedEventhandler(GetDataState state);
        public event SavedataRecievedEventhandler SaveDataRecieved;
        public delegate void DownloadedLibraryNamesEventHandler(List<Hashtable> libraryNames);
        public event DownloadedLibraryNamesEventHandler DownloadedLibrayNames;
        public event EventHandler CheckLibraryError;
        public event EventHandler SetSaveDataError;
        public int retryCnt = 3;

        public DataTable tblErrorLog = new DataTable();

        private class ReqState
        {
            public WebRequest Request;
            public string UserId;
        }

        public enum GetDataState
        {
            Success,
            Timeout,
            Error
        }

        string appkey = "73ec9cd9e4b62b65b9549dc173750e9c";

        public void initializelibron()
        {
            selectedPrefecture = "東京都";
            selectedSystemId = "Tokyo_Pref";
            selectedSystemName = "東京都立図書館";

            tblErrorLog.Columns.Add("No", typeof(int));
            tblErrorLog.Columns.Add("ErrorMessage", typeof(string));
            tblErrorLog.DefaultView.Sort = "No DESC";
        }

        private void AddErrorLog(Exception ex)
        {
            try
            {
                int no = 1;
                if(tblErrorLog.Rows.Count > 0 )
                    no = (int)tblErrorLog.DefaultView[0]["No"] + 1;
                DataRow row = tblErrorLog.NewRow();
                row["No"] = no;
                row["ErrorMessage"] = ex.Message;
                tblErrorLog.Rows.Add(row);
                if (tblErrorLog.Rows.Count > 50)
                    tblErrorLog.Rows.RemoveAt(0);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message);
            }

        }

        public void GetSaveData(string userId)
        {
            WebRequest req = WebRequest.Create("http://libron.net/api/storage?key=" + userId);
            ReqState state = new ReqState() { Request = req, UserId = userId };
            IAsyncResult iares = req.BeginGetResponse(new AsyncCallback(RecieveCallBack), state);
            ThreadPool.RegisterWaitForSingleObject(iares.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), req, 2000, true);
        }

        public void SetSaveData(string userId)
        {
            try
            {
   
                string saveValue = HttpUtility.UrlEncode(selectedPrefecture + "@" + selectedSystemId + "@" + selectedSystemName);
                string url = "http://libron.net/api/storage?key=" + userId + "&value=" + saveValue;
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                retryCnt--;
                if (retryCnt > -1)
                {
                    SetSaveData(userId);
                }
                else
                {
                    retryCnt = 3;
                    SetSaveDataError(this, null);
                }
            }
        }

        void RecieveCallBack(IAsyncResult iaRes)
        {
            try
            {
                ReqState state = (ReqState)iaRes.AsyncState;
                WebResponse response = state.Request.EndGetResponse(iaRes);
                Stream s = (Stream)response.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string strRes = sr.ReadToEnd();
                Hashtable saveData = (Hashtable)JSON.JsonDecode(strRes);
                string[] saveValues = ((string)saveData["value"]).Split('@');
                selectedPrefecture = saveValues[0];
                selectedSystemId = saveValues[1];
                selectedSystemName = saveValues[2];
                response.Close();
                s.Close();
                sr.Close();
                SaveDataRecieved(GetDataState.Success);
            }
            catch(Exception ex) 
            {
                AddErrorLog(ex);
                retryCnt--;
                if (retryCnt > -1)
                {
                    GetSaveData(((ReqState)iaRes.AsyncState).UserId);
                }
                else
                {
                    retryCnt = 3;
                    SaveDataRecieved(GetDataState.Error);
                }
                //SaveDataRecieved(GetDataState.Error);
            }
        }

        void TimeoutCallback(object req, bool timedOut)
        {
            if (timedOut)
            {
                if (req != null)
                    ((WebRequest)req).Abort();
                SaveDataRecieved(GetDataState.Timeout);
            }
        }
       
        public void DownloadLibraryNames()
        {
            try
            {
                string url = "http://api.calil.jp/library?appkey=" + appkey + "&pref=" + HttpUtility.UrlEncode(selectedPrefecture) + "&format=xml";
                WebClient lwc = new WebClient();
                lwc.Encoding = Encoding.UTF8;
                lwc.DownloadStringCompleted += (ls, le) =>
                {
                    WebClient cwc = new WebClient();
                    cwc.Encoding = Encoding.UTF8;
                    cwc.DownloadStringCompleted += (cs, ce) =>
                    {
                        Regex reg = new Regex(@"loadcity\((.*)\);$");
                        Match m = reg.Match(ce.Result);
                        if (m.Success && m.Groups.Count == 2)
                        {
                            Hashtable cities = (Hashtable)JSON.JsonDecode(m.Groups[1].Value);
                            XmlDocument ldoc = new XmlDocument();
                            ldoc.LoadXml(le.Result);
                            libraries[selectedPrefecture] = ldoc;
                            libraryNames[selectedPrefecture] = createLibraryNames(libraries, (Hashtable)cities[selectedPrefecture]);
                            DownloadedLibrayNames((List<Hashtable>)libraryNames[selectedPrefecture]);
                        }
                    };
                    cwc.DownloadStringAsync(new Uri("http://calil.jp/city_list"));
                };
                lwc.DownloadStringAsync(new Uri(url));
            }
            catch(Exception ex)
            {
                AddErrorLog(ex);
                retryCnt--;
                if (retryCnt > -1)
                {
                    DownloadedLibrayNames(null);
                }
                else
                {
                    retryCnt = 3;
                }
            }
        }

        List<Hashtable> createLibraryNames(Hashtable libraries, Hashtable cities)
        {
            XmlDocument libdoc = (XmlDocument)libraries[selectedPrefecture];

            Hashtable smallMediumLibrariesObject = new Hashtable();
            List<Hashtable> smallMediumLibraries = new List<Hashtable>();
            List<Hashtable> largeLibraries = new List<Hashtable>();
            List<Hashtable> univLibiraries = new List<Hashtable>();
            List<Hashtable> otherLibraries = new List<Hashtable>();

            XmlNodeList libs = ((XmlDocument)libraries[selectedPrefecture]).SelectNodes("Libraries/Library");
            foreach (XmlNode library in libs)
            {
                string city = library.SelectSingleNode("city").InnerText;
                string category = library.SelectSingleNode("category").InnerText;
                string systemid = library.SelectSingleNode("systemid").InnerText;
                string systemname = library.SelectSingleNode("systemname").InnerText;
                Hashtable data = new Hashtable();
                data["systemid"] = library.SelectSingleNode("systemid").InnerText;
                data["systemname"] = library.SelectSingleNode("systemname").InnerText;

                if (category == "SMALL" || category == "MEDIUM")
                {
                    if (smallMediumLibrariesObject[city] != null)
                    {
                        ((List<Hashtable>)smallMediumLibrariesObject[city]).Add(data);
                    }
                    else
                    {
                        smallMediumLibrariesObject[city] = new List<Hashtable>();
                        ((List<Hashtable>)smallMediumLibrariesObject[city]).Add(data);
                    }
                }
                else if (category == "LARGE")
                {
                    largeLibraries.Add(data);
                }
                else if (category == "UNIV")
                {
                    univLibiraries.Add(data);
                }
                else
                {
                    otherLibraries.Add(data);
                }
            }
            string[] kanas = new[] { "あ", "か", "さ", "た", "な", "は", "ま", "や", "ら", "わ" };
            foreach (string kana in kanas)
            {
                if (cities[kana] != null)
                {
                    foreach (object city_name in (ArrayList)cities[kana])
                    {
                        if (smallMediumLibrariesObject[(string)city_name] != null)
                        {
                            smallMediumLibraries.AddRange(((List<Hashtable>)smallMediumLibrariesObject[(string)city_name]).ToArray());
                        }
                    }
                }
            }

            List<Hashtable> libraryNamesArray = new List<Hashtable>();

            foreach (Hashtable smallMediumLibrary in smallMediumLibraries)
            {
                if (systemNames[smallMediumLibrary["systemid"]] != null)
                {
                    continue;
                }
                smallMediumLibrary["group"] = "図書館(地域)";
                libraryNamesArray.Add(smallMediumLibrary);
                systemNames[smallMediumLibrary["systemid"]] = smallMediumLibrary["systemname"];
            }

            foreach (Hashtable largeLibrary in largeLibraries)
            {
                if (systemNames[largeLibrary["systemid"]] != null)
                {
                    continue;
                }
                largeLibrary["group"] = "図書館(広域)";
                libraryNamesArray.Add(largeLibrary);
                systemNames[largeLibrary["systemid"]] = largeLibrary["systemname"];
            }

            foreach (Hashtable univLibrary in univLibiraries)
            {
                if (systemNames[univLibrary["systemid"]] != null)
                {
                    continue;
                }
                univLibrary["group"] = "図書館(大学)";
                libraryNamesArray.Add(univLibrary);
                systemNames[univLibrary["systemid"]] = univLibrary["systemname"];
            }

            foreach (Hashtable otherLibrary in otherLibraries)
            {
                if (systemNames[otherLibrary["systemid"]] != null)
                {
                    continue;
                }
                otherLibrary["group"] = "移動・その他";
                libraryNamesArray.Add(otherLibrary);
                systemNames[otherLibrary["systemid"]] = otherLibrary["systemname"];
            }
            return libraryNamesArray;
        }
        
        public void libraryLinky()
        {
            try
            {
                string href = document.location.href;
                Regex reg = new Regex(@"/(dp|ASIN|product)/([\dX]{10})");
                Match m = reg.Match(href);

                string title;
                if (m.Success && m.Groups.Count == 3)
                {
                    string isbn = m.Groups[2].Value;
                    HTMLDivElement div = (HTMLDivElement)document.getElementById("btAsinTitle").parentElement.parentElement;
                    title = truncate(Regex.Replace(document.getElementById("btAsinTitle").innerHTML, "</?[^>]+>", ""));
                    addLoadingIcon((IHTMLDOMNode)div);
                    string url = "http://api.calil.jp/check?appkey=" + appkey + "&isbn=" + isbn + "&systemid=" + selectedSystemId + "&format=xml";
                    checkLibrary(url, (IHTMLDOMNode)div, isbn, title);
                }
                else if ((href.IndexOf("wishlist") != -1) || (href.IndexOf("/s?") != -1) || (href.IndexOf("/s/") != -1) || (href.IndexOf("/exec/") != -1) || (href.IndexOf("/gp/search") != -1))
                {
                    IHTMLElementCollection objects = null;
                    if (href.IndexOf("wishlist") != -1)
                    {
                        objects = (IHTMLElementCollection)document.getElementsByTagName("span");
                    }
                    else
                    {
                        objects = (IHTMLElementCollection)document.getElementsByTagName("div");
                    }
                    if (objects != null)
                    {
                        IEnumerator objEnum = objects.GetEnumerator();
                        while (objEnum.MoveNext())
                        {
                            IHTMLElement obj = (IHTMLElement)objEnum.Current;
                            if (obj.className == null) continue;
                            if (obj.className.IndexOf("productTitle") != -1)
                            {
                                IHTMLDOMChildrenCollection childs = null;
                                if (((IHTMLElement)obj).tagName.ToLower() == "span")
                                {
                                    childs = (IHTMLDOMChildrenCollection)((HTMLSpanElement)obj).childNodes;
                                }
                                else
                                {
                                    childs = (IHTMLDOMChildrenCollection)((HTMLDivElement)obj).childNodes;
                                }
                                IEnumerator childEnum = childs.GetEnumerator();
                                while (childEnum.MoveNext())
                                {
                                    if (((IHTMLElement)childEnum.Current).tagName.ToLower() == "a")
                                    {
                                        HTMLAnchorElement link = (HTMLAnchorElement)childEnum.Current;
                                        if (link != null)
                                        {
                                            reg = new Regex("<span title='(.+)'>");
                                            m = reg.Match(link.innerHTML);
                                            if (m.Success && m.Groups.Count == 2)
                                            {
                                                title = truncate(stripTags(m.Groups[1].Value.Trim()));
                                            }
                                            else
                                            {
                                                title = truncate(stripTags(Regex.Replace(link.innerHTML, @"<\w[^>]*?>", "")));
                                            }

                                            reg = new Regex(@"/dp/([\dX]{10})/ref");
                                            m = reg.Match(link.href);
                                            if (m.Success && m.Groups.Count == 2)
                                            {
                                                string isbn = m.Groups[1].Value;
                                                addLoadingIcon((IHTMLDOMNode)obj);
                                                string url = "http://api.calil.jp/check?appkey=" + appkey + "&isbn=" + isbn + "&systemid=" + selectedSystemId + "&format=xml";
                                                checkLibrary(url, (IHTMLDOMNode)obj, isbn, title);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
        }

        void addLoadingIcon(IHTMLDOMNode div)
        {
            HTMLDivElement loadingIconDivOuter = (HTMLDivElement)document.createElement("div");
            loadingIconDivOuter.className = "libron_loading_icon_div_outer";
            loadingIconDivOuter.style.cssText = "padding:7px;border:1px solid #cbc6bd;background:#e8e4db;-moz-border-radius:5px;-webkit-border-radius:5px;font-size:12px;";
            HTMLDivElement loadingIconDiv = (HTMLDivElement)document.createElement("div");
            loadingIconDiv.className = "libron_loading_icon_div";
            loadingIconDiv.innerHTML = "<span style='color:#666;'>図書館を検索中</span> <image style='vertical-align:middle;' src='" + loadingIcon + "'><br /><br /><br />";
            loadingIconDivOuter.appendChild((IHTMLDOMNode)loadingIconDiv);
            div.appendChild((IHTMLDOMNode)loadingIconDivOuter);
        }

        void removeLoadingIcon(IHTMLDOMNode div)
        {
            for (int i = 0; i < ((IHTMLDOMChildrenCollection)div.childNodes).length; i++)
            {
                object elm = ((IHTMLDOMChildrenCollection)div.childNodes).item(i);
                if (elm.GetType().Name == "HTMLDivElementClass")
                {
                    if (((HTMLDivElement)elm).className == "libron_loading_icon_div_outer")
                    {
                        div.removeChild((IHTMLDOMNode)elm);
                        return;
                    }

                }
            }
        }

        void checkLibrary(string url, IHTMLDOMNode div, string isbn, string title)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.DownloadStringCompleted += (s, e) =>
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(e.Result);
                    XmlNode apiContinue = xdoc.SelectSingleNode("result/continue");
                    if (apiContinue.InnerText == "0")
                    {
                        addLink(xdoc, (IHTMLDOMNode)div, isbn, title);
                    }
                    else
                    {
                        XmlNode apiSession = xdoc.SelectSingleNode("result/session");
                        if (apiSession.InnerText.Length > 0)
                        {
                            string new_url = "http://api.calil.jp/check?appkey=" + appkey + "&session=" + apiSession.InnerText + "&format=xml";
                            checkLibrary(url, (IHTMLDOMNode)div, isbn, title);
                        }
                    }

                };
                wc.DownloadStringAsync(new Uri(url));
            }
            catch
            {
                CheckLibraryError(this, null);
            }
        }

        void addLink(XmlDocument xdoc, IHTMLDOMNode div, string isbn, string title)
        {
            removeLoadingIcon((IHTMLDOMNode)div);
            XmlNode apibookstatus = xdoc.SelectSingleNode("result/books/book[@isbn='" + isbn + "']/system[@systemid='" + selectedSystemId + "']/status");
            XmlNode libkeys = null;
            List<string> calil_library_links = new List<string>();
            HTMLDivElement linkDivOuter = (HTMLDivElement)document.createElement("div");
            HTMLDivElement linkDiv = (HTMLDivElement)document.createElement("div");
            linkDiv.className = "libron_link_div";
            linkDiv.style.cssText = "padding:7px;border:1px solid #cbc6bd;background:#e8e4db;-moz-border-radius:5px;-webkit-border-radius:5px;font-size:12px;";
            string tweet_body = null;
            string twitter_link_inner = null;
            string libreq_link = "";

            if (apibookstatus != null && apibookstatus.InnerText == "Error")
            {
                HTMLDivElement error_link = (HTMLDivElement)document.createElement("div");
                error_link.innerHTML = "<span>エラーが発生しました <image src='" + ngIcon + "'></span>";
                linkDiv.appendChild((IHTMLDOMNode)error_link);
            }
            else
            {
                XmlNode reserveurl = null;
                reserveurl = xdoc.SelectSingleNode("result/books/book[@isbn='" + isbn + "']/system[@systemid='" + selectedSystemId + "']/reserveurl");
                libkeys = xdoc.SelectSingleNode("result/books/book[@isbn='" + isbn + "']/system[@systemid='" + selectedSystemId + "']/libkeys");
                foreach (XmlNode libkey in libkeys.ChildNodes)
                {
                    string calil_library_link = "<a href='http://calil.jp/library/search?s=" + selectedSystemId + "&k=" + HttpUtility.UrlEncode(libkey.InnerText) + "' target='_blank'>" + libkey.InnerText + "(" + libkey + ")" + "</a>";
                    calil_library_links.Add(calil_library_link);
                }
                if (calil_library_links.Count > 0)
                {
                    HTMLDivElement ok_link = (HTMLDivElement)document.createElement("div");
                    if (reserveurl != null)
                    {
                        ok_link.innerHTML = "<span>&raquo; <a target='_blank' href='" + reserveurl.InnerText + "'>" + selectedSystemName + "で予約する</a> <image style='vertical-align:middle;' src='" + okIcon + "'></span>";
                    }
                    else
                    {
                        ok_link.innerHTML = "<span style='color:#666666;'>" + selectedSystemName + "に蔵書あり <image style='vertical-align:middle;' src='" + okIcon + "'> " + string.Join("・", calil_library_links.ToArray()) + "</span>";
                    }
                    linkDiv.appendChild((IHTMLDOMNode)ok_link);
                    tweet_body = "「" + title + "」を" + selectedSystemName + "の図書館" + "で予約しました。 http://libron.net";
                    twitter_link_inner = "「予約したよ」とつぶやく";
                }
                else
                {
                    HTMLDivElement ng_message = (HTMLDivElement)document.createElement("div");
                    ng_message.innerHTML = "<span style='color:#666666;'>" + selectedSystemName + "には見つかりません <image style='vertical-align:middle;' src='" + ngIcon + "'></span>";
                    linkDiv.appendChild((IHTMLDOMNode)ng_message);
                    tweet_body = "@libreq " + isbn + " " + selectedSystemName + " 「" + title + "」を図書館にリクエスト。 http://libreq.net";
                    libreq_link = " <a href='http://libreq.net/top/about' target='_blank'>[これは何?]</a>";
                    twitter_link_inner = "リクエストをつぶやく(借りられるようになったら通知を受け取れます)";
                }
            }

            HTMLDivElement calil_link = (HTMLDivElement)document.createElement("div");
            calil_link.style.marginTop = "3px";
            string calil_url = "http://calil.jp/book/" + isbn;
            calil_link.innerHTML = "<span>&raquo; <a target='_blank' href='" + calil_url + "'>他の図書館で検索する(カーリル)</a> <image style='vertical-align:middle;' src='" + calilIcon + "'> </span>";
            linkDiv.appendChild((IHTMLDOMNode)calil_link);

            if (tweet_body != null)
            {
                HTMLDivElement twitter_link = (HTMLDivElement)document.createElement("div");
                twitter_link.style.marginTop = "3px";

                string twitter_url = "http://twitter.com/home?status=" + HttpUtility.UrlEncode(tweet_body);
                twitter_link.innerHTML = "<span>&raquo; <a target='_blank' href='" + twitter_url + "'>" + twitter_link_inner + "</a>" + libreq_link + " <image style='vertical-align:middle;' src='" + twitterIcon + "'> </span>";
                linkDiv.appendChild((IHTMLDOMNode)twitter_link);
            }

            linkDivOuter.appendChild((IHTMLDOMNode)linkDiv);
            div.appendChild((IHTMLDOMNode)linkDivOuter);
        }

        string truncate(string str)
        {
            if (str.Length < 28)
            {
                return str;
            }
            else
            {
                return str.Substring(0, 27) + "...";
            }
        }

        string stripTags(string str)
        {
            return Regex.Replace(str, "</?[^>]+>", "");
        }

        #region 未使用関数
        //void addSelectBox()
        //{
        //    HTMLDivElement div = (HTMLDivElement)document.createElement("div");
        //    div.id = "libron_title_outer";
        //    div.style.cssText = "text-align:right;border:1px solid #cbc6bd;background:#e8e4db;-moz-border-radius:5px;-webkit-border-radius:5px;font-size:14px;padding:7px;";

        //    HTMLDivElement titleDiv = (HTMLDivElement)document.createElement("div");
        //    titleDiv.id = "libron_title";

        //    HTMLSpanElement titleSpan = (HTMLSpanElement)document.createElement("span");
        //    titleSpan.innerHTML = "<image style='vertical-align:-5px;' src='" + libronLogo + "'> ver." + libronversion + " ";
        //    titleSpan.style.fontWeight = "bold";
        //    titleSpan.style.color = "#e47911";

        //    HTMLSpanElement currentLibrary = (HTMLSpanElement)document.createElement("span");
        //    currentLibrary.innerHTML = "[" + selectedPrefecture + "]" + selectedSystemName + "で検索 ";
        //    currentLibrary.style.color = "#666666";

        //    HTMLButtonElement showLink = (HTMLButtonElement)document.createElement("button");
        //    //showLink.href = "javascript:void(0);";
        //    //HTMLAnchorEvents_Event showLinkEventHandler = showLink as HTMLAnchorEvents_Event;
        //    //showLinkEventHandler.onclick += new HTMLAnchorEvents_onclickEventHandler(showSelectBox);
        //    showLink.innerHTML = "変更";

        //    titleDiv.appendChild((IHTMLDOMNode)titleSpan);
        //    titleDiv.appendChild((IHTMLDOMNode)currentLibrary);
        //    titleDiv.appendChild((IHTMLDOMNode)showLink);

        //    HTMLDivElement selectBoxDiv = (HTMLDivElement)document.createElement("div");

        //    HTMLSelectElement prefectureSelect = (HTMLSelectElement)document.createElement("select");
        //    prefectureSelect.style.marginLeft = "10px";

        //    foreach (string pref in prefectures)
        //    {
        //        HTMLOptionElement option = (HTMLOptionElement)document.createElement("option");
        //        option.value = pref;
        //        option.innerHTML = pref;
        //        if (pref == selectedPrefecture)
        //        {
        //            option.selected = true;
        //        }
        //        prefectureSelect.appendChild((IHTMLDOMNode)option);
        //    }

        //    HTMLSpanElement loadingMessage = (HTMLSpanElement)document.createElement("span");
        //    loadingMessage.style.marginLeft = "10px";
        //    loadingMessage.style.color = "#e47911";
        //    loadingMessage.style.paddingRight = "70px";
        //    loadingMessage.innerText = "データ取得中...";

        //    HTMLButtonElement btn = (HTMLButtonElement)document.createElement("button");
        //    btn.style.marginLeft = "10px";
        //    btn.innerHTML = "保存";

        //    HTMLAnchorElement hideLink = (HTMLAnchorElement)document.createElement("a");
        //    hideLink.style.margin = "0 0 0 3px";
        //    hideLink.href = "return false";
        //    HTMLAnchorEvents_Event hideLinkEventHandler = (HTMLAnchorEvents_Event)hideLink;
        //    hideLinkEventHandler.onclick += new HTMLAnchorEvents_onclickEventHandler(hideSelectBox);
        //    hideLink.innerText = "キャンセル";

        //    selectBoxDiv.appendChild((IHTMLDOMNode)prefectureSelect);
        //    selectBoxDiv.appendChild((IHTMLDOMNode)loadingMessage);
        //    selectBoxDiv.appendChild((IHTMLDOMNode)btn);
        //    selectBoxDiv.appendChild((IHTMLDOMNode)hideLink);
        //    selectBoxDiv.id = "libron_select_box";
        //    selectBoxDiv.style.display = "none";

        //    //updateLibrarySelectBox(selectBoxDiv);
        //    HTMLSelectElementEvents_Event prefectureSelectEventHandler = (HTMLSelectElementEvents_Event)prefectureSelect;
        //    if (prefectureSelectEventHandler != null)
        //    {
        //        prefectureSelectEventHandler.onchange += () =>
        //        {
        //            selectBoxDiv.replaceChild((IHTMLDOMNode)loadingMessage, (IHTMLDOMNode)((IHTMLElementCollection)selectBoxDiv.childNodes).item(null, 1));
        //            selectedPrefecture = prefectureSelect.value;
        //            //updateLibrarySelectBox(selectBoxDiv, prefectureSelect.value);
        //        };
        //    }


        //    div.appendChild((IHTMLDOMNode)titleDiv);
        //    div.appendChild((IHTMLDOMNode)selectBoxDiv);

        //    // iframe内にはセレクトボックスを表示しない
        //    if (true)
        //    {
        //        ((IHTMLDOMNode)document.body).insertBefore((IHTMLDOMNode)div, (IHTMLDOMNode)((IHTMLElementCollection)document.body.children).item(null, 0));
        //    }

        //    HTMLButtonElementEvents_Event showLinkEventHandler = showLink as HTMLButtonElementEvents_Event;
        //    showLink.setAttribute("onmoousedown", "javascript:document.getElementById('libron_title').style.display='none';document.getElementById('libron_select_box').style.display='block';", 0);
        //    //showLink.href = "return false;";
        //    //showLinkEventHandler.onclick += new HTMLButtonElementEvents_onclickEventHandler(delegate()
        //    //    {
        //    //        document.getElementById("libron_title").style.display = "none";
        //    //        document.getElementById("libron_select_box").style.display = "block";
        //    //        return false;
        //    //    });


        //    HTMLButtonElementEvents_Event btnEventHandler = (HTMLButtonElementEvents_Event)btn;
        //    if (btnEventHandler != null)
        //    {
        //        btnEventHandler.onclick += () =>
        //        {
        //            Hashtable options = new Hashtable();
        //            options["prefecture"] = prefectureSelect.value;
        //            options["systemid"] = ((HTMLSelectElement)((IHTMLDOMChildrenCollection)selectBoxDiv.childNodes).item(1)).value;
        //            options["systemname"] = systemNames[(string)options["systemid"]];
        //            saveSelection(options);
        //            object o1,o2,o3, o4;
        //            o1 = null;
        //            o2 = null;
        //            o3 = null;
        //            o4 = null;
        //            MessageBox.Show(document.location.href);
        //            this.Explorer.Navigate(document.location.href, ref o1, ref o2, ref o3, ref o4);
        //            return false;
        //        };
        //    }

        //}

        //bool showSelectBox()
        //{
        //    document.getElementById("libron_title").style.display = "none";
        //    document.getElementById("libron_select_box").style.display = "block";
        //    return false;
        //}

        //bool hideSelectBox()
        //{
        //    document.getElementById("libron_title").style.display = "block";
        //    document.getElementById("libron_select_box").style.display = "none";
        //    return false;
        //}

        //void updateLibrarySelectBox(HTMLDivElement selectBoxDiv)
        //{
        //    try
        //    {
        //        if (libraryNames[selectedPrefecture] != null)
        //        {
        //            IHTMLDOMNode selectBox = (IHTMLDOMNode)createLibrarySelectBox((List<Hashtable>)libraryNames[selectedPrefecture]);
        //            if(selectBox != null)
        //                selectBoxDiv.removeChild((IHTMLDOMNode)createLibrarySelectBox((List<Hashtable>)libraryNames[selectedPrefecture]));
        //        }
        //        else
        //        {
        //            string url = "http://api.calil.jp/library?appkey=" + appkey + "&pref=" + HttpUtility.UrlEncode(selectedPrefecture) + "&format=xml";
        //            WebClient lwc = new WebClient();
        //            lwc.Encoding = Encoding.UTF8;
        //            lwc.DownloadStringCompleted += (ls, le) =>
        //            {
        //                WebClient cwc = new WebClient();
        //                cwc.Encoding = Encoding.UTF8;
        //                cwc.DownloadStringCompleted += (cs, ce) =>
        //                {
        //                    Regex reg = new Regex(@"loadcity\((.*)\);$");
        //                    Match m = reg.Match(ce.Result);
        //                    if (m.Success && m.Groups.Count == 2)
        //                    {
        //                        Hashtable cities = (Hashtable)JSON.JsonDecode(m.Groups[1].Value);
        //                        XmlDocument ldoc = new XmlDocument();
        //                        ldoc.LoadXml(le.Result);
        //                        libraries[selectedPrefecture] = ldoc;
        //                        libraryNames[selectedPrefecture] = createLibraryNames(libraries, (Hashtable)cities[selectedPrefecture]);
        //                        IHTMLDOMNode selectBox = (IHTMLDOMNode)createLibrarySelectBox((List<Hashtable>)libraryNames[selectedPrefecture]);
        //                        if (selectBox != null)
        //                            selectBoxDiv.replaceChild(selectBox, (IHTMLDOMNode)((IHTMLDOMChildrenCollection)selectBoxDiv.childNodes).item(1));
        //                    }
        //                };
        //                cwc.DownloadStringAsync(new Uri("http://calil.jp/city_list"));
        //            };
        //            lwc.DownloadStringAsync(new Uri(url));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddErrorLog(ex);
        //    }
        //}

        //HTMLSelectElement createLibrarySelectBox(List<Hashtable> libNames)
        //{
        //    try
        //    {
        //        HTMLSelectElement select = (HTMLSelectElement)document.createElement("select");
        //        select.style.marginLeft = "10px";
        //        string[] groups = new[] { "図書館(地域)", "図書館(広域)", "図書館(大学)", "移動・その他" };

        //        Hashtable optGroups = new Hashtable();
        //        foreach (string group in groups)
        //        {
        //            optGroups[group] = (HTMLOptionElement)document.createElement("optgroup");
        //            ((HTMLOptionElement)optGroups[group]).label = group;
        //        }

        //        foreach (Hashtable libraryName in libNames)
        //        {
        //            HTMLOptionElement option = (HTMLOptionElement)document.createElement("option");
        //            option.value = (string)libraryName["systemid"];
        //            option.innerHTML = (string)libraryName["systemname"];

        //            if ((string)libraryName["systemid"] == selectedSystemId)
        //            {
        //                option.selected = true;
        //            }

        //            ((HTMLOptionElement)optGroups[libraryName["group"]]).appendChild((IHTMLDOMNode)option);
        //        }

        //        foreach (string group in groups)
        //        {
        //            if (((IHTMLDOMChildrenCollection)((HTMLOptionElement)optGroups[group]).childNodes).length > 0)
        //            {
        //                select.appendChild((IHTMLDOMNode)optGroups[group]);
        //            }
        //        }

        //        return select;
        //    }
        //    catch (Exception ex)
        //    {
        //        AddErrorLog(ex);
        //        return null;
        //    }
        //}

        #endregion
    }
}
