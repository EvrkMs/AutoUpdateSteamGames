using System;

namespace AutoUpdateSteamGames.Models
{
    public class Config
    {
        public string BatFilePath { get; set; }
        public string BotToken { get; set; }
        public long ChatId { get; set; }
        public bool CheakTraid { get; set; }
        public int TaidId { get; set; }
        public string SteamAppsPath { get; set; }
        public string ClientPath { get; set; }
        public string CmdPath { get; set; }
        public string Message { get; set; }
    }
}
