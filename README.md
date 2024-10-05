# BlueWP

![image](https://raw.githubusercontent.com/Gargaj/BlueWP/main/BlueWP/Assets/Square44x44Logo.scale-200.png)

Bluesky client for Windows Phone

![image](https://raw.githubusercontent.com/Gargaj/BlueWP/main/screenshot.jpg)

Don't ask why.

## Building
Everything should be in `BlueWP.sln`; the solution is pinned to use VS2017 so that you can deploy directly
to the device, but you should be able to compile and build packages with VS2022 as well.

## Installation
- Enable Developer mode on your phone (Settings > Update & Security > For developers)
- Enable Device discovery and Device Portal
- Connect your phone to your WiFi and open the address provided under "Connect using:"
- In the browser, follow the instructions to pair your phone to your computer
- In the device portal's App Manager, select the `.appxbundle` file from the downloaded release
- If this is your first time installing, add the contents of the Dependencies directory using the "App dependency" button
- Press Go and wait until it installs; ensure your phone doesn't lock while it's installing.

## Todo
- Posting: Autocomplete handles
- Search
- Context-menu stuff (copy URL etc)
