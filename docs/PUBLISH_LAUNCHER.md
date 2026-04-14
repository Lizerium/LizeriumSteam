# 🚀 Launcher Update Publishing

- [Back to main](../README.md)

## Publishing Component

Publishing is performed using:

- [AppUpdater.Publisher](../AppUpdater.Publisher/bin/Release)

![Publisher](MEDIA/2.png)

---

## Pre-Publish Preparation

Before generating a new version, make sure to:

1. Update `config.xml` in the launcher:
   - set the current application version in `<version>`
   - set the previous version in `<last_version>`
   - specify the update server address in `updateServer`, for example:

   ```xml
   <updateServer>https://lizup.ru/uploader/</updateServer>
   ```

2. Verify paths to Freelancer and the mod — they must be standard.

3. Place `config.xml` inside the update folder (e.g., `1.0.5`) **before building**.

4. Sign all generated libraries using **kSign** with the `dvurechensky.pfx` key.

5. Remove unnecessary files:
   - `.pdb` files
   - log files generated during build or testing

---

## Generating Update Files

Run the following command in **PowerShell**:

```bash
.\AppUpdater.Publisher.exe -source:{AppFolder} -target:{ReleaseFolder} -version:{X.X.X} -deltas:{X}
```

---

## Parameters

1. `-source` — path to the freshly built application
2. `-target` — publish directory (from which updates will be uploaded to the server)
3. `-version` — application version (e.g., `1.0.0`, `1.2.3`)
4. `-deltas` — number of previous versions to generate delta updates for

Recommended value: `2`

<details>
<summary>Examples</summary>

```sh
.\AppUpdater.Publisher.exe -source:..\..\..\TestApp\bin\Debug\ -target:.\PUBLISH\ -version:1.2.5 -deltas:2
```

> Actual command

```sh
.\AppUpdater.Publisher.exe -source:..\..\..\LizeriumLauncher\bin\Release\ -target:.\PUBLISH\ -version:1.0.1 -deltas:2
```

</details>

---

## Uploading to Server

After generation, upload the following to the server:

- the new [`version.xml`](../AppUpdater.Publisher/bin/Release/PUBLISH/version.xml) (contains latest version info)
- the version folder from [`PUBLISH`](../AppUpdater.Publisher/bin/Release/PUBLISH)

For example, for version `1.0.4`, upload to the `uploader` directory on the server (`LizeriumServer`):

- `version.xml`
- folder `1.0.4`

### Example structure:

```text
uploader/
├── version.xml
└── 1.0.4/
```

![Launcher Updates](MEDIA/3.png)
