# 📦 Update Format

- [Back to main](../README.md)

## Update Archive Contents

![update](MEDIA/5.png)

---

### `manifest.launcher`

The `manifest.launcher` file contains product and version information for the update.

Example:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<config>
    <name>LizeriumFreelancerMode</name>
    <version>99.3.12</version>
</config>
```

---

### `updates_info.json`

The `updates_info.json` file contains:

- information about the latest update
- list of changes
- update categories

<details>
<summary>Content</summary>

```json
{
	"Comment": "Enjoy the game — it's completely free.",
	"Categories": [
		{
			"name": "GAME_UNIVERSE",
			"title": "Universal client updates"
		},
		{
			"name": "GAME_SINGLE",
			"title": "Single-player updates"
		}
	],
	"Updates": [
		{
			"name": "99.5.1",
			"data": [
				{
					"category": "GAME_UNIVERSE",
					"values": ["text_GAME_UNIVERSE"]
				},
				{
					"category": "GAME_SINGLE",
					"values": ["text_GAME_SINGLE"]
				}
			]
		}
	]
}
```

</details>

---

## What Should Be Inside the Update Archive

The root of the archive must contain **only those files and folders that need to be changed** in Freelancer and related game data.

The structure must match the original Freelancer directory structure.

---

> [!IMPORTANT]
> The difference between versions is calculated **file-by-file programmatically**.
> Tools for calculating this difference are available both within this project and externally.

---

To compare game directories and extract differences, use:

🔗 [https://github.com/Lizerium/LizeriumFindChanges](https://github.com/Lizerium/LizeriumFindChanges)

This tool also generates a required manifest for correct update processing.
