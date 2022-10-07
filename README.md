# DarkRift2SingleEditorTemplate
A minimal project template that shows how to use and launch a DarkRift2 client and server in a single Unity-Editor (without parallel sync). Also added some build pipeline ideas that further improve the developer experience and boost productivity. 

The project is intended as a template for 3D multiplayer games with Unity and DarkRift2 with the aim for a maximum in productivity and comfort when it comes to developing multiplayer games. Managing server and client builds can often be a pain in the a** and not beeing able to run the server and the client in the same project slows down your development a lot and can be extremly frustrating. 

### Features

Some of the code and the bascic principals behind this template where taken form this awesome tutorial https://github.com/LukeStampfli/EmbeddedFPSExample which is a really great learning resource for DarkRift2 and multiplayer games in general. The biggest differnce to this tutorial is, that this project is only using a single server instead of a server that has multiple rooms which the client can connect to.

  - Server that manages player connections and logins
  - Client that is able to login and "play"
  - Multiplayer first person character controller (Walk, Look, Jump, Run)
  - Server authoritive movement with client side prediction and reconciliation
  - Buildscripts (currently only for Windows) that easily let you build and run the server or the client
  - Command line arguments that allow the client to automatically login with a given username 
  - `SinglePlayerServer`, a component that lets you run server and client simultaneously in the Unity-Editor

### Dependencies

I used some assets from the Unity Asset Store for this project which are not included in the project and need to be installed manually after you checked out the project:

  - DarkRift2 (https://assetstore.unity.com/packages/tools/network/darkrift-networking-2-95309)
  - FreeFlyCam (https://assetstore.unity.com/packages/tools/camera/free-fly-camera-140739)

FreeFlyCam is not mandatory and is only used by the camera of the server. If you don't want to download or use it, just remove the script from the camera GameObject in the "Server" scene. 

### Project Layout

The projects layout is pretty minimalistic and should be self explanatory. There are three scenes "Login", "Client" and "Server". The "Login" scene is the entrypoint for the client and switches automatically to the "Client" scene after a successfull login. The "Server" scene is the entrypoint of the server project. If in need for a desciption of some of the components in the scenes or in general, please visit the above mentioned tutorial as it provides a great explanation of everything going on. 

### Buildscripts

In the file `Assets/Scripts/Editor/Builds.cs` you can find a variety of build scripts which allow you to build and/or run the client or server via a "Builds" menu item in the editor, without the need for the build screen of unity. Currently all those scripts target the windows plattform but it is very easy to change this to run it on your desired plattform. Also the builds have currently no `BuildOptions`, which also can be very helpfull for development.

When running the client via this menu option, the buildscript passes a randomly generated username to the client via command line. The client then parses this name and automatically logs you in with this username so you don't have to type a name and press the login button every time you launch a client.

### SinglePlayerServer

The `SinglePlayerServer` which can be added the "Server" scene, is the component which allows you to run the server and the client simultaneously in the same editor. But in fact it is actually not this component that does the magic, instead multiple pieces across the project the enable it. The `ServerInstance` component is creating its own physics scene which hosts its environment and all the players. So when you start a client in parallel to the server, the actors from both scenes don't interfere with each other as they are just visually in the same scene but not physically. When enabled the `SinglePlayerServer` component then handles the launch of the login and then automatically sets a username and logs you in and afterwards switches to the client scene and et voila, you got a server and a client running at the same time in the unity editor. Currently there is a small downside to this when running client and server in parallel, the camera of the client renders both scenes which looks a bit weird as the player is not always at the same postion for the server and the client. More about this in the ToDo section. 

### How to move on from here

As already mentioned above, some of the code is taken from here https://github.com/LukeStampfli/EmbeddedFPSExample, but the tutorial is not completly finished in this repository. The last part of the tutorial, shooting with lag compensation, isn't implemented and waits for you to be done!

Feel free to use this template to start a new project or use it as a learning resource for DarkRift2, using build scripts in Unity or muliplayer games in general. 

### To Dos

One thing that bothers me and I want to change for sure is, that when using the `SingePlayerServer` the camera of the player renders both, the clients scene and the server scene. For the sake of this demo project I could have just set the layers of every object on the server scene to a specific layer and then set the clients camera to not render this layer. This approach isn't suitable for bigger projects as you may want to use layers for other stuff and you would need some complex logic for setting this layers, as some of the assets are used for both projects, the server and the client. I haven't found a good way yet to solve this and if someone has a good idea please contact me or create an issue.

### Future Plans 

As of now, I consider this project finished (aside from the camera thing mentioned above) but I may pick it up at some point and add some of the stuff that I implement for the games which I started with this template which may be:

  - Shooting with lag compensation 
  - Third person mode
  - Advanced character controller with climbing and vaulting 
  - Ingame console for logging and commands 
