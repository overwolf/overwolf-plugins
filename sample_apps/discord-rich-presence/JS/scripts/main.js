let DRP;

const logArea = document.getElementById('log');

function log(message) {
  logArea.innerHTML += message + '\n';
}

async function createPlugin() {
  return new Promise((resolve, reject) => {
    // Create a new instance of the plugin
    const plugin = new OverwolfPlugin('DiscordRPCPlugin', true);

    // Initialize the plugin by the get method
    // and store the instance in the DRP variable
    plugin.initialize(async (status) => {
      if (status === true) {
        try {
          DRP = await plugin.get();
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
    DRP.onClientReady.addListener(console.log);
    DRP.onPresenceUpdate.addListener(console.log);
    DRP.onClientError.addListener(console.error);
    DRP.onLogLine.addListener(console.info);

    // Initialize DRP with the application ID and the logging level
    DRP.initialize('1288821936851255389', 2, console.log);
    log('[discord] Plugin initialized!');
  } catch (error) {
    log('[discord] Error initializing plugin!');
    console.error(error);
  }
}

async function initializeAndShow() {
  try {
    await initialize();
    firstPresence();
  } catch (error) {
    log('[discord] Error in initializeAndShow!');
    console.error(error);
  }
}
initializeAndShow();

function dispose() {
  // Dispose connection to Discord
  DRP.dispose(console.log);
  log('[discord] Disposing plugin!');
}

function initializeFromButton() {
  DRP.initialize('1288821936851255389', 2, console.log);
  log('[discord] Plugin initialized!');
}
