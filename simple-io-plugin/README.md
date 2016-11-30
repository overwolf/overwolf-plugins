simple-io-plugin
================
Used to read files from local disk.
Includes an Overwolf sample WebApp to show all features.

Constants:
==========
```
plugin.get().PROGRAMFILES
plugin.get().COMMONFILES
plugin.get().COMMONAPPDATA
plugin.get().DESKTOP
plugin.get().WINDIR
plugin.get().SYSDIR
plugin.get().SYSDIRX86
plugin.get().MYDOCUMENTS
plugin.get().MYVIDEOS
plugin.get().MYPICTURES
plugin.get().MYMUSIC
plugin.get().COMMONDOCUMENTS
plugin.get().FAVORITES
plugin.get().FONTS
plugin.get().HISTORY
plugin.get().STARTMENU
plugin.get().LOCALAPPDATA
```

Functions:
==========
NOTE: Don't call other plugin APIs from callback functions - it will freeze the application

NOTE: Because of a chromium bug - sometimes that plugin blocks on propagating events - to circumvent this use the
following code in your app (at a global scope):

setInterval(function() {
  document.getElementById('plugin');
}, 1000);


- fileExists - check if a file exists locally (notice the way we use /, otherwise you need \\)

```
plugin.get().fileExists(
  plugin.get().PROGRAMFILES + "/overwolf/Overwolf.exe.config", 
  function(status) {
  
  if (status === true) {
  } else {
  }
});
```

- isDirectory - check if a given path is a directory (false if not or doesn't exist)

```
plugin.get().isDirectory(
  plugin.get().PROGRAMFILES + "/overwolf", 
  function(status) {
  
    if (status === true) {
    } else {
    }
});
```

- listDirectory - return a directory's content.
NOTE: this function returns a JSON object with the content of a directory, in a descending order by the last time modified (last write time).
if this function fails, an error message will be returned.

```
io.get().listDirectory(
  io.get().PROGRAMFILES,
  function(status, result) { 
    if(status === true) {
	  directory = JSON.parse(result);
	  directory.map(function(content) { 
	    if(content.type == "file") {
	      console.log(content.name);
		} else {
	      console.log(result);
		}
	});
  }
```

- createDirectory - create a new directory in the Local App Data path, and return wether it exists
NOTE: this function returns whether the directory *exists* or not.
if you want to check whether the directory was *created*, you should
use isDirectory before the creation operation.

``` 
var directory = "MYDOCUMENTS"
var mydirectoryname = "myapp"

plugin.get().createDirectory(
  directory, mydirectoryname,
  function(status, message) {
  
    if(status === true) {
    } else {
    }
});
  
```

- getTextFile - reads a file's contents and returns as text.
Use the second parameter to indicate if the file is in UCS-2 (2 bytes per char) and
it will automatically do the UTF8 conversion.  Otherwise, returns in UTF8

```
plugin.get().getTextFile(
  plugin.get().PROGRAMFILES + "/overwolf/Overwolf.exe.config", 
  false, // not a UCS-2 file
  function(status, data) {
          
  if (!status) {
    console.log("failed to get Overwolf.exe.config contents");
  } else {
    console.log(data);
  }
});
```
        
- getBinaryFile - reads a file's contents and returns as an array of byte values.
NOTE: this function is extremly slow! Use only for small files or to get file header
info using the second parameter (limit) to limit amount of data to fetch

```
plugin.get().getBinaryFile(
  plugin.get().PROGRAMFILES + "/overwolf/Overwolf.exe.config",
  -1, // no limits
  function(status, data) {
    if (!status) {
      console.log("failed to get Overwolf.exe.config");
    } else {
      var arr = data.split(",");
      console.log(arr);
    }
});
```

- writeLocalFile - Create a file on the local filesystem with given text content. For security reasons, we only allow to write in specific, pre-defined folders.
Note: can't append to files. This function will either create a new file or overwrite the previous one (based on implementation).

```
var directory = "LOCALAPPDATA";
var filename = "/folder/file.txt";
var content = "1234\n56768";
plugin.get().writeLocalFile(directory, filename, content, function(status, message)
  {
    console.log(arguments);
  });
```

- plugin.get().listenOnFile - Stream a file on the local filesystem to a javascript callback (text files only)
NOTE: don't call other plugin APIs from callback

```
var fileIdentifier = "my-id";
var filename = "c:/folder/file.log";
var skipToEndOfFile = false;
plugin.get().listenOnFile(fileIdentifier, filename, skipToEndOfFile, function(fileId, status, data) {
  if (fileId == fileIdentifier) {
    if (status) {
      console.log("[" + fileId + "] " + data);
    } else {
      console.log('something bad happened: ' + data);
    }
  }
});
```

- plugin.get().stopFileListen - Stop streaming a file that you previously passed when calling |listenOnFile|
NOTE: there are no callbacks - as this will never fail (even if the stream doesn't exist)
NOTE: you will get a callback to |listenOnFile| with status == false when calling this function

```
var fileIdentifier = "my-id";
plugin.get().stopFileListen(fileIdentifier);
```

- plugin.get().getLatestFileInDirectory - Retreieve the most updated (latest accessed) file in a given folder (good for game logs)

```
var folder = "c:/game/logs/*"; // no extension filtering
plugin.get().getLatestFileInDirectory(folder, function(status, filename) {
  if (status) {
    console.log("The most update file in this folder is: " + filename);
  } else {
    console.log("No file found");
  }
});
```
