#Русская документация
#Настройка и скачивание StemaCMD.exe
## 1. Скачайте SteamCMD.exe по ссылке https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip
## 2. Создайте папку (не закидывать туда, где сам Steam) и закиньте разархевируйте файл в папку
## 3. Создайте .bat файл с содержимым
```bash
@echo off
chcp 65001 > nul 2>&1
cls

C:\SteamCMD\steamcmd.exe +runscript C:\SteamCMD\scr.txt
```
  ### а. Вместо в пути укажите путь до steamcmd.exe, точнее куда разархивировали файл

## 4. После надо создать .txt файл(у меня это scr.txt вам как удобно) с содержимым:
```bash
@ShutdownOnFailedCommand 1
@NoPromptForPassword 1

login login password
app_update id_game validate
logout
quit
```
  ### а. Вместо логина и пароля поставте ваш реальный логин и пароль
  ### б. Обязательно Steam аккаунт должен быть без SteamGuard
  ### в. Вместо id_game поставьте id нужной игры (чтоб узнать id установленных игр, можете найти в каталоге стим клиента, в папке steamapps, там в названиях файлов id игр, выглядит примерно "appmanifest_730.acf"(это файл кс2))
## 5. готовность папок
  ### а. нужно найти каталог где находятся все уже установленные игры (...\steamapps\common\) и нужно скопировать этот путь
  ### б. создать в каталоге где steamcmd.exe создать папку steamapps и скопировать путь до этой папки
  ### в. зайти в cmd (Win+R, введите cmd, энтр) и введите сначало
```bash
cd C:\steamCMD\steamapps\
```
  #### вместо C:\steamCMD\steamapps\ введите путь из пункта Б
  ### г. после введите следующее
```bash
mklink common\ E:\steam\steamapps\common\
```
  #### вместо E:\steam\steamapps\common\ введите папку из пункта А
# 6. Скачать проект(из релиза)
  ### а. После распаковки перейдите в файл config.json и заполнить данные
```json
{
  //путь до батника из пункта 3
  "BatFilePath": "C:\\SteamCMD\\UpdateSteamGames.bat",
  //токен телеграмм бота
  "BotToken": "Ваш_токен_бота",
  //ваш_id_чата
  "ChatId": -0,
  //есть ли топики\треды в чате телеграмм
  "CheakTraid": true,
  //топик чата если CheakTraid == true
  "TaidId": 0
}
```
  ### б. откройте "Планировщик задач" (Win+R, ```taskschd.msc```)
  ### в. Создайте задачу с тригером на точное время, укажите время во сколько обновлять игры и действие: запуск приложение и укажите путь к ConsoleApp1.exe

