# eschoolbot

Бот-зеркало для нотификаций о новых оценках с сайта https://eschool.center

## Комманды

- /start - старт/логин
- /off - выключить бота

## Сборка и запуск бота

1. Откройте проект в Visual Studio
2. Откройте проект ESchoolBot в коммандной строке
3. Установите токен от телеграмма при помощи следующих комманд:
```sh
dotnet user-secrets init
dotnet user-secrets set "Config:BotToken" "token"
```
4. Нажмите F5 чтобы запустить бота!
