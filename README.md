## Contents
* [About ToF AR](#about)
* [Overview of ToF AR Samples AR](#overview)
* [Component](#component)
* [Assets](#assets)
* [Development environment](#environment)
* [Contributing](#contributing)

<a name="about"></a>
# About ToF AR

ToF AR, Time of Flight Augmented Reality, is a toolkit library intended to aid in Unity application development for iOS and Android devices. It consists of a group of functionalities based on depth information from ToF / Lidar (light detection and ranging) sensor and so on.

As well as ToF AR, Unity and a compatible device with a ToF sensor are required to build and execute this sample application.

Please see [the ToF AR Site on Sony Developer World](https://developer.sony.com/develop/tof-ar) for ToF AR downloads and development guides, sample applications, and a list of compatible devices.


<a name="overview"></a>
# Overview of ToF AR Samples AR

**ToF AR Samples AR** is a sample application that uses the functions of ToF AR and provides 5 scenes (see below).

When you start the application, the scene list is displayed on the screen. Select a scene from the list and tap it to start it.

<img src="/Images/topmenu.jpg" width="150">

Tap the screen with four fingers to return to the top screen.

## List of scenes

### Rock Paper Scissors

A Rock Paper Scissors game using ToF AR gesture recognition.

<img src="/Images/01_RPS.jpg" width="500">

The game starts by recognizing thumbs up, and an in-app guidance voice will be played.

### Juggling
A Juggling game using face recognition and hand recognition.

<img src="/Images/02_juggling.jpg" width="500">

Place your hands in the appropriate juggling position, according to the on-screen instructions, to start the Juggling game.

A ball will drop when the game starts. Catch this ball.

Throw the ball towards your other hand by moving the hand holding the ball up.

Every time you catch a ball 5 times, a new ball will be added.

### TextureRoom
Creates a mesh of the environment with 3D Capturing, and maps text or photos onto the mesh.


<img src="/Images/03_TextureRoom.jpg" width="500">

Three modes are available. Please touch the lower right hand icon and select the mode from "Mode DropDown".

* TextureAnimation mode:
 Maps text onto the mesh.

* TextInput mode:
 Enter the text to map from the input field at the bottom left.

* Stamp mode:
 Touch the "Add Stamp Button" and select photos from the camera roll. The photos will be mapped onto the mesh.

### BGChange
Recognizes and changes the sky.

<img src="/Images/04_BGC.jpg" width="500">

The background changes when you do thumbs up.

### Hand Decoration
Maps a pattern onto the back of your hand.

<img src="/Images/05_Handmark.jpg" width="500">


Please point the back of your hand at the camera. A pattern will be mapped onto the back of your hand.


<a name="component"></a>
# Component

The table below shows the relationships between the 5 scenes in the sample application and the ToF AR components used by each scene. The scene names are arranged vertically and the component names are arranged horizontally. A check mark indicates that the component is used.


||ToF|Color|Mesh|Coordinate|Hand|MarkRecog|Body|Segmentation|Face|Plane|Modeling|
|:--|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|Rock Paper Scissors |✓|✓|  |  |✓|  |  |  |  |  |  |
|Juggling            |✓|✓|  |　|✓|  |  |  |✓|  |  |
|TextureRoom         |✓|✓|  |  |  |  |  |✓|  |  |✓|
|BGChange            |✓|✓|  |  |✓|  |  |✓|  |  |  |
|Hand Decoration     |✓|✓|  |  |✓|  |  |  |  |  |  |


<a name="assets"></a>
# Assets

**ToF AR Samples AR** provides the following assets.

### TofArSamplesBasic
5 sample scene scripts and resources are stored for each component.

### TofArSettings
Prefabs and scripts are stored as the configuration change UI used by each component.


|File|Description|
|:--|:--|
|Settings.Prefab|Configuration change UI|
|XXXController.Prefab|Manage configuration changes for each component|


<a name="environment"></a>
# Development environment

## Build library
ToF AR is required for build.
Please download the Toolkit from [the ToF AR Site on Sony Developer World](https://developer.sony.com/develop/tof-ar), and then import and use it.
Please see [Setting up AR Foundation](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html#_setting_up_ar_foundation) in the [ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html) for more information on how to set up AR Foundation.

## Documents

ToF AR Development documents are also available on Developer World.

* [ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html) for overview and usage
* [ToF AR reference articles](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_Reference_Articles_en.html) for articles about each component
* [ToF AR API references](https://developer.sony.com/develop/tof-ar/development-guides/reference-api/reference/api/TofAr.V0.html)

## Verification environment

Operation was verified in the following environment:

* Unity Version  : 2020.3.28f1
* ToF AR Version : 1.0.0
* AR Foundation  : 4.2.2


<a name="contributing"></a>
# Contributing
**We cannot accept any Pull Request (PR) at this time.** However, you are always welcome to report bugs and request new features by creating issues.

We have released this program as a sample app with a goal of making ToF AR widely available. So please feel free to create issues for reporting bugs and requesting features, and we may update this program or add new features after getting feedback.
