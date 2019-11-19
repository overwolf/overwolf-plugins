var plugin = new OverwolfPlugin("process-manager-plugin", true);
var processId = 0;

plugin.initialize(status => {
  if (status == false) {
    console.error("Plugin couldn't be loaded??");
    return;
  }
  
  const path = "notepad.exe";
  const args = "";
  const environmentVariables = { };

  const hidden = true;

  // only relevant for executables that write info back to the launcher - 
  // notepad.exe isn't one of them
  plugin.get().onDataReceivedEvent.addListener(({ error, data }) => {
    if (error) {
      console.error(error);
    }

    if (data) {
      console.log(data);
    }
  });

  let _processId = -1;
  plugin.get().onProcessExited.addListener(({processId, exitCode}) => {
    console.log(`process exit - pid=${processId} exitCode=${exitCode}`);
    if (_processId == processId) {
      processId = -1;
    }
  });

  plugin.get().launchProcess(path, 
                             args, 
                             JSON.stringify(environmentVariables), 
                             hidden, 
                             false, // if we close the app, don't close notepad
                             ({ error, data }) => {
    if (error) {
      console.error(error);
    }

    if (data) {
      _processId = data;
      console.log(`process launched - pid=${_processId}`);
    }
  });
 
  //plugin.get().suspendProcess(processId,console.log)
  //plugin.get().resumeProcess(processId,console.log)
  //plugin.get().terminateProcess(processId)
 });
