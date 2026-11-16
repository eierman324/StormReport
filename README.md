> [!WARNING]
> Everything this fork adds or changes (code, README, everything) was written by Claude (Anthropic's AI). No line of it was written by a human. It builds on [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) by LED-N and the projects listed under Credits. Use at your own risk.

<div align="center">

**English** · [Français](README.fr.md) · [Español](README.es.md)

<img src="docs/icon.png" alt="" width="88">

# Storm Report

[![version](https://img.shields.io/github/v/release/eierman324/ChestAddictionCounter?style=flat-square&label=version&color=7d55ff)](../../releases/latest) [![downloads](https://img.shields.io/github/downloads/eierman324/ChestAddictionCounter/total?style=flat-square&label=downloads&color=7d55ff)](../../releases) ![Windows 10 | 11](https://img.shields.io/badge/Windows-10%20%7C%2011-3a7bd5?style=flat-square) [![license](https://img.shields.io/github/license/eierman324/ChestAddictionCounter?style=flat-square&label=license&color=555)](LICENSE)

**Your Fortnite report: wins, eliminations, damage, chests and treasures, your locker, with an overlay for the game and for streaming.**

[**⬇ Download StormReport.exe**](../../releases/latest) · Windows 10 / 11 · free · no install

</div>

---

> Storm Report started as a fork of [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) by LED-N. It keeps its chest counting and treasure hunt data and grows it into a general Fortnite report. Coming from Fortnite Chest Counter or Chest Addiction Counter? Your history and settings are picked up as they are.

## What it shows

Storm Report combines two sources, and never shows the same figure from both:

| From your **Epic account** (sign-in required) | From your **replays** (the matches recorded on your PC) |
|---|---|
| Matches, wins, win rate, eliminations, K/D and time played, in total and per mode, for every match on every device | Every single match: placement, eliminations and assists, damage dealt and taken, accuracy, materials, distance |
| Your whole locker (search, categories, favourites), V-Bucks, purchases, past seasons | Average placement, damage per match, the current session, live figures during a match |
| The treasures you own (the collection fills in by itself) | Every chest opened or destroyed, chests per day and per hour, treasures found and where they came from |

- **Report**: overview, a table per mode with both sources side by side, the latest matches, then chests and treasures (your chances of the gold outfit, the collection, the treasures other players got near you).
- **Account / Locker**: your Epic account, read only, and a **Transfer my whole locker to fortnite.gg** button.
- **Overlay**: in game (over Fortnite in windowed fullscreen), for OBS (a local page) and a **stream window** for apps without browser sources such as TikTok LIVE Studio (window capture, dark or chroma-key green). The big figure can be chests, eliminations or wins.

## Getting started

1. Download **[`StormReport.exe`](../../releases/latest)** and run it. Nothing to install.
   Windows may show *"Windows protected your PC"* (the exe is not signed): *More info → Run anyway*. The release also carries `StormReport.exe.sha256` to check the download (`Get-FileHash .\StormReport.exe`).
2. Turn on replays in Fortnite: *Settings → Game → Replays → Record Replays: **On***.
3. Sign in with Epic on the **Account** tab: the browser opens the Epic login; when a page shows a text with `authorizationCode`, press Ctrl+A, Ctrl+C (picked up by itself) or paste it into the window.

## Privacy and safety

- **Epic account, read only**: Storm Report uses the same Epic endpoints and Fortnite game clients as community tools (for example the fortnite.gg locker import). Nothing is ever changed on your account. These endpoints are not an official public API: Epic may change or block them.
- **Stay signed in** keeps an Epic device login encrypted with Windows DPAPI (readable by your Windows user only). **Sign out** deletes it on your PC and at Epic.
- **Replays are only read**: `%LOCALAPPDATA%\FortniteGame\Saved\Demos`, opened read-only. No game modification, no access to the game's memory.
- **Your data stays on your PC** (`%APPDATA%\ChestCounter`). The window is a page served to your PC only (`127.0.0.1`, random key).
- **Network**: Epic (your account), fortnite-api.com (item names and icons), fortnite.gg (only when you transfer your locker), GitHub (treasure hunt data of the original project and updates of Storm Report).

## How reliable are the figures?

- **From Epic**: the official totals of your account.
- **From the replays**: placement, eliminations, damage, accuracy and the other end-of-match stats are written by Fortnite itself into the replay. Chests are recognized from the events of the replay: chests a vehicle destroys are sometimes not recorded (such matches are flagged), and teammates far from you are missing from the squad figures. Where a treasure came from is estimated and flagged when unsure.
- The replay reader is not made by Epic: after a big Fortnite update it may need an update before replays can be read again.

## Credits

- [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) by LED-N: chest counting, treasure hunt data and images.
- Fixes and ideas from the forks of cevival (treasure attribution), koiseka (weekly odds), tbpg58 (release checksums, activity log), voiddog30 (chests and treasures per hour) and cherjp (replay key check).
- [Obsidian](https://docs.mspaint.cc/obsidian) UI by mspaint, [Tailwind CSS](https://tailwindcss.com) (MIT), [IBM Plex Mono](https://github.com/IBM/plex) (SIL OFL).
- [fortnite-api.com](https://fortnite-api.com) for item names and icons; the fortnite.gg locker link is built like [FNGG-LockerGenerator](https://github.com/Liqutch/FNGG-LockerGenerator) (MIT).

Storm Report is not affiliated with or endorsed by Epic Games. Fortnite is a trademark of Epic Games, Inc.

## Build it yourself

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and run `build.bat`: the exe lands in `dist\`.
