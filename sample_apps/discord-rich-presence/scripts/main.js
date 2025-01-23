let _discordRichPresence;

const logArea = document.getElementById('log');

function log(message) {
  logArea.innerHTML += message + '\n\n';
}

async function createPlugin() {
  return new Promise((resolve, reject) => {
    // Create a new instance of the plugin
    const plugin = new OverwolfPlugin('discordPlugin', true);

    // Initialize the plugin by the get method
    // and store the instance in the DRP variable
    plugin.initialize(async (status) => {
      if (status === true) {
        try {
          _discordRichPresence = await plugin.get();
          console.log('[discord] Plugin loaded!', status);
          log('[discord] Plugin loaded!');
          resolve();
        } catch (error) {
          log('[discord] Error loading plugin!');
          console.error(error);
        }
      }
    });
  });
}

async function initialize() {
  try {
    await createPlugin();

    // Registering the event listeners
    _discordRichPresence.onReady.addListener(msg => log(JSON.stringify(msg)));
    _discordRichPresence.onError.addListener(msg => log(JSON.stringify(msg)));
    _discordRichPresence.onPresenceUpdate
                        .addListener(msg => log(JSON.stringify(msg)));

    _discordRichPresence.initApp('1288821936851255389');
    log('[discord] Plugin initialized!');
  } catch (error) {
    log(`[discord] Error initializing plugin!\r\n${error}`);
    console.error(error);
  }
}

function dispose() {
  // Dispose connection to Discord
  _discordRichPresence.dispose();
  log('[discord] Disposing plugin!');
}

function initializeFromButton() {
  try {
    _discordRichPresence.initialize('1288821936851255389');
    log('[discord] Plugin initialized!');
  } catch (e) {
    log('[discord] Failed to initialize plugin!');
  }
}

function setPresence() {
  const withRunningTimestamp = true;
  const withImages = true;
  const withButtons = true;

  const details = 'Overwolf Rich Presence';
  const state = 'Playing a game';

  const presence = {
    details,
    state,
  };

  // timestamps
  if (withRunningTimestamp) {
    presence.timestamps = {
      start: Date.now(),
      end: null // This allows to limit the timer
    };
  }

  // images
  if (withImages) {
    presence.assets = {
     large_image: 'https://media.forgecdn.net/game-box-art/1_9b0a8ff4-90c0-4d72-967c-c2c60b8029f7.webp',
     large_text: 'sharmota',
     small_image: 'https://static-beta.curseforge.com/images/cf_legacy.png',
     small_text: 'slime'
    };
  }

  // buttons
  if (withButtons) {
    // Up to 2 buttons are allowed
    presence.buttons = [{
      label: 'Outplayed.tv',
      url: 'https://outplayed.tv'
    }, {
      label: 'CurseForge.com',
      url: 'https://www.curseforge.com'
    }]
  }

  try {
    _discordRichPresence.setPresence(presence);
  } catch (e) {
    log(e);
  }
}

function clearPresence() {
  _discordRichPresence.clearPresence();
}