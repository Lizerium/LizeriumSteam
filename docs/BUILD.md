# ⚙️ Build Instructions

- [Back to main](../README.md)

## Building the `.exe`

1. Navigate to the project folder: [LizeriumSteam](../LizeriumSteam)
2. Create a folder named `ProjectConfigs`
3. Inside `ProjectConfigs`, create a file named `config.xml`
4. Use this template as a base: [config.default.xml](../LizeriumSteam/DefaultConfigs/config.default.xml)
5. Fill `config.xml` with your own values before building the project

---

## Running

1. In `config.xml`, specify the address of your update server
2. The project requires a dedicated backend server to function
3. Server-side source code is available here:  
   [LizeriumServer](https://github.com/Lizerium/LizeriumServer)

---

## Important

- The `config.xml` file is **not included in the repository**
- It must be created manually before building
- Do NOT use test or local values (`localhost`, `127.0.0.1`) in production builds
