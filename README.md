# Patches for 7 Days to Die
This repository contains my patches for [7 Days to Die](https://store.steampowered.com/app/251570/7_Days_to_Die/).
They are incompatible with Easy Anti-Cheat and require [BepInEx](https://github.com/BepInEx/BepInEx) to load.

# Available patches
- ForgeFuelSaver *(Stop fuel waste by automatically turning off forges)*
- FpsLimiter *(Make built-in FPS limiter settings persistent. Save additional power if the game is in not focused.)*
- MuteTraderAnnouncements *(Mute annoying speaker announcements)*

# How to install?
1. Download [BepInEx v5.4.21](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21) (latest version known to work with 7D2D)
2. Extract it into the game root folder (the one with both 7DaysToDie.exe and UnityPlayer.dll inside)
3. Generate BepInEx config by launching the game
4. Download and move selected patches (`.dll` files) into `BepInEx\plugins` folder
5. Patches should be applied on next game launch

Check [this guide](https://docs.bepinex.dev/v5.4.21/articles/user_guide/installation/index.html) if you are having trouble.

# How to compile? - Tips
- Configure PATH_7D2D_MANAGED env variable
- Setup dependencies (NuGet source - https://nuget.bepinex.dev/v3/index.json)
