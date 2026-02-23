## Deadwood.NET console edition ##

### Compiling ###

This will require the dotnet runtime to compile. It can be obtained from microsoft's website []. This codebase was compiled with dotnet version 10.0.102. I purposefully avoided using features like "global using" to try and make this code compatible with a wide variety of programs, but if you can't compile it let me know (I can be found @ swiersw@wwu.edu).

This ui also requires ANSI escape sequences turned on. If they are not turned on, the output will look a bit strange as it is constantly clearing the screen and setting the output to different colors.

### Playing the game ###

Type "dotnet run" to start the game. From here, you can add and remove players by typing "add" until you wish to start by typing "start". Most menus in the game work like this, with a list of options given at the top. If the menu is numbered, you can use your arrow keys or type digits to select an option, and then press enter to select.

### Architecture ###

The architecture of the program is really more designed to have a persistant UI than anything, so it's a bit clunky to play via the console. 

There are two threads, the application thread, and the ui thread. These threads are completely seperate, spare for two queue objects that link them together. The queues pass commands, as defined in the file "deadwood defines", along with their parameters. This mostly follows the "MVC" style, except that it is more of a "MV", with the controller just being constructed from the commands passed back and forth.

There are two real motivators that made me design the overall engine in the way that it is designed:

    1. Wanting different games with different rulesets to be able to be run inside of the same engine. (honestly more of an ego thing and really less important.)
    2. Able to be played through different means, like a console, a ui, or even as a fully remote player on a different device.

As consequence, nothing executing commands actually knows what sent it the commands, and neither the application thread nor ui thread actually know the contents of what they are running. There is basically 0 coupling throught the entire program, and everything is being passed as loosely as possible. In theory this is a good thing for extensibility.

Therefore, almost everything is passed via handle rather than by reference. For some reason, c# only recently added an equivalent of c's typedef, so there are a lot of random integers that represent handles, as I wasn't comfortable using a feature that was added a couple months ago. 

There are tradeoffs to developing this way. To look at the solid design principles:
    SRP: the deadwood game itself has most of the functionality of the program, but it wouldn't really make sense to break a lot of this functionality out. The rest is a mixed bag.
    OCP: This is where the engine really shines. There is no modification you need to do to add a new kind of UI, for example, and there is also very little modification that needs to be done to add networking. 
    LCP: because everything is as little referenced as possible, I think this works on a technicality.
    DIP: better than most I think.
    ISP: sure, I think.

To maintain parity between the instances data is only serialized in one place. The definition for how much an upgrade costs is in the deadwood class. Yes, this should be parsed in real time, but if I personally was doing it I'd move the upgrade definitions into a new xml file. Having the board also be responsible for checking upgrades seems a bit odd. Although I can imagine in a more traditional object oriented implementation, made under different constraints and with different motivating factors, the tile would be an abstract class and the behavior of that tile would be implemented inside of the tile. This would be a bit of a nightmare scenerio for networking tho.


\- Nicole