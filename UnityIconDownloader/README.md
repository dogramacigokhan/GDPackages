# Icon Downloader
This package provides icon search and download functionalities to easily find and import icons into your Unity projects either for prototyping or actual development purposes.

![IconDownloader](https://user-images.githubusercontent.com/3823941/148414185-e50478f6-37bc-45c1-be32-3fb26778ad0d.gif)

It provides:
* Searching and downloading icons in runtime (Play Mode).
* Searching and downloading icons in editor (Edit Mode) with ready-to-use editor windows.
* Selecting desired icon size(s) before downloading them.
* Automatically downloading license info for the icons.
* Generating combined license info for all downloaded icons, so it can be easily copy pasted into the credits screen.
* Image & RawImage integrations to easily search, download and assign icons.
* Filtering search result depending on premium type, color type, stroke type etc.
* Supporting multiple icon APIs. Currently it supports [IconFinder](https://www.iconfinder.com/) and [FlatIcon](https://www.flaticon.com/) APIs.

It does **not** provide:
* Searching for icon sets.
* Showing similar icons for the selected icons.
* Changing colors of the icons.

## Table of Contents
1. [Installation](#installation)
   1. [Importing the package to your game](#importing-the-package-to-your-game)
      1. [Using Git URL (Recommended)](#using-git-url-recommended)
      2. [Using embedded package](#using-embedded-package)
      3. [Using local package](#using-local-package)
   2. [Adding Dependencies](#adding-dependencies)
      1. [UniRx Dependency](#unirx-dependency)
      2. [Json.NET Dependency](#jsonnet-dependency)
   3. [Setting up the tool](#setting-up-the-tool)
      1. [API Selection](#api-selection)
      2. [License Data](#license-data)
   4. [Generating License Info](#generating-license-info)
2. [Using the tool](#using-the-tool)
   1. [Searching and downloading icons in Edit Mode](#searching-and-downloading-icons-in-edit-mode)
   2. [Searching and downloading icons in Play Mode](#searching-and-downloading-icons-in-play-mode)
3. [Future roadmap](#future-roadmap)

## Installation

Follow these steps to install the package:

### Importing the package to your game
In order to import the package to your game, you can employ one of these methods.

#### Using Git URL (Recommended)
If you want the latest version of the package without needing to update it manually, you can open the Package Manager, choose “Add package from git URL…” and use this as the target:
```
https://github.com/dogramacigokhan/GDPackages.git?path=UnityIconDownloader/Packages/IconDownloader#iconDownloader_v2
```

#### Using embedded package
You can download this repository, copy the `IconDownloader` package from `UnityIconDownloader/Packages` into your `[GameFolder]/Packages` folder.

#### Using local package
You can download this repository, open the Package Manager and choose “Add package from disk…”, and select `[Repository]/Packages/IconDownloader/package.json` as the target.

### Adding Dependencies

Regardless of the importing option you choose, don't forget to add these dependencies to your game.

#### UniRx Dependency

There are a lot of `UnityWebRequests` usages in this package and they are done in the Edit mode. `UnityWebRequest` relies on coroutines but since the coroutines are a bit tricky to use in Edit mode and [they are a bit problematic](https://www.gokhandogramaci.com/2018/02/05/problems-with-unity3d-coroutines/) in general, UniRx – Reactive Extensions for Unity is used to make these requests.

To import UniRx into the project, you can open the Package Manager and choose “Add package from git URL…” and use this as the target:
```
https://github.com/dogramacigokhan/UniRx.git?path=Assets/Plugins/UniRx/Scripts
```

#### Json.NET Dependency

Json string parsing is done using the Newtonsoft.Json package. If you are [using Unity 2018.4 or above you can add the package into manifest.json](https://forum.unity.com/threads/newtonsoft-json-package.843220/#post-5941664), but it should be there already.

If you are using a lower Unity version, you need to [download the package](https://github.com/JamesNK/Newtonsoft.Json/releases) and import the library into your project.

### Setting up the tool

Go to `Tools/Icon Downloader/Settings` menu to create initial settings asset. Keep this file under the `Assets/Resources` folder. Some fields of the settings asset are explained below.

#### API Selection
Enable desired APIs and set API keys. You can use "Get API Key" button to open related API pages.

<img src="https://user-images.githubusercontent.com/3823941/148419467-544f3366-d082-42be-a968-7e155368db07.png" height="250"/>

#### License Data
Enable "Download License Data" option to download additional info alongside with the icons. These files will contain not only licenses information, but other useful information that you might need (e.g. icon source, icon id, etc.). To be able to generate license info for the downloaded icons, make sure to enable this option.

### Generating License Info
You can use `Tools/Icon Downloader/Generate License Info` menu to generate license information for all downloaded icons.

<img src="https://user-images.githubusercontent.com/3823941/148421355-2bb0c9cb-09a4-46e8-b820-57f4d214fc22.png" height="300" />

## Using the tool

### Searching and downloading icons in Edit Mode
`IconDownloadEditorFlow` (static) class can be used to initiate icon downloads in edit mode. Following methods are available with ready-to-use editor UIs:

| Method                         | Description                                                                                                                             |
|--------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| DownloadSingle                 | Searches and downloads the first found icon.                                                                                            |
| DownloadWithSelection          | Searches and lists found icons in an icon selection window.                                                                             |
| DownloadAsTextureWithSelection | Searches and lists found icons in an icon selection window. After the icon is downloaded, imports it as texture and returns the result. |
| DownloadAsSpriteWithSelection  | Searches and lists found icons in an icon selection window. After the icon is downloaded, imports it as sprite and returns the result.  |

### Searching and downloading icons in Play Mode
`IconDownloadFlow` class can be used to initiate icon downloads in play mode.
1. Create a class that implements `IIconDownloadFlowUI` and implement the interface methods.
2. Provide the instance of the class to `IconDownloadFlow` constructor.
3. Use following methods to initiate the download:

| Method                | Description                                                 |
|-----------------------|-------------------------------------------------------------|
| DownloadSingleIcon    | Searches and downloads the first found icon.                |
| DownloadWithSelection | Searches and lists found icons in an icon selection window. |

## Future roadmap
* Pagination support for the icon selection editor window.
* Choosing a background color for icon selection editor window.
* More API integrations.
