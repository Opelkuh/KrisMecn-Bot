using DSharpPlus;
using KrisMecn.Commands;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.CommandsNext;
using System.Text.RegularExpressions;

namespace KrisMecn.Extensions
{
    class SoundEffectWatcherExtension : BaseExtension
    {
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>()
        {
            ".mp3", ".opus", ".wav", ".ogg", ".flac", ".m4a", ".webm", ".mp4", ".mkv", ".avi"
        };
        private static readonly Regex _whitespaceRegex = new Regex(@"\s+");

        private readonly SoundEffectCommands _seCommands;
        private readonly HashSet<string> _registeredEffects = new HashSet<string>();
        private readonly string _sePath;
        private FileSystemWatcher _fsWatcher;

        public SoundEffectWatcherExtension(string path) : base()
        {
            _seCommands = new SoundEffectCommands();
            _sePath = path;

            // create path if it doesn't exist
            Directory.CreateDirectory(path);

            // prepare fs watcher
            _fsWatcher = new FileSystemWatcher(path);
            _fsWatcher.IncludeSubdirectories = true;
            _fsWatcher.NotifyFilter = NotifyFilters.FileName;
            _fsWatcher.Created += FsWatcher_Changed;
            _fsWatcher.Changed += FsWatcher_Changed;
            _fsWatcher.Deleted += FsWatcher_Changed;
            _fsWatcher.Renamed += FsWatcher_Renamed;

            // register file filters
            foreach(var extension in AllowedExtensions)
                _fsWatcher.Filters.Add($"*{extension}");
        }

        protected override void Setup(DiscordClient client)
        {
            Client = client;

            // register all existring se files
            RegisterAllFilesFromPath(_sePath);

            // start watching file changes
            _fsWatcher.EnableRaisingEvents = true;
        }

        protected void RegisterSoundEffectFromFile(string fullPath)
        {
            var cmdNext = Client.GetCommandsNext();
            var cmd = new CommandBuilder(_seCommands);
            var overload = new CommandOverloadBuilder(_seCommands.GetPlaySoundEffectCommand(fullPath));
            var cmdName = GetCommandNameFromPath(fullPath);

            // check if we're not trying to register a duplicate command
            if(cmdNext.RegisteredCommands.ContainsKey(cmdName))
            {
                Logger.Error("Sound Effect Watcher Error", $"Tried to register command name '{cmdName}' that already existed!");
                return;
            }

            // build command
            cmd.WithName(cmdName)
                .WithOverload(overload);

            // register command
            cmdNext.RegisterCommands(cmd);
            _registeredEffects.Add(cmdName);
        }

        protected void UnregisterSoundEffectFile(string fullPath)
        {
            var cmdName = GetCommandNameFromPath(fullPath);

            if (!_registeredEffects.Contains(cmdName)) return;

            _registeredEffects.Remove(cmdName);

            // remove command from DSharpPlus
            var cmdNext = Client.GetCommandsNext();

            Command cmd;
            if (!cmdNext.RegisteredCommands.TryGetValue(cmdName, out cmd)) return;

            cmdNext.UnregisterCommands(cmd);
        }

        protected string GetCommandNameFromPath(string path)
            => Path.GetFileNameWithoutExtension(_whitespaceRegex.Replace(path, "-"));

        protected void RegisterAllFilesFromPath(string path)
        {
            foreach(string filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                var extension = Path.GetExtension(filePath);
                if (!AllowedExtensions.Contains(extension)) continue;

                RegisterSoundEffectFromFile(filePath);
            }
        }

        private void FsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            switch(e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    RegisterSoundEffectFromFile(path); break;
                case WatcherChangeTypes.Deleted:
                    UnregisterSoundEffectFile(path); break;
            }
        }

        private void FsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            UnregisterSoundEffectFile(e.OldFullPath);
            RegisterSoundEffectFromFile(e.FullPath);
        }
    }
}
