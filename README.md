# NOTE!!! PLEASE READ THIS BEFORE YOU START
When downloading this repo and specifically DLL files in this repo,
Windows will set the DLL files as untrusted.

You need to:

1. Right click the DLL file 

2. Select properties

3. Check the Unblock checkbox on the bottom right side

If you don't do this, the DLL will fail to load in Overwolf and 
the your app will crash.

The other alternative is to build the DLL yourself with Visual Studio.



# overwolf-plugins
A solution with a bunch of Overwolf plugin dlls

Includes an Overwolf sample WebApp to show all features.

simple-io-plugin
================
Used to stream files from local disk.

process-manager
================
Allows launching external processes.

Question: How to detect if OBS is running, from within your app?
Answer: Use the process_manager.dll plugin in your app and call plugin().isProcessRunning("obs64")
1. Get it from: https://github.com/overwolf/overwolf-plugins/tree/master/dist
2. Download process_manager.dll
3. Right click the dll file > properties > check the Unlock checkbox on the bottom
4. Add the plugin to your manifest (see https://github.com/overwolf/overwolf-plugins/tree/master/sample_apps/process_manager) for reference
5. For any process you want to check, pass the process name (without the exe extension) to the isProcessRunning function


downloader
================
Allows downloading a remote files.
