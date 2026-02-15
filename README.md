Armok Vision
============

[Forum thread](http://www.bay12forums.com/smf/index.php?topic=146473) | [Screenshots](http://imgur.com/a/bPmeo)

A 3d realtime visualizer for Dwarf Fortress.

**Compatibility:** Dwarf Fortress **50.04+** (Steam/Premium and current Classic, e.g. **53.10**). Requires **DFHack** built for the same DF version. The name "armok-vision-steam" refers to the Steam release era of Dwarf Fortress (Dec 2022), not the Steamworks SDK.

## To Use

1. Install Dwarf Fortress and a current DFHack (same version as your DF, e.g. [DFHack for 53.10](https://docs.dfhack.org/)).
2. Download Armok Vision from the forum thread.
3. Load into a fortress mode map.
4. Start up Armok Vision.
5. Enjoy!

## To Contribute

Want to help out? We love contributions!

#### Bugs
If you've run into any bugs: [report them!](https://github.com/JapaMala/armok-vision/issues) (You'll need a [github account](https://github.com/).) Make sure to describe the issue in as much detail as you can, and to mention what system & Armok Vision version you're using. Also, check if what you're reporting has been reported before.

#### Artists
If you're an artist and want to contribute 3D models, sounds, concept art:

- There should be a folder called `StreamingAssets` somewhere around the armok vision executable (or inside, if you're on a mac.) If you edit the files inside and restart Armok Vision, it will use your modified assets. Be careful editing the .xml files, they're finnicky. You can post your edited resources in the forum thread and we can try to integrate them with the project.
- Alternatively, load things up in Unity and edit them there (see the following instructions).
- Check the [issues](https://github.com/JapaMala/armok-vision/issues); there may be something open about things that need prettifying.

#### Developers
If you know how to code and want to hack on the engine:

1. Install [Unity 2022](https://unity3d.com/get-unity) (project uses 2022.1.x). Personal Edition and Unity Hub are fine.
2. Non-Windows users: install [Git LFS](https://git-lfs.github.com/) (test with `git lfs version`).
3. Clone the repo with submodules: `git clone --recurse-submodules <repo-url>` (e.g. armok-vision-steam).
4. Load the project folder in the Unity editor.
5. **Regenerating protos (if you have DFHack):** Create `ProtoPath.txt` in the project root with the path to your DFHack plugin protos folder, e.g. `.../dfhack/plugins/remotefortressreader/proto`. Then run **Mytools → Build Proto** to copy and regenerate `Assets/RemoteClientLocal/protos.cs`.
6. Run **Mytools → Build Material Collection**. Required after a fresh pull and after changing material files.
7. Hack around. Check out the [issues](https://github.com/JapaMala/armok-vision/issues) to find things that need fixing / ideas that could be implemented.
8. Submit a [pull request](https://github.com/JapaMala/armok-vision/pulls) with your changes!

#### Financially
If you want to buy the lead programmer a snack, you can donate on his [Patreon Page](https://www.patreon.com/japamala)

##### Structural Notes
(Some short notes for anyone getting started with the codebase.)

- Armok Vision is built with the [Unity engine](https://unity3d.com/). It connects to the [remotefortressreader](https://github.com/DFHack/dfhack/tree/develop/plugins/remotefortressreader) DFHack plugin over TCP and exchanges [protobuf messages](https://github.com/DFHack/dfhack/tree/develop/plugins/remotefortressreader/proto) (RemoteFortressReader.proto and related).
- On the Unity side, the submodule [Assets/RemoteClientDF-Net](https://github.com/JapaMala/armok-vision/tree/master/Assets) contains the generated C# protobuf files, as well as classes for managing the network connection. The script [Assets/Scripts/MapGen/DFConnection.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/Scripts/MapGen/DFConnection.cs) runs the connection on a separate thread and exposes data collected from DF.
- The script that actually manages the onscreen map is [Assets/Scripts/MapGen/GameMap.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/Scripts/MapGen/GameMap.cs), which stores the `GameObject`s representing different map chunks. It calls the scripts in [Assets/Scripts/MapGen/Meshing](https://github.com/JapaMala/armok-vision/tree/master/Assets/Scripts/MapGen/Meshing) to build the actual meshes (on separate threads).
- Most assets - textures, 3d models, sprites, etc. - are loaded at runtime from [Assets/StreamingAssets](https://github.com/JapaMala/armok-vision/tree/master/Assets/StreamingAssets), which is copied directly to folder containing the generated app. The script that handles this is [Assets/Scripts/MapGen/ContentLoader.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/Scripts/MapGen/ContentLoader.cs).

There's a lot of other stuff but hopefully it'll be reasonably self-explanatory. Alternatively, you can ask in the forum thread, or the #dfhack IRC channel on freenode; somebody might be lurking who can help.
