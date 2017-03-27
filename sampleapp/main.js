var plugin = new OverwolfPlugin("simple-io-plugin", true);
plugin.initialize(function(status) {
  if (status == false) {
    document.querySelector('#title').innerText = "Plugin couldn't be loaded??";
    return;
  }

  function addMessage(message) {
    var obj = document.createElement("div");
    obj.innerText = message;
    document.querySelector('#messages').appendChild(obj);
  }
  
  function clearMessage() {
	var messageDiv = document.querySelector('#messages');
	while (messageDiv.hasChildNodes()) {
    messageDiv.removeChild(messageDiv.lastChild);
	}
    
  }


  document.querySelector('#title').innerText =
    "Plugin " + plugin.get()._PluginName_ + " was loaded!";


  //    for (i = 0; i < 500000; i++) {
//        plugin.get().IsValidUrl();
//      }
  /////////
  addMessage("PROGRAMFILES = " + plugin.get().PROGRAMFILES);
  addMessage("PROGRAMFILESX86 = " + plugin.get().PROGRAMFILESX86);
  addMessage("COMMONFILES = " + plugin.get().COMMONFILES);
  addMessage("COMMONFILESX86 = " + plugin.get().COMMONFILESX86);
  addMessage("COMMONAPPDATA = " + plugin.get().COMMONAPPDATA);
  addMessage("DESKTOP = " + plugin.get().DESKTOP);
  addMessage("WINDIR = " + plugin.get().WINDIR);
  addMessage("SYSDIR = " + plugin.get().SYSDIR);
  addMessage("SYSDIRX86 = " + plugin.get().SYSDIRX86);
  addMessage("MYDOCUMENTS = " + plugin.get().MYDOCUMENTS);
  addMessage("MYVIDEOS = " + plugin.get().MYVIDEOS);
  addMessage("MYPICTURES = " + plugin.get().MYPICTURES);
  addMessage("MYMUSIC = " + plugin.get().MYMUSIC);
  addMessage("COMMONDOCUMENTS = " + plugin.get().COMMONDOCUMENTS);
  addMessage("FAVORITES = " + plugin.get().FAVORITES);
  addMessage("FONTS = " + plugin.get().FONTS);
  addMessage("HISTORY = " + plugin.get().HISTORY);
  addMessage("STARTMENU = " + plugin.get().STARTMENU);
  addMessage("LOCALAPPDATA = " + plugin.get().LOCALAPPDATA);
  addMessage("\n");

   // setInterval(function() {
	 // plugin.get().fileExists(
  // plugin.get().PROGRAMFILES + "/overwolf/Overwolf.1exe.config",
  // function(status) {
   // console.log("fileExists:", status);
   // if (status === true) {
      // // // // addMessage(
        // // // // plugin.get().PROGRAMFILES +
        // // // // "/overwolf/Overwolf.exe.config" +
        // // // // " exists on disk!");
     // } else {
      // // // // addMessage(
        // // // // plugin.get().PROGRAMFILES +
        // // // // "/overwolf/Overwolf.exe.config" +
        // // // // " does NOT exist on disk!");
     // }
   // });
	  
   // }, 100);


  plugin.get().isDirectory(
  plugin.get().PROGRAMFILES + "/overwolf",
  function(status) {

    if (status === true) {
      addMessage(
        plugin.get().PROGRAMFILES +
        "/overwolf" +
        " exists and is a directory");
    } else {
      addMessage(
        plugin.get().PROGRAMFILES +
        "/overwolf" +
        " is not a directory or does NOT exist!");
    }
  });

  plugin.get().getTextFile(
    plugin.get().PROGRAMFILES +
    "/overwolf/Overwolf.exe.config",
    false, // not a widechars file (i.e. not ucs-2)
    function(status, data) {

      if (!status) {
        addMessage("failed to get Overwolf.exe.config");
      } else {
        addMessage(data);
      }
  });

  plugin.get().getBinaryFile(
    plugin.get().PROGRAMFILES +
    "/overwolf/Overwolf.exe.config",
    -1, // no limits
    function(status, data) {

      if (!status) {
        addMessage("failed to get Overwolf.exe.config");
      } else {
        var arr = data.split(",");
        addMessage(arr);
      }
  });

   //var file_name = "e:\\Games\\Hearthstone\\Logs\\Power.log";
  var file_name = "c:\\temp\\2017-03-25T19-17-46_r3dlog.txt";
  var index =0;
  plugin.get().onFileListenerChanged.addListener(function(id, status, line) {
     console.log("index" + index + ": " + line);
     index++;
	  addMessage(line);
	  if (status == false && line == "truncated") {
		  console.log("truncated");
		 clearMessage();
		plugin.get().listenOnFile("file-id", file_name, false, function(id, status, data) {
			console.log("Start listen on file: ", file_name, id, status, data);
		})		 
	  }
  });
  
 
  plugin.get().listenOnFile("file-id", file_name, false, function(id, status, data) {
		console.log("Start listen on file: ", file_name, id, status, data);
  });
  //plugin.get().stopFileListen("file-id");
  plugin.get().listDirectory(  "c:\Users\elad.bahar\AppData\Local\Overwolf", function(status, filename ) {
	   addMessage("getLatestFileInDirectory" + filename);

  });
  
  var processId = 12964;
  plugin.get().listenOnProcess(processId,  function(status, error) {
			console.log("listen process: ", processId, status, error);
  })
  // plugin.get().stopListenProcess(processId,  function(status, data) {
			// console.log("stop listen process: ", processId, status, data);
  // })		 
    
    
   plugin.get().onOutputDebugString.addListener(function(processId, line) {
    console.log("onOutputDebugString" + processId + ": " + line);
   });
});
