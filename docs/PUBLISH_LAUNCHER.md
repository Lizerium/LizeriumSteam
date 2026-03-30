# 🚀 Публикация обновлений лаунчера

## Компонент публикации

Для публикации используется:

- [AppUpdater.Publisher](../AppUpdater.Publisher/bin/Release)

![Publisher](MEDIA/2.png)

## Подготовка перед публикацией

Перед формированием новой версии необходимо:

1. Указать в `config.xml` лаунчера:
   - актуальную версию приложения в поле `<version>`
   - предыдущую версию в поле `<last_version>`
   - адрес сервера обновлений в `updateServer`, например:

   ```xml
   <updateServer>https://lizup.ru/uploader/</updateServer>
   ```

2. Проверить пути до Freelancer и мода — они должны быть стандартными.

3. Поместить `config.xml` внутрь папки с обновлением (например, `1.0.5`) **до сборки**.

4. Подписать все созданные вами библиотеки электронной подписью через **kSign** с использованием ключа `dvurechensky.pfx`.

5. Удалить лишние:
   - `.pdb` файлы
   - лог-файлы, если они были созданы в процессе сборки или тестирования

## Генерация файлов обновления

Запускаем в **PowerShell** команду:

```bash
.\AppUpdater.Publisher.exe -source:{AppFolder} -target:{ReleaseFolder} -version:{X.X.X} -deltas:{X}
```

## Параметры

1. `-source` — путь до собранного свежего приложения
2. `-target` — папка публикации, из которой новая версия будет загружена на сервер обновлений
3. `-version` — номер версии приложения, который обычно зашивается при сборке (`1.0.0`, `1.2.3` и т.д.) и указывается в конфигурации
4. `-deltas` — количество предыдущих версий, для которых будут сформированы дельта-обновления

Рекомендуемое значение: `2`

<details>
<summary>Примеры</summary>

```sh
.\AppUpdater.Publisher.exe -source:..\..\..\TestApp\bin\Debug\ -target:.\PUBLISH\ -version:1.2.5 -deltas:2
```

> Актуальная команда

```sh
.\AppUpdater.Publisher.exe -source:..\..\..\LizeriumLauncher\bin\Release\ -target:.\PUBLISH\ -version:1.0.1 -deltas:2
```

</details>

## Публикация на сервер

После генерации необходимо загрузить на сервер:

- новый [`version.xml`](../AppUpdater.Publisher/bin/Release/PUBLISH/version.xml), в котором указана последняя версия приложения
- папку с этой версией из [`PUBLISH`](../AppUpdater.Publisher/bin/Release/PUBLISH)

То есть, если собиралась версия `1.0.4`, то в корень папки `uploader` на сервере (`LizeriumServer`) необходимо положить:

- `version.xml`
- папку `1.0.4`

Пример структуры:

```text
uploader/
├── version.xml
└── 1.0.4/
```

![Launcher Updates](MEDIA/3.png)
