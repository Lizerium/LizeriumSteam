<h1 align="center">💽 Lizerium Launcher 💽</h1>

<p align="center">
  <img src="https://shields.dvurechensky.pro/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://shields.dvurechensky.pro/badge/.NET-WPF-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://shields.dvurechensky.pro/badge/Architecture-Prism.Unity-6A1B9A?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Updater-Delta%20Patch-2E7D32?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Distribution-Auto%20Update-1565C0?style=for-the-badge" />
  <img src="https://shields.dvurechensky.pro/badge/Status-Release-00C853?style=for-the-badge" />
</p>

<p align="center">
  <img src="https://shields.dvurechensky.pro/badge/Product-Lizerium-E53935?style=flat-square" />
  <img src="https://shields.dvurechensky.pro/badge/Game-Freelancer-FF8F00?style=flat-square" />
  <img src="https://shields.dvurechensky.pro/badge/Server-LizeriumServer-3949AB?style=flat-square" />
  <img src="https://shields.dvurechensky.pro/badge/Publisher-AppUpdater.Publisher-00897B?style=flat-square" />
  <img src="https://shields.dvurechensky.pro/badge/Patching-Versioned%20Builds-5E35B1?style=flat-square" />
  <img src="https://shields.dvurechensky.pro/badge/Deploy-Manual%20Pipeline-6D4C41?style=flat-square" />
</p>

<p align="center">
  <b>Lizerium Launcher</b> — a launcher and incremental update system for client applications and game modifications
  within the <b>Lizerium</b> ecosystem, designed for minimal bandwidth usage, version control, and server-side publishing.
</p>

<div align="center" style="margin: 20px 0; padding: 10px; background: #1c1917; border-radius: 10px;">
  <strong>🌐 Language: </strong>
  
  <a href="./README.ru.md" style="color: #F5F752; margin: 0 10px;">
    🇷🇺 Russian
  </a>
  | 
  <span style="color: #0891b2; margin: 0 10px;">
    ✅ 🇺🇸 English (current)
  </span>
</div>

---

> [!NOTE]
> This project is part of the **Lizerium** ecosystem and belongs to the following direction:
>
> - [`Lizerium.Software.Structs`](https://github.com/Lizerium/Lizerium.Software.Structs)
>
> If you are looking for related engineering and supporting tools, start there.

# Table of Contents

- [Table of Contents](#table-of-contents)
  - [📦 About the Project](#-about-the-project)
  - [Overview](#overview)
  - [Technologies](#technologies)
  - [Features](#features)
  - [Documentation](#documentation)
  - [Related Projects](#related-projects)
  - [Other](#other)

---

## 📦 About the Project

**Lizerium Launcher** is a launcher and update system designed for the Lizerium ecosystem.

The main goal is to provide **incremental (delta-based) updates** for applications and game modifications with minimal bandwidth usage.

The project follows an approach similar to modern browser updates (e.g., Chrome):

- only differences between versions are downloaded
- multiple previous versions are supported
- updates are transparent to the user

---

## Overview

![PRISM version](<docs/HISTORY/Переход на PRISM.png>)

> [!IMPORTANT]
> The goal of this component is to provide continuous and incremental updates without user interaction.  
> The behavior is similar to Google Chrome’s update mechanism — each update downloads only the minimal required data.

> [!NOTE]
> The project uses a local configuration that is applied during the build process.

---

## Technologies

- WPF
- Prism.Unity
- .NET

---

## Features

- Incremental launcher updates
- Delta patching between versions
- Game modification publishing
- Minimal update bandwidth usage
- Integration with server-side infrastructure
- Versioned build support

---

## Documentation

- [Build Guide](docs/BUILD.md)
- [Launcher Publishing](docs/PUBLISH_LAUNCHER.md)
- [Mods Publishing](docs/PUBLISH_MODS.md)
- [Update Format](docs/UPDATE_FORMAT.md)
- [Project Architecture](docs/Architecture.md)
- [Visual History](docs/HISTORY)
- [Competitors | Observations](docs/LAUNCHER_CONCURENTS)

---

## Related Projects

- https://github.com/Lizerium/LizeriumServer
- https://github.com/Lizerium/LizeriumFindChanges

---

## Other

> [!TIP]
> The latest installer version of `LizeriumLauncher` is typically located in the `Launcher` directory  
> at the root of the server (`LizeriumServer`).
