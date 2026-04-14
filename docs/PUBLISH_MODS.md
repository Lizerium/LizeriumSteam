# 💣 Mod Publishing

- [Back to main](../README.md)

## Server Mod Folder Structure

For the `LizeriumFreelancerMode` mod on `LizeriumServer`, the following structure is used:

```text
Mods/
└── LizeriumFreelancerMode/
    ├── 99.3.4/
    ├── updates/
    └── version.xml
```

---

## Generating Mod Release Files

When publishing a mod, you need to:

- generate `manifest.xml`
- prepare installation files for deployment

These files will be placed in:

```text
Mods/LizeriumFreelancerMode/99.3.4
```

To generate them, use the Publisher from:

[AppUpdater.Publisher](../AppUpdater.Publisher/bin/Release)

### Generation command:

```sh
.\AppUpdater.Publisher.exe -source:.\INPUT\3_FL_APP\ -target:.\MODS\ -version:99.3.4 -deltas:2
```

> [!IMPORTANT]
> Additionally, game files must be split into smaller `.bin` parts.
> This is typically done using an **Inno Setup** script (you can use your own version), for example:
>
> ```text
> 99.3.4.iss
> ```

---

## Uploading Mod Files to Server

After generating the release files:

- take all generated binaries
- include the manifest
- upload everything into the server directory

![VIEW_MOD](MEDIA/1.png)

---

## Mod Version File

In the file:

```text
Mods/LizeriumFreelancerMode/version.xml
```

you must specify:

- the current game version
- the latest update archive version available in `updates`

### Example:

```xml
<config>
    <version>99.3.4</version>
    <updates>99.3.12</updates>
</config>
```

---

## Update Archives

Upload update archives into:

```text
Mods/LizeriumFreelancerMode/updates
```

> [!TIP]
> Archives are created as follows:
>
> 1. First, create a `.7z` archive using **ultra compression (LZMA2)**
> 2. Then wrap the `.7z` into a `.tar` archive

![Updates](MEDIA/4.png)
