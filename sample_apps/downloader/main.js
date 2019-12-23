var plugin = new OverwolfPlugin("downloader-plugin", true);

plugin.initialize(status => {
  if (status == false) {
    console.error("Plugin couldn't be loaded??");
    return;
  }
  
  plugin.get().onDownloadError.addListener(({ url, reason }) => {
    console.log(`Error: ${url} - ${reason}`);
  });

  plugin.get().onDownloadProgress.addListener(({ url, progress }) => {
    console.log(`Progress: ${url} - ${progress}`);
  });
  
  plugin.get().onDownloadComplete.addListener(({ url, localFile, md5 }) => {
    console.log(`Download Complete: ${localFile} MD5: ${md5}`);
    
  });
  
  overwolf.extensions.current.getManifest(result => {
    const remoteFile = 
      "https://download.overwolf.com/install/Download?Channel=web_dl_btn";
    const extId = result.UID;
    
    const prgmData = overwolf.io.paths.commonAppData;    
    const localFile = `${prgmData}\\overwolf\\extensions\\${extId}\\setup.exe`;
    plugin.get().downloadFile(remoteFile, localFile);
  });
  

});
