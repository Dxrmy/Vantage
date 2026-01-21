<div align="center">
  <img src="icon.png" alt="Vantage Icon" width="128" />
  <h1>Vantage</h1>
  <p>
    <strong>The Ultimate Valorant Swiss-Army Tool & Extensions Host</strong>
  </p>
  
  ![C#](https://img.shields.io/badge/C%23-12-239120?style=flat&logo=c-sharp&logoColor=white)
  ![Valorant](https://img.shields.io/badge/Game-Valorant-FF4655?style=flat&logo=valorant)
  ![Status](https://img.shields.io/badge/Status-In--Development-yellow)
  ![License](https://img.shields.io/badge/License-MIT-green)

  <br />
</div>

**Vantage** is a high-performance, modular host designed specifically for Valorant players. It serves as a unified platform for powerful extensions that interact directly with the game's live API and local state.

## Features
- **Extension Ecosystem:** Build and swap specialized tools without touching the core host logic.
- **Discord Rich Presence:** Live integration showing map, agent, and real-time match stats to your community via the official Valorant API.
- **Match Overlay:** High-speed HUD with "Rank Yoinker" capabilities to see your opponents' ranks instantly.
- **Unified Session Auth:** Standalone management of Riot API sessions for all connected plugins.
- **Plugin Management:** Core host handles dynamic assembly loading and configuration persistence.

## 🚧 Roadmap & Todo
State of the project as of latest push:

- [x] **Core Host**: Plugin manager, dynamic assembly loading, and service injection.
- [x] **SDK Layer**: Standardized interfaces for `IVantagePlugin` and `IHost`.
- [x] **Valorant Integration**: Session management and base API connector.
- [/] **Extensions**:
    - [x] Discord RPC (Live Match Data).
    - [/] Match Overlay (WIP rendering engine).
    - [ ] Rank Yoinker (API integration refinement).
- [ ] **UI/UX**: WinUI 3 modernization for the main host dashboard.

## 🛠 Tech Stack
- **Framework**: .NET 8.0 / C# 12
- **Architecture**: Service-Oriented Plugin Architecture
- **APIs**: Unofficial Valorant Client API (Local & Remote)

## 📊 Analytics
<div align="center">
  <a href="https://github.com/Dxrmy/Vantage">
  <img height="130" align="center" src="https://github-readme-stats.vercel.app/api/pin/?username=Dxrmy&repo=Vantage&theme=transparent&border_color=30363d&show_owner=true"/>
  </a>
</div>

## ⚠️ Disclaimer
This project uses unofficial APIs and memory-reading techniques. Use at your own risk. The developer is not responsible for any game bans or account losses.

## 📄 License
Distributed under the MIT License. See `LICENSE` for more information.
