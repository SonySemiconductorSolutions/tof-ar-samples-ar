## 目次
* [ToF ARについて](#about)
* [ToF AR Samples ARの概要](#overview)
* [コンポーネント](#component)
* [アセット](#assets)
* [開発環境](#environment)
* [注意事項](#notes)
* [コントリビューション](#contributing)

<a name="about"></a>
# ToF ARについて

ToF AR は、Time-of-Flight(ToF)/Light detection and ranging(Lidar)などの距離計測センサーを持つiOS/Andoroidスマートフォン向けの、Unity用ツールキットライブラリです。

サンプルアプリケーションのビルドと実行には、この ToF AR の他に、UnityとToFセンサを搭載した対応スマートフォンが必要です。

ToF AR のパッケージや開発ドキュメント、ToF ARを使ったアプリケーションソフト、対応スマートフォンのリストにつきましては、

[ToF AR サイト](https://tof-ar.com/)をご覧ください。


<a name="overview"></a>
# ToF AR Samples ARの概要

**ToF AR Samples AR** は ToF AR の機能を使ったサンプルアプリケーションを提供しています。

<img src="/Images/topmenu.jpg" width="150">

各シーンは、アプリケーショントップ画面のシーン一覧から選んでください。

シーンからトップ画面に戻るには、4本の指で画面をタップします。


## サンプルシーン一覧

### SimpleARFoundation

TofARとUnityARFoundationの機能を併用したシンプルなシーン。

<img src="/Images/06_SimpleARFoundation.jpg" width="500">

Hand、Mesh、Modelingそれぞれの動作を確認することができます。

### Puppet

腕の位置に合わせてパペットを表示します。

<img src="/Images/07_Puppet.jpg" width="150">

### Hand Decoration
ハンド認識を利用して、手の甲に模様を表示します。

<img src="/Images/05_Handmark.jpg" width="500">

手の甲をカメラに向けることで、手の甲に模様を表示します。

### Rock Paper Scissors

ハンド認識を利用したじゃんけんゲーム。

<img src="/Images/01_RPS.jpg" width="500">

サムズアップジェスチャーを認識すると、ガイダンス音声を再生し、ゲームを開始します。

### Juggling
フェイス、ハンド認識を利用したジャグリング。

<img src="/Images/02_juggling.jpg" width="500">

セットアップUIに従い、両手を正しい位置に持っていくと、ゲームを開始します。

ボールが落下してきますので、キャッチしてください。

ボールを持っている手を上に振ると、反対側の手に向かってボールが投げられます。

ボールを5回キャッチする度に、新しいボールを追加します。


### BGChange
ハンドや空を検出し、背景を変化させます。

<img src="/Images/04_BGC.jpg" width="500">

サムズアップジェスチャーを認識すると、背景を変化させます。


### TextureRoom
認識した環境のMeshに対してテキストアニメーションや写真をマッピングします。

<img src="/Images/03_TextureRoom.jpg" width="500">

モードは右下の手のアイコンを押し、"Mode DropDown"から変更可能です。

* TextureAnimationモード：
  認識した環境のMeshに対してテキストアニメーションをマッピングします。

* TextInputモード：
  左下にinputfieldが表示します。こちらにテキストを入力すると入力したテキストをマッピングします。

* Stampモード：
  "Add Stamp Button"を押すことでカメラロールから写真を選択でき(複数選択可)、マッピングすることができます。


### BallPool

認識した空間にARボールを出現させます。

<img src="/Images/09_BallPool.jpg" width="500">

シーン起動後、空間の認識を行い3DMeshを作成します。

BallのToggleをOnにすると画面上部からボールが出現します。

手や足を画面上に映すと、ボールに触れることが出来ます。

### StepOn

接触した場所から草花が出現します。

<img src="/Images/10_StepOn.jpg" width="500">

シーン起動後、空間の平面認識を行います。

手や足が壁、床に触れるとその場所に草花が出現するアニメーションが表示されます。

<a name="component"></a>
# コンポーネント

サンプルアプリケーションのシーンと、各シーンが利用するToF AR コンポーネントの関係を示すテーブルです。縦にシーン名、横にコンポート名を並べています。チェックマークは、コンポーネント利用を示します。

||ToF|Color|Mesh|Coordinate|Hand|MarkRecog|Body|Segmentation|Face|Plane|Modeling|
|:--|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|SimpleARFoundation  |✓|✓|✓|✓|✓|  |  |  |  |  |  |
|Puppet     　　　　　|✓|✓|  |  |✓|  |  |  |  |  |  |
|Hand Decoration     |✓|✓|  |  |✓|  |  |  |  |  |  |
|Rock Paper Scissors |✓|✓|  |  |✓|  |  |  |  |  |  |
|Juggling            |✓|✓|  |　|✓|  |  |  |✓|  |  |
|BGChange            |✓|✓|  |  |✓|  |  |✓|  |  |  |
|TextureRoom         |✓|✓|  |  |  |  |  |✓|  |  |✓|
|BallPool            |✓|✓|  |✓|  |  |  |✓|  |  |✓|
|StepOn              |✓|✓|  |✓|  |  |  |✓|  |  |  |

<a name="assets"></a>
# アセット

**ToF AR Samples AR** は、以下のアセットを提供します。

### TofArSamplesAr
サンプルシーンのスクリプトやリソースが、コンポーネントごとに格納されています。

### TofArSettings
各コンポーネントが使用する設定変更UIとして、プレハブやスクリプトが格納されています。

|File|Description|
|:--|:--|
|Settings.Prefab|設定操作用UI|
|XXXController.Prefab|各コンポーネントの設定変更を管理|

<a name="environment"></a>
# 開発環境

## ビルド用ライブラリ
ビルドには、ToF AR と AR Foundation が必要です。
ToF AR は[ToF AR サイト](https://tof-ar.com/)からダウンロードし、インポートして使用してください。
AR Foundationは[ToF AR user manual](https://tof-ar.com/files/2/tofar/manual_reference/ToF_AR_User_Manual_ja.html)の[Setting up AR Foundation](https://tof-ar.com/files/2/tofar/manual_reference/ToF_AR_User_Manual_ja.html#_setting_up_ar_foundation)を参照して、セットアップを行ってください。  
インポート前にプロジェクトを開くと、設定によってはセーフモードへの移行確認メッセージが表示されます。  
セーフモードに移行した場合、セーフモードメニューなどからセーフモードを終了してインポートを行ってください。

## ドキュメント

ToF ARの開発ドキュメントも、ToF AR サイトで公開しています。
* 概要や使い方についてのマニュアルは[ToF AR user manual](https://tof-ar.com/files/2/tofar/manual_reference/ToF_AR_User_Manual_ja.html)
* 各コンポーネントの詳細記事は[ToF AR reference articles](https://tof-ar.com/files/2/tofar/manual_reference/ToF_AR_Reference_Articles_ja.html)
* APIリファレンスは[ToF AR API references](https://tof-ar.com/files/2/tofar/manual_reference/reference_ja/reference/api/TofAr.V0.html)

## 動作検証環境

動作検証は、下記の環境で行っています。

* Unity Version  : 2022.3.54f1
* ToF AR Version : 1.5.0
* AR Foundation  : 5.1.5


<a name="notes"></a>
# 注意事項

認識可能なハンドジェスチャーは国・地域によって異なる意味を有する場合があります。  
事前に確認されることをお勧めします。


<a name="contributing"></a>
# コントリビューション
**現在、プルリクエストは受け付けておりません。** バグ報告や新規機能のリクエストがありましたらissueとして登録して下さい。

このサンプルプログラムはToF ARを広く利用して頂けるようリリースしております。ご報告頂いたissueについては、検討の上、更新で対応する可能性があります。
