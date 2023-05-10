using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Yuna.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {

        private readonly LavaNode _lavaNode;
        public static bool LoopEnabled { get; set; } = false;
        public static LavaTrack CurrentTrack { get; set; }
        public static AudioModule Instance { get; private set; }
        public AudioModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
            Instance = this;
            _lavaNode.OnTrackEnd += _lavaNode_OnTrackEnd;

        }

        private Task _lavaNode_OnTrackEnd(Victoria.Node.EventArgs.TrackEndEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
        {
            if (LoopEnabled && arg.Player.Track == null && CurrentTrack != null)
            {
                arg.Player.PlayAsync(CurrentTrack);
                return Task.CompletedTask;
            }

            var areMoreTracks = arg.Player.Vueue.TryDequeue(out var nextTrack);
            if (areMoreTracks)
            {
                arg.Player.PlayAsync(nextTrack);
            }

            return Task.CompletedTask;
        }
                
        public Tuple<IGuild, ITextChannel, IVoiceState> GetDiscordContext()
        {
            try
            {
                return new Tuple<IGuild, ITextChannel, IVoiceState>(Context.Guild, (ITextChannel)Context.Channel, (IVoiceState)Context.User);
            }
            catch (Exception)
            {

                return default;
            }
            
        }
        

        // We pass an Audio service task to a section that usually requires embedding, since this is what all audio Service tasks return.

        #region Clear
        [Command("clear", RunMode = RunMode.Async)]
        [Name("Clear"), Summary("Clears the playlist.")]
        public async Task Clear() => await ReplyAsync(embed: await PlayerClear.ClearAsync(_lavaNode, Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));
        #endregion

        #region Join
        [Command("join", RunMode = RunMode.Async)]
        [Name("join"), Summary("Connects a bot to a voice channel.")]
        public async Task Join() => await ReplyAsync(embed: await PlayerJoin.JoinAsync(_lavaNode, Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));
        #endregion

        #region Leave
        [Command("leave", RunMode = RunMode.Async)]
        [Name("Leave"), Summary("Disconnects the bot from the voice channel.")]
        public async Task Leave() => await ReplyAsync(embed: await PlayerLeave.LeaveAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Loop
        [Command("loop", RunMode = RunMode.Async)]
        [Name("Loop"), Summary("Loops the current track.")]
        public async Task Loop() => await ReplyAsync(embed: await PlayerLoop.LoopAsync(_lavaNode, Context.Guild));
        #endregion

        #region Move
        [Command("move", RunMode = RunMode.Async)]
        [Name("Move"), Summary("Moves a track to the top of the Queue.")]
        public async Task Move(ushort index) => await PlayerMove.MoveAsync(_lavaNode, Context.Guild, index);
        #endregion

        #region NowPlaying
        [Command("nowplaying", RunMode = RunMode.Async)]
        [Name("NowPlaying"), Summary("Lists tracks.")]
        public async Task NowPlayng() => await ReplyAsync(embed: await PlayerNowPlaying.NowPlayingAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Pause
        [Command("pause", RunMode = RunMode.Async)]
        [Name("Pause"), Summary("Pauses the current track.")]
        public async Task Pause() => await ReplyAsync(embed: await PlayerPause.PauseAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Play
        [Command("play", RunMode = RunMode.Async)]
        [Name("Play"), Summary("Plays a song with the given name or url.")]
        public async Task Play([Remainder] string search)=> await ReplyAsync(embed: await PlayerPlay.PlayAsync(_lavaNode, Context.User as SocketGuildUser, Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel, search));
        #endregion

        #region Playlist
        [Command("playlist", RunMode = RunMode.Async)]
        [Alias("list")]
        [Name("Playlist"), Summary("Displays the list of tracks in the playlist")]
        public async Task Playlist(ushort page) =>await ReplyAsync(embed: await PlayerPlaylist.PlaylistAsync(_lavaNode, Context.Guild, Context.User as IVoiceState, page));
        #endregion

        #region Remove
        [Command("remove", RunMode = RunMode.Async)]
        [Name("Remove"), Summary("Removes a specific record from the sheet.")]
        public async Task Remove(ushort index) => await ReplyAsync(embed: await PlayerRemove.RemoveAsync(_lavaNode, Context.Guild, Context.User as IVoiceState, index));
        #endregion

        #region Replay
        [Command("replay", RunMode = RunMode.Async)]
        [Alias("restart")]
        [Name("Replay"), Summary("Replays a track from the beginning")]
        public async Task Replay() => await ReplyAsync(embed: await PlayerReplay.ReplayAsync(_lavaNode, Context.Guild));
        #endregion

        #region Resume
        [Command("resume", RunMode = RunMode.Async)]
        [Name("Resume"), Summary("Resumes the current track.")]
        public async Task Resume() => await ReplyAsync(embed: await PlayerResume.ResumeAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Search

        #endregion

        #region Seek
        [Command("seek", RunMode = RunMode.Async)]
        [Name("Seek"), Summary("Seeks to a specific part of the current song.")]
        public async Task Seek(string seekTime) => await ReplyAsync(embed: await PlayerSeek.SeekAsync(_lavaNode, Context.Guild, seekTime));
        #endregion

        #region Shuffle
        [Command("shuffle", RunMode = RunMode.Async)]
        [Name("Shuffle"), Summary("Shuffles the tracks in the Queue")]
        public async Task Shuffle() => await ReplyAsync(embed: await PlayerShuffle.ShuffleAsync(_lavaNode, Context.Guild));
        #endregion

        #region Skip
        [Command("skip", RunMode = RunMode.Async)]
        [Name("Skip"), Summary("Skips the current track.")]
        public async Task Skip() => await ReplyAsync(embed: await PlayerSkip.SkipAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Stop
        [Command("stop", RunMode = RunMode.Async)]
        [Name("Stop"), Summary("Stops playing music and clears the playlist.")]
        public async Task Stop() => await ReplyAsync(embed: await PlayerStop.StopAsync(_lavaNode, Context.Guild, Context.User as IVoiceState));
        #endregion

        #region Volume
        [Command("volume", RunMode = RunMode.Async)]
        [Alias("vol")]
        [Name("Volume"), Summary("Changes the volume of the bot (from 0 to 150).")]
        public async Task Volume(ushort volume) => await ReplyAsync(embed: await PlayerVolume.VolumeAsync(_lavaNode, Context.Guild, Context.User as IVoiceState, volume));
        #endregion

    }
}
