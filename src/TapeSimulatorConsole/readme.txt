
Please change the configuration before start simulator application.

How to change the configuration?
1. Open TapeSimulatorConfig.xml to change the configuration.

**********************************************************************
2. Have to Change "ClientGuid" to ignore all the simulators use same client GUID.
***********************************************************************

3. Change Host, Port, UserName, Password as same as configured by Extended Storage Server.
4. If want to send a different file to Extended Storage Server, please change VideoFilePath and SendTimesPerMinute.
As we know, the send speed should bigger than or equals 24 MB/s. 
So the appliance has to send 24*60=1440 MB to Extended Storage Server in one minute .
So if you want to send a different file, check the file size, and then count the send times by 1440MB/{FileSize}.

For Example: 
If you want use {demo.avi}, 

1). First check the file size. We assume it is 4 MB.
2). Count send times. 1440/4= 360
3). Change VideoFilePath to demo.avi, change SendTimesPerMinute to 360.


Start TapeSimulatorConsole.exe to test send speed.

