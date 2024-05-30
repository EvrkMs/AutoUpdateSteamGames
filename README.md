# Русская документация
# Настройка и скачивание StemaCMD.exe
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



# English doc.
# SteamCMD Setup and Game Download
## 1. Download and Setup SteamCMD
  ### a. Download SteamCMD.exe from the link: https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip
  ### b. Create a folder (do not place it where Steam itself is located) and unzip the file into that folder.
  ### c. Create a .bat file with the following content:
```bat
Копировать код
@echo off
chcp 65001 > nul 2>&1
cls

"C:\SteamCMD\steamcmd.exe" +runscript "C:\SteamCMD\scr.txt"
```
  #### Replace the path with the location of steamcmd.exe, i.e., where you unzipped the file.
  ### 2. Create a script file
  ### Create a .txt file (in my case it's scr.txt, but you can name it as you like) with the following content:
```bash
Копировать код
@ShutdownOnFailedCommand 1
@NoPromptForPassword 1

login your_login your_password
force_install_dir "E:\steam\steamapps\common\Valheim\"
app_update 892970 validate
logout
quit
```
### Replace your_login and your_password with your actual Steam login and password.
### Make sure the Steam account does not have SteamGuard enabled.
### Replace 892970 with the ID of the desired game (you can find the ID of installed games in the Steam client directory, in the steamapps folder, where the file names look like "appmanifest_730.acf" (this is the file for CS
)).
## 3. Prepare directories
###  Prepare directories:
##  Find the directory where all installed games are located (e.g., ...\steamapps\common\) and copy this path.
##  Create a folder steamapps in the directory where steamcmd.exe is located and copy the path to this folder.
## Open CMD (Win+R, type cmd, press Enter) and first enter:
```bash
Копировать код
cd "C:\SteamCMD\steamapps\"
```
####  Replace C:\SteamCMD\steamapps\ with the path from the previous step.
### Then enter the following:
```bash
Копировать код
mklink /D common "E:\steam\steamapps\common\"
```
####  Replace E:\steam\steamapps\common\ with the path from the previous step.
##  4. Download and configure the project
###  Download the project (from the release).
###  After unpacking, go to the config.json file and fill in the data:
```json
Копировать код
{
  "BatFilePath": "C:\\SteamCMD\\UpdateSteamGames.bat",
  "BotToken": "Your_bot_token",
  "ChatId": -1002066018588,
  "CheakTraid": true,
  "TaidId": 6
}
```
###  Open "Task Scheduler" (Win+R, taskschd.msc).
### Create a task with a trigger at a specific time, set the time when you want to update the games, and the action: start the application and specify the path to ConsoleApp1.exe.
