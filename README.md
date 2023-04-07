## Contents
* [About ToF AR](#about)
* [Overview of ToF AR Samples AR](#overview)
* [Component](#component)
* [Assets](#assets)
* [Development environment](#environment)
* [Notes](#notes)
* [Contributing](#contributing)

<a name="about"></a>
# About ToF AR

ToF AR, Time of Flight Augmented Reality, is a toolkit library intended to aid in Unity application development for iOS and Android devices. It consists of a group of functionalities based on depth information from ToF / Lidar (light detection and ranging) sensor and so on.

As well as ToF AR, Unity and a compatible device with a ToF sensor are required to build and execute this sample application.

Please see [the ToF AR Site on Sony Developer World](https://developer.sony.com/develop/tof-ar) for ToF AR downloads and development guides, sample applications, and a list of compatible devices.


<a name="overview"></a>
# Overview of ToF AR Samples AR

**ToF AR Samples AR** is a sample application that uses the functions of ToF AR (see below).

When you start the application, the sample list is displayed on the screen. Select a sample from the list and tap it to start it.

<img src="/Images/topmenu.jpg" width="150">

Tap the screen with four fingers to return to the top screen.

## List of scenes

### SimpleARFoundation

A sample that uses both TofAR and UnityARFoundation features.

<img src="/Images/06_SimpleARFoundation.jpg" width="500">

This sample can be used to check the operation of the Hand, Mesh, and Modeling features.

### Puppet

This sample displays hand puppets at the position of each hand and forearm.

<img src="/Images/07_Puppet.jpg" width="150">

### Hand Decoration
Maps a pattern onto the back of your hand.

<img src="/Images/05_Handmark.jpg" width="500">

When you show the back of your hand to the camera a pattern will be mapped onto it.

### Rock Paper Scissors

A Rock Paper Scissors game using ToF AR gesture recognition.

<img src="/Images/01_RPS.jpg" width="500">

The game starts by recognizing a thumbs up, and an in-app voice will guide you through the game.

### Juggling
A Juggling game using face recognition and hand recognition.

<img src="/Images/02_juggling.jpg" width="500">

To start the game, follow the on-screen instructions to place your hands in the appropriate juggling position.

A ball will drop when the game starts.

Catch the ball and throw it up in the air towards your other hand by moving the hand holding the ball up.

When you have caught the balls 5 times, a new ball will be added.

### BGChange
Recognizes and changes the sky.

<img src="/Images/04_BGC.jpg" width="500">

The background changes when you do a thumbs up.

### TextureRoom
Creates a mesh of the environment with 3D Capturing, and maps text or photos onto the mesh.

<img src="/Images/03_TextureRoom.jpg" width="500">

Three modes are available. Tap the lower right hand icon and select the mode from "Mode DropDown".

* TextureAnimation mode:
 Maps text onto the mesh.

* TextInput mode:
 Enter the text to map from the input field at the bottom left.

* Stamp mode:
 Touch the "Add Stamp Button" and select photos from the camera roll. The photos will be mapped onto the mesh.

### BallPool

Generate AR balls in a recognized space.

<img src="/Images/09_BallPool.jpg" width="500">

The sample starts with space recognition to create a 3D mesh.

When the Ball Toggle set to On, balls will appear from above.

When objects like hands, feet, or a person, enters the scene it will move the balls.

### StepOn

Grass and flowers appear from the point of contact.

<img src="/Images/10_StepOn.jpg" width="500">

The sample starts with plane recognition of the space.

When hands or feet touch a wall or floor, animations of grass and flowers appearing is shown at the point of contact.

<a name="component"></a>
# Component

The table below shows the relationships between the scenes in the sample application and the ToF AR components used by each scene. The scene names are arranged vertically and the component names are arranged horizontally. A check mark indicates that the component is used.


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
# Assets

**ToF AR Samples AR** provides the following assets.

### TofArSamplesBasic
Sample scene scripts and resources are stored for each component.

### TofArSettings
Prefabs and scripts are stored as the Configuration change UI used by each component.


|File|Description|
|:--|:--|
|Settings.Prefab|Configuration change UI|
|XXXController.Prefab|Manage configuration changes for each component|


<a name="environment"></a>
# Development environment

## Build library
ToF AR is required in order to build.
Please download the Toolkit from [the ToF AR Site on Sony Developer World](https://developer.sony.com/develop/tof-ar) and import it.
Please see [Setting up AR Foundation](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html#_setting_up_ar_foundation) in the [ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html) for more information on how to set up AR Foundation.  
If the project is opened before importing, a confirmation message for entering safe mode will appear depending on the settings.  
If safe mode is entered, please import after exiting safe mode from the safe mode menu etc.

## Documents

ToF AR Development documents are also available on Developer World.

* [ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html) for overview and usage
* [ToF AR reference articles](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_Reference_Articles_en.html) for articles about each component
* [ToF AR API references](https://developer.sony.com/develop/tof-ar/development-guides/reference-api/reference/api/TofAr.V0.html)

## Verification environment

Operation was verified in the following environment:

* Unity Version  : 2021.3.18f1
* ToF AR Version : 1.3.0
* AR Foundation  : 4.2.7


<a name="notes"></a>
# Notes

Be aware that recognizable hand gestures may have different meaning in countries/areas.  
Prior cultural checks are advisable.


<a name="contributing"></a>
# Contributing
**We cannot accept any Pull Requests (PR) at this time.** However, you are always welcome to report bugs and request new features by creating issues.

We have released this program as a sample app with a goal of making ToF AR widely available. So please feel free to create issues for reporting bugs and requesting features, and we may update this program or add new features after getting feedback.
