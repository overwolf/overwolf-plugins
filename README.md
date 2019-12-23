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

downloader
================
Allows downloading a remote files.
