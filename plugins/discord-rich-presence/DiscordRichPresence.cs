using System;
using DiscordRPC;
using DiscordRPC.Message;
using Newtonsoft.Json;

namespace com.overwolf.discord {
  public class DiscordRichPresence : IDisposable {
    /// <summary>
    /// The following global events are directly connected to DiscordRpcClient's
    /// events - if you wish to add more events you may (e.g.
    /// DiscordRpcClient.OnClose).
    ///
    /// The objects that will be passed to the app can be reviewed in the
    /// DiscordRpcClient library
    /// </summary>
    public event Action<object> onReady = null;
    public event Action<object> onError = null;
    public event Action<object> onPresenceUpdate = null;

    private string _currentAppId = String.Empty;
    private DiscordRpcClient _discordRpcClient = null;

    public DiscordRichPresence() {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="appId"></param>
    /// <exception cref="Exception"></exception>
    public void initApp(string appId) {
      StringComparison compare = StringComparison.InvariantCultureIgnoreCase;
      if (_currentAppId.Equals(appId, compare)) {
        throw new Exception($"Already initialized with this app id! ({appId})");
      }

      try {
        DisposeDiscordRpcClientIfExists();

        _discordRpcClient = new DiscordRpcClient(appId);
        _discordRpcClient.OnReady += discordRpcClient_OnReady;
        _discordRpcClient.OnError += discordRpcClient_OnError;
        _discordRpcClient.OnPresenceUpdate += discordRpcClient_OnPresenceUpdate;

        _discordRpcClient.Initialize();

        _currentAppId = appId;
      } catch (Exception e) {
        throw new Exception(e.Message);
      }
    }

    /// <summary>
    /// See RichPresence for values
    /// </summary>
    /// <param name="richPresenceObj"></param>
    public void setPresence(object richPresenceObj) {
      if (_discordRpcClient == null) {
        throw new Exception("Not initialized!");
      }

      string json = richPresenceObj as string;

      var richPresence = JsonConvert.DeserializeObject<RichPresence>(json);
      _discordRpcClient.SetPresence(richPresence);
    }

    /// <summary>
    ///
    /// </summary>
    public void clearPresence() {
      _discordRpcClient.ClearPresence();
    }

    /// <summary>
    ///
    /// </summary>
    public void Dispose() {
      DisposeDiscordRpcClientIfExists();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void discordRpcClient_OnReady(object sender, ReadyMessage args) {
      onReady?.Invoke(args);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void discordRpcClient_OnError(object sender, ErrorMessage args) {
      onError?.Invoke(args);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void discordRpcClient_OnPresenceUpdate(object sender,
                                                   PresenceMessage args) {
      onPresenceUpdate?.Invoke(args);
    }


    /// <summary>
    /// Trying to reduce leaks
    /// </summary>
    private void DisposeDiscordRpcClientIfExists() {
      if (_discordRpcClient == null) {
        return;
      }

      _discordRpcClient.OnReady -= discordRpcClient_OnReady;
      _discordRpcClient.OnError -= discordRpcClient_OnError;
      _discordRpcClient.OnPresenceUpdate -= discordRpcClient_OnPresenceUpdate;

      _discordRpcClient.Dispose();
      _discordRpcClient = null;
      _currentAppId = String.Empty;
    }
  }
}
