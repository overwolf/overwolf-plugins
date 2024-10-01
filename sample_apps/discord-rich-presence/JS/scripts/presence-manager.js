// Demonstration of updating the presence "for the first time"
function firstPresence() {
  log('[discord] Setting first presence!');
  DRP.updatePresence(
    'Overwolf Rich Presence',
    'First Presence',
    'overwolf-icon',
    'Overwolf DRP Large Image',
    'small_image_key',
    'Small image',
    true,
    0,
    'Button 1',
    'https://www.overwolf.com/',
    'Button 2',
    'https://www.overwolf.com/',
    console.log
  );
}

// Demonstration of updating the presence "for the second time"
function updatePresence() {
  log('[discord] Updating presence!');
  DRP.updatePresence(
    'Overwolf Updated Rich Presence',
    'Updated Presence',
    'overwolf-black',
    'Overwolf Updated DRP Large Image',
    'small_image_key',
    'Small image',
    true,
    0,
    'Button 1',
    'https://www.overwolf.com/',
    'Button 2',
    'https://www.overwolf.com/',
    console.log
  );
}

// Demonstration of updating the presence with buttons
const buttonsJson =
  '[{"label": "Overwolf", "url": "https://www.overwolf.com/"}]';

function updatePresenceWithButtons() {
  DRP.updatePresenceWithButtonsArray(
    'Rich Presence with Buttons',
    'Updated Presence with Buttons',
    'overwolf-black',
    'Overwolf DRP Large Image',
    'small_image_key',
    'Small image',
    true,
    0,
    buttonsJson,
    console.log
  );
}

// Demonstration of updating the presence based on game info updated events
function updatePresenceForGame() {
  log('[Overwolf] Registering to game launch event');
  overwolf.games.onGameInfoUpdated.addListener((result) => {
    console.log(result);

    if (
      result.reason[0] === 'gameRendererDetected' ||
      result.reason[0] === 'gameLaunched'
    ) {
      log(
        `[discord] Updating presence based on game launched: ${result.gameInfo.title}`
      );
      DRP.updatePresence(
        `Now Playing ${result.gameInfo.title}`,
        `Playing ${result.gameInfo.title} while using DRP-Sample`,
        'overwolf-black',
        'Overwolf DRP Large Image',
        'small_image_key',
        'Small image',
        true,
        0,
        'Button 1',
        'https://www.overwolf.com/',
        'Button 2',
        'https://www.overwolf.com/',
        console.log
      );
    }

    if (result.reason[0] === 'gameTerminated') {
      firstPresence();
    }
  });
}
