# CustomGameSpeed

**A simple configurable mod for overriding time scale values in the game UI.**

## Description

Replaces the original game speed buttons with the following:

* Pause (x0)
* Normal (x1)
* Custom speed 1 (default: x3)
* Custom speed 2 (default: x5)
* Custom speed 3 (default: x10)

This works with **UI buttons, hotkeys, and other in-game systems**.

## Configuration

After first launch, the config file will be generated at:

```
BepInEx/config/com.madmer.CustomGameSpeed.cfg
```

You can adjust the custom speeds using fractional values (use a dot `.` as the decimal separator):

```ini
[General]
## Custom time scale for slot 3 (default: 3x)
Speed 3 = 3

## Custom time scale for slot 4 (default: 5x)
Speed 4 = 5

## Custom time scale for slot 5 (default: 10x)
Speed 5 = 10
```

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) for your game
2. Place the compiled `CustomGameSpeed.dll` into:

   ```
   BepInEx/plugins/CustomGameSpeed/
   ```
3. Launch the game once to generate config
4. (Optional) Edit `com.madmer.CustomGameSpeed.cfg` to your desired speeds

---

Made with ❤ by madmer

Feel free to tweak or fork. Feedback is welcome!
