# ⚙️ Сборка проекта

## Сборка `.exe`

1. Перейдите в папку проекта [LizeriumSteam](../LizeriumSteam)
2. Создайте папку `ProjectConfigs`
3. Внутри `ProjectConfigs` создайте файл `config.xml`
4. В качестве шаблона используйте [config.default.xml](../LizeriumSteam/DefaultConfigs/config.default.xml)
5. Заполните `config.xml` своими значениями перед сборкой проекта

## Запуск

1. В файле `config.xml` необходимо указать адрес вашего сервера обновлений
2. Для работы проекта требуется собственный сервер
3. Исходный код серверной части находится здесь: [LizeriumServer](https://github.com/Lizerium/LizeriumServer)

## Важно

- Файл `config.xml` **не входит в репозиторий**
- Перед сборкой его необходимо создать вручную
- Не используйте тестовые или локальные значения (`localhost`, `127.0.0.1`) в production-сборке
