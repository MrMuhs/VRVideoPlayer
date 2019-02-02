# VRVideoPlayer
When messing around with VR videos and the head mounted device in the face, i had trouble operate other players like VLC/MediaPlayer. So i just wrote some simple tool which does the basics i needed for testing. It plays propably everything you can play with your media player, as it uses the MediaPlayer element from the .NET framework.
- Change x/y postion of the video on the VR screen
- Change the width/height of the video on the VR screen
- Play/pause toggle
- Previous/next video
- Storage of tested values to simple persistent file
# Usage
- Build the solution
- Create a persistent file next to the executable. Example in the root of repo.
# Known issue / accepted crap
- Resizing the player in width/height doesnt apply to the video instant... reqires a pause/play toggle. I guess has something to do with the video encoder or at least the media player component, but i didnt care enought to figure it out
- The persistent file is the only way to get new stuff in, it needs to be placed next to the executable
- Next video is triggered with some dirty timer to workaround some threading issue with the player, i didnt care enought to do something better
