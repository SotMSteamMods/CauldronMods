# Contributing

We welcome contributions, including from newcomers with no prior modding experience.

Don't know C#, that's ok, lots of Art resources need to be created.
Don't know art, that's ok, lots of DeckLists that need proofreading.
Don't like proofreading, that's ok, just play the Mod and report issues.

First step to helping out is to join the [Discord](https://discord.gg/t6xupMv767).

## Table of contents

* [Resources and links](#resources-and-links)
* [How to get started](#how-to-get-started)
* [How to contribute code](#how-to-contribute-code)
* [How to contribute art](#how-to-contribute-art)
* [How to contribute music](#how-to-contribute-music)

## Resources and links

* Discusion: [Discord Invite](https://discord.gg/t6xupMv767)
* Cauldron website: https://cauldron4.webnode.com/
* Handlebra Mod Trello board: https://trello.com/b/vYBMImbg/sotm-workshop
* Project Trello board: https://trello.com/b/Doyff7gB/cauldron-sotm-mod
* BGG Forum: https://boardgamegeek.com/thread/2532898/mod-sentinels-digital 
* SotM Full Card Breakdown Sheet - [Google Sheets](https://docs.google.com/spreadsheets/d/1F-4drbJyXUWxFg_EzzBsrprzPciAtRdZZLyUStl3abI/edit?usp=sharing)

## How to get started

1. Install [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
   * Community Edition is free and perfect for modders.
   * When installing choose the C# and Game Development options.
   * You'll also want the select the .NET Core SDK's.
1. Install a GitHub client
   * Microsoft has a integrated [GitHub Extension](https://visualstudio.github.com/) available.
   * Directions assume this is the route you choose.
1. Install Sentinels of the Multiverse Digital in Steam.
1. Connect to GitHub and get the project
   * In Team Explorer, connect to your GitHub account.
   * Sync from https://github.com/SotMSteamMods/CauldronMods.git.
1. Build the Solution, and Run the Tests

You will also want one or more .Net Decompilers
* [dnSpy](https://github.com/dnSpy/dnSpy/releases) is easy to use and great for just looking.
* [ILSpy](https://github.com/icsharpcode/ILSpy/releases) generates better code if you plan on exporting code.

## How to contribute code

* [Coding Notes](./CODE_WISDOM.md)

1. Create a local github Branch for your develeopment.
1. Implement!
   1. Follow the existing structure for folders, files, and code
   1. Create or modify tests first
   1. Create or modify the cards
   1. Test
   1. Review any related tests and expand if you can.
1. Let the Discord know that you have code you are ready to share. You'll be given write access to push your branch.
1. Push your branch, and open a Pull Request for review.
1. Success!

