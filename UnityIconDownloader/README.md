# Icon Downloader
This package provides icon search and download functionalities to easily find and import icons into your Unity projects either for prototyping or actual development purposes.

## What does this tool provide?
* Searching and downloading icons in runtime (Play Mode).
* Searching and downloading icons in editor (Edit Mode) with ready-to-use editor windows.
* Selecting desired icon size(s) before downloading them.
* Automatically downloading license info for the icons.
* Generating combined license info for all downloaded icons, so it can be easily copy pasted into the credits screen.
* Image & RawImage integrations to easily search, download and assign icons.
* Filtering search result depending on premium type, color type, stroke type etc.
* Supporting multiple icon APIs. Currently it supports [IconFinder](https://www.iconfinder.com/) and [FlatIcon](https://www.flaticon.com/) APIs.

## What does this tool not provide?
* Searching for icon sets.
* Showing similar icons for the selected icons.
* Changing colors of the icons.

## Future roadmap
* Pagination support for the icon selection editor window.
* Choosing a background color for icon selection editor window.
* More API integrations.

## Importing the package to your game
In order to import the package to your game, you can employ one of these methods.

### Using Git URL (Recommended)
If you want the latest version of the package without needing to update it manually, you can open the Package Manager, choose “Add package from git URL…” and use `https://github.com/dogramacigokhan/GDPackages.git?path=UnityIconDownloader/Packages/IconDownloader` as the target.

### Using embedded package
You can download this repository, copy the `IconDownloader` package from `UnityIconDownloader/Packages` into your `[GameFolder]/Packages` folder.

### Using local package
You can download this repository, open the Package Manager and choose “Add package from disk…”, and select `[Repository]/Packages/IconDownloader/package.json` as the target.

## Dependencies

Regardless of the importing option you choose, don't forget to add these dependencies to your game.

### UniRx Dependency

There are a lot of `UnityWebRequests` usages in this package and they are done in the Edit mode. `UnityWebRequest` relies on coroutines but since the coroutines are a bit tricky to use in Edit mode and [they are a bit problematic](https://www.gokhandogramaci.com/2018/02/05/problems-with-unity3d-coroutines/) in general, UniRx – Reactive Extensions for Unity is used to make these requests.

To import UniRx into the project, you can open the Package Manager and choose “Add package from git URL…” and use `https://github.com/dogramacigokhan/UniRx.git?path=Assets/Plugins/UniRx/Scripts` as the target.

### Json.NET Dependency

Json string parsing is done using the Newtonsoft.Json package. If you are [using Unity 2018.4 or above you can add the package into manifest.json](https://forum.unity.com/threads/newtonsoft-json-package.843220/#post-5941664) if it's not there already. Otherwise, you need to [download the package](https://github.com/JamesNK/Newtonsoft.Json/releases) and import the library into your project.

## Installation

1. Go to `Tools/Icon Downloader/Settings` menu to create initial settings asset. Keep this file under the `Assets/Resources` folder.
2. From the settings, enable desired APIs by entering the API keys and set desired options.
3. Use `Tools/Icon Downloader/Generate License Info` menu to generate license information for all downloaded icons.
