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
  <b>Lizerium Launcher</b> — лаунчер и система постепенного обновления клиента и игровых модификаций
  для экосистемы <b>Lizerium</b>, рассчитанная на минимальный трафик, контроль версий и серверную публикацию.
</p>

# Оглавление

- [Оглавление](#оглавление)
  - [Общее](#общее)
  - [Технологии](#технологии)
  - [Возможности](#возможности)
  - [Документация](#документация)
  - [Связанные проекты](#связанные-проекты)
  - [Другое](#другое)

## Общее

![PRISM версия](<docs/HISTORY/Переход на PRISM.png>)

> [!IMPORTANT]
> Цель компонента — обеспечить постоянное и постепенное обновление приложений без вмешательства пользователя.
> Поведение аналогично механике обновления Google Chrome: каждое обновление загружает минимально возможный объём данных.

> [!NOTE]
> Проект использует локальный конфиг, который подхватывается во время сборки.

## Технологии

- WPF
- Prism.Unity
- .NET

## Возможности

- Постепенное обновление лаунчера
- Дельта-обновления между версиями
- Публикация игровых модификаций
- Минимизация трафика обновлений
- Интеграция с серверной частью
- Поддержка версионирования сборок

## Документация

- [Сборка проекта](docs/BUILD.md)
- [Публикация обновлений лаунчера](docs/PUBLISH_LAUNCHER.md)
- [Публикация модов](docs/PUBLISH_MODS.md)
- [Формат обновлений](docs/UPDATE_FORMAT.md)
- [Архитектура проекта](docs/Архитектура.md)
- [История визуальных изменений](docs/HISTORY)
- [Конкуренты | Наблюдения](docs/LAUNCHER_CONCURENTS)

## Связанные проекты

- [LizeriumServer](https://github.com/Lizerium/LizeriumServer)
- [Lizerium Find Changes](https://github.com/Lizerium/LizeriumFindChanges)

## Другое

> [!TIP]
> В папке `Launcher`, как правило, в корне сервера (`LizeriumServer`) лежит актуальная установочная версия файла `LizeriumLauncher`.
