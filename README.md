<p align="center">
    <img src="https://avatars2.githubusercontent.com/u/39589027?s=256">
</p>

# BepInEx 6.0.0-pre.1 Legacy

![Github All Releases](https://img.shields.io/github/downloads/bepinex/bepinex/total.svg)
![GitHub release](https://img.shields.io/github/release/bepinex/bepinex.svg)
[![BepInEx Discord](https://user-images.githubusercontent.com/7288322/34429117-c74dbd12-ecb8-11e7-896d-46369cd0de5b.png)](https://discord.gg/MpFEDAg)

Bepis Injector Extensible

---

BepInEx is a plugin / modding framework for Unity Mono, IL2CPP and .NET framework games (XNA, FNA, MonoGame, etc.)


#### Platform compatibility chart

|              | Windows | OSX  | Linux | ARM |
|--------------|---------|------|-------|-----|
| Unity IL2CPP | ✔️       | ❌    | ❌ (Wine only)  | ❌   |

This is a fork of [BepInEx 6.0.0-pre.1](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1) release to work with Star Trek Fleet Command™ (STFC) client.

The latest version of BepInEx (builds after 6.0.0-be.647) using Il2CppInterop library has some [issues](https://github.com/BepInEx/Il2CppInterop/issues/106) with Access Violation causing silent crashes. These errors occur when adding plugins or when multiple methods injections are used in one plugin.
BepInEx 6.0.0-pre.1 worked fine but due to game engine update to Unity 2021.3.41f it can't parse global-metadata file of version 31. So, it was an update integration of newer Il2CppDumper to work with metadata v31.

Tested only with STFC client - Unity v2021.3.41f Il2Cpp

use [Legacy](https://github.com/Plurimus/BepInEx.Legacy/tree/Legacy) branch 

## Resources

**[BepInEx](https://github.com/BepInEx/BepInEx)**
- [BepInEx 6.0.0-pre.1](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1) - v6.0.0-pre.1
  fork ([ec79ad0](https://github.com/BepInEx/BepInEx/commit/ec79ad057b20c302c17b34e63906ee398352d852))
  
**[Il2CppDumper](https://github.com/Perfare/Il2CppDumper)**
- [Perfare/Il2CppDumper](https://github.com/Perfare/Il2CppDumper/releases/tag/v6.7.46) - v6.7.46
  fork ([8a521b9](https://github.com/Perfare/Il2CppDumper/commit/8a521b9c180cf13499253f0818cbc729dca767cb))

## STFC Plugins/Mods examples
- [ConfigManager](https://github.com/Plurimus/Optimus.STFC.ConfigManager) - ingame settings configurator for [STFC Community Patch Mod](https://github.com/netniV/bob)
- [UniversalTranslator](https://github.com/Plurimus/Optimus.STFC.UniversalTranslator) - alliance/galaxy chat translator
- [Transformers](https://github.com/Plurimus/Optimus.STFC.Transformers) - some client GUI tweaks

## Support
For STFC BepInEx items, please visit the [BORG Box](https://discord.gg/8MRcfserGH) discord server.
