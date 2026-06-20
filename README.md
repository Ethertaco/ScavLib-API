**Current Version: 0.7.5 Stable**

[Source (DLL repository)](https://github.com/Kanisuko/ScavLib-API-DLL-Repository)

[Nexus Mods](https://www.nexusmods.com/scavprototype/mods/83)

---

## Building from source

ScavLib API is a BepInEx plugin targeting **.NET Framework 4.8**, built against the game's own assemblies. You don't need anything beyond Visual Studio and a local copy of the game.

### 1. Prerequisites

- **Visual Studio 2022** (Community is fine) with the *.NET desktop development* workload
- **.NET Framework 4.8 Developer Pack**
- A local install of **Casualties Unknown** with **BepInEx 5.x** already injected at least once (so the `BepInEx/` folder exists)

### 2. Clone the repo

```bash
git clone https://github.com/Ethertaco/ScavLib-API.git
cd ScavLib-API
```

### 3. Drop in the required DLLs

The `.csproj` references the following assemblies via flat `HintPath`, so they must sit **next to `ScavLib API.csproj`** (i.e. inside the `ScavLib API/` folder):

| DLL | Where to copy it from |
| --- | --- |
| `Assembly-CSharp.dll` | `<Game>/Casualties Unknown_Data/Managed/` |
| `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.IMGUIModule.dll`, `UnityEngine.InputLegacyModule.dll`, `UnityEngine.Physics2DModule.dll`, `UnityEngine.TextRenderingModule.dll`, `UnityEngine.UI.dll`, `UnityEngine.UIModule.dll` | `<Game>/Casualties Unknown_Data/Managed/` |
| `Unity.TextMeshPro.dll` | `<Game>/Casualties Unknown_Data/Managed/` |
| `0Harmony.dll`, `BepInEx.dll` | `<Game>/BepInEx/core/` |
| `Newtonsoft.Json.dll` | `<Game>/BepInEx/core/` (shipped with BepInEx) |
| `KrokoshaCasualtiesMP.dll` | `<Game>/BepInEx/plugins/` (only if you want KrokMP compat to compile; optional) |

> The pre-built versions of these DLLs are also mirrored in the [DLL repository](https://github.com/Kanisuko/ScavLib-API-DLL-Repository) for convenience.

### 4. Open and build

1. Open `ScavLib API.sln` in Visual Studio.
2. Pick configuration:
   - **Debug** — symbols included, easier to attach a debugger
   - **Release** — what you ship
3. Build (`Ctrl+Shift+B`). Output lands in `bin/Debug/` or `bin/Release/` as `ScavLib API.dll`.

### 5. Test in-game

1. Copy `ScavLib API.dll` into `<Game>/BepInEx/plugins/`.
2. Launch the game and check `<Game>/BepInEx/LogOutput.log` for the `[ScavLib]` startup line.
3. For iterative work, point your post-build event at the plugins folder so every rebuild deploys automatically, e.g.:

```
copy /Y "$(TargetPath)" "C:\Path\To\Casualties Unknown\BepInEx\plugins\"
```

### 6. Submitting changes

- Fork the repo, branch off `main`, push, open a PR against `Ethertaco/ScavLib-API:main`.
- Test on at least **EN** and one non-EN locale if your change touches `i18n/`.
- Keep DLLs out of commits — they're listed in `.gitignore` and should stay local.