<p align="center">
   <a href="https://github.com/Aragas/Bannerlord.MBOptionScreen" alt="Lines Of Code">
   <img src="https://tokei.rs/b1/github/Aragas/Bannerlord.MBOptionScreen?category=code" /></a>
   <a href="https://www.codefactor.io/repository/github/aragas/bannerlord.mboptionscreen"><img src="https://www.codefactor.io/repository/github/aragas/bannerlord.mboptionscreen/badge" alt="CodeFactor" /></a>
   </br>
   <a href="https://github.com/Aragas/Bannerlord.MBOptionScreen/actions?query=workflow%3A%22Test%20and%20Publish%22"><img src="https://github.com/Aragas/Bannerlord.MBOptionScreen/workflows/Test%20and%20Publish/badge.svg?branch=dev" alt="Test and Publish" /></a>
   <a href="https://codecov.io/gh/Aragas/Bannerlord.MBOptionScreen"><img src="https://codecov.io/gh/Aragas/Bannerlord.MBOptionScreen/branch/dev/graph/badge.svg" />
   </a>
   </br>
   <a href="https://www.nuget.org/packages/Bannerlord.MCM" alt="NuGet Bannerlord.MCM">
   <img src="https://img.shields.io/nuget/v/Bannerlord.MCM.svg?label=NuGet%20Bannerlord.MCM&colorB=blue" /></a>
   <a href="https://mcm.bannerlord.aragas.org/">
   <img src="https://img.shields.io/badge/Documentation-%F0%9F%94%8D-blue?style=flat" /></a>
   </br>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/612" alt="NexusMods Mod Configuration Menu">
   <img src="https://img.shields.io/badge/NexusMods-Mod%20Configuration%20Menu-yellow.svg" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/612" alt="NexusMods Mod Configuration Menu">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-version-pzk4e0ejol6j.runkit.sh%3FgameId%3Dmountandblade2bannerlord%26modId%3D612" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/612" alt="NexusMods Mod Configuration Menu">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dunique%26gameId%3D3174%26modId%3D612" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/612" alt="NexusMods Mod Configuration Menu">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dtotal%26gameId%3D3174%26modId%3D612" /></a>
   <a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/612" alt="NexusMods Mod Configuration Menu">
   <img src="https://img.shields.io/endpoint?url=https%3A%2F%2Fnexusmods-downloads-ayuqql60xfxb.runkit.sh%2F%3Ftype%3Dviews%26gameId%3D3174%26modId%3D612" /></a>
   </br>
   <img src="https://staticdelivery.nexusmods.com/mods/3174/images/headers/612_1592411190.jpg" width="800">
   </br>
   <img src="https://github.com/Aragas/Bannerlord.MBOptionScreen/blob/dev/resources/main.png?raw=true" width="800">
</p>



AKA MBOptionScreen Standalone.  
Previously, a fork of [ModLib](https://github.com/mipen/ModLib) that was de-forked.  
  
MCM is a Mod Options screen library designed to let modders use its API for defining the options.  
It can also display settings from other API's like ModLib, pre 1.3 and post 1.3, MBOv1/MCMv2, by using the compatibility layer modules.  
  
MCM supports two setting types - Global and PerSave. Global are shared across characters and saves, PerSave are stored within the save file!

It provides 5 types of options:
* Bool
* Int Slider / Textbox
* Float Slider / Textbox 
* Textbox
* Dropdown  
  
The settings can be defined at compile time by using the Attribute API and at runtime by using the Fluent Builder.  
