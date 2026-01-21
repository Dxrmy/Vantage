# Vantage
**Modular Event-Driven Host for Valorant Tools**

This solution replaces the monolithic `ValorantRPC_WinUI`.

## Structure
*   **Vantage.SDK**: The contract (`IVantagePlugin`) that all projects reference.
*   **Vantage.Core**: The main desktop application (Host). It loads plugins from the `/Plugins` directory.
*   **Vantage.Ext.Discord**: Plugin for Discord Rich Presence.
*   **Vantage.Ext.Overlay**: Plugin for the In-Game Overlay.

## Building
Run `dotnet build Vantage.slnx` or build projects individually.
*Warning: This solution uses a `.slnx` (XML) file.*

## Architecture
Vantage uses a "Host/Plugin" architecture.
1.  Core starts up and initializes `PluginManager`.
2.  `PluginManager` scans for `.dll` files implementing `IVantagePlugin`.
3.  Core calls `Initialize(host)` on each plugin.
4.  Plugins inject UI into the Core's Navigation via `GetSettingsView()`.
