# Настройка программы
### 1. После того как скачали дебаг проекта и распокавали, разместисте где вам будет удобно
### 2. При первом запуске обязательно надо прописать пути где находится клиент стим и куда вы закинули steamCMD.exe
    Это команды "downloadsteamcmd C:\your\path\" чтоб скачать steamcmd (если не доверяете, то можете сами скачать и указать путь до папкт с steamcmd по команде cmdpath)
    Это команды clientpath  (вводите путь до папки c клиентом стим, пример "clientpath E:\steam\" "
    После программа сохранит пути для дальнейшего и создаст символическую ссылку к папке с играми
    Так же после программа потребует ввести время, для того чтоб создать задачу в планировшике задач для автоматического обновления
### 3. После того как вы указали путь, можно выполнить команду collectlist, который соберёт id скачаных игр на клиент стим и напиши неполный скрипт сохранив его в scr1.txt
формат:
```
@ShutdownOnFailedCommand 1
@NoPromptForPassword 1
app_update 1001270 validate // Kebab Chefs! - Restaurant Simulator
app_update 1030840 validate // Mafia: Definitive Edition
app_update 105600 validate // Terraria
app_update 107410 validate // Arma 3
...
quit
```
после того как список игр внесён в скрипт, нужно дополнить его. Увы но программа не может обновить игры без аккаунта где её нет в библиотеке и пока-что не добавлен steamGuard чтоб можно было использовать его на полную
### 4. чтоб дополнить скрипт, в нужно вписать логирование
```
login your_login your_password
app_update id_game validate
logout
```
    помните что нужно на аккаунте отключить Steamguard
    так же помните что надо чтоб игра чей id вы ставите в логгирование, долна быть в библиотеке аккаунта на который вы логгируетесь 
### Созраняете файл с логгированием под название scr.txt
```
```
### 5. После того как настроили обновление, нужно настроить бота, чтоб он мог отправлять в чат автоматический о том что произошло обновление
```
        сперва нужно создать чат бота в телеграмм
        переходите к @BotFather, пишете /newbot выполняете инструкции и получаете токен бота(setting 1)
        после того как получили токе заходите в чат куда надо будет отсылать информацию о том, что произошло обновление и в ссылке на чат, находите -{число}, это и есть id чата, нам надо вместе со знаком -   (ввести в setting 2)
        после надо определится с топиком, это чат внутри которого много других чатов, выглядит так
```
![image](https://github.com/EvrkMs/AutoUpdateSteamGames/assets/167056681/1a403e6e-bf61-4bc8-b1be-87f4dc3b31d0)
```
        если он имеет деления, то в комнаде setting на вопрос "У вас есть топик?" надо ответить yes, если у вас обычый чаит, то нет (setting 3)
        возвращаемся если у вас всё же чат с топиками, вам надо достать тогда id топика, он находится после id чата через /
            то есть id_чата/id_топика
            выглядит примерно так
```
![image](https://github.com/EvrkMs/AutoUpdateSteamGames/assets/167056681/e14bc2f9-733d-444b-a4a9-62a502d7087d) (setting 4)
```
        после будет вопрос "Какое сообщение отправлять в чат?" сюда вводите что бот должен отписывать в чат после обновления (setting 5)
```
### 6. Если надо изменить каконкретные данные, а не все, можно задовать 
            
### по сути тут всё, остальное по обстоятельствам
```
updateplan - изменение времени для планирования задачи
+update - запуск обновления по скрипту scr.txt
help - сводка команд
```


Список команд
```json
+update                   - Запуск обновления игр.
updateplan                - Создание задачи для обновления игр по расписанию. Формат: updateplan {время}.
collectlist               - Сбор списка установленных игр и создание скрипта обновления.
clientpath                - Указание пути к клиенту Steam. Формат: clientpath {путь}.
cmdpath                   - Указание пути к SteamCMD. Формат: cmdpath {путь}".
createbat                 - Создание батника для обновления игр.
setting                   - Настройка конфигурации бота.
setting 1                 - Изменения токена бота в конфигурации отдельно от остальных вопросов.
setting 2                 - Изменения id чата отдельно от других вопросов.
setting 3                 - Изменения галочки использования топпиков чата.
setting 4                 - Изменения id топпика отдельно от других вопросов.
setting 5                 - Изменить отправку сообщения отельно от других вопросов.
downloadsteamcmd {path}   - Скачивание последней версии SteamCMD. Формат: downloadsteamcmd {путь}.
help                      - Показать справку по командам.
```
