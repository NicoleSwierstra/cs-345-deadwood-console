/* Nicole Swierstra
 * Weird extensible game engine thing 
 *
 * idk tbh 
 */

class Application {

    /* in the future these could be dll files or something idk - dynamically loading the game would be kinda sick */
    Type GAME_TYPE = typeof(DeadwoodGame);
    Type UI_TYPE = typeof(DWConsoleUI);
    
    public enum Commands {
        ID_QUIT = 0x0,
        ID_ADD_PLAYER,
        ID_START,
    }
    
    IGameInstance game_backend;
    UIThread ui_thread;

    CommandQueue ui_queue, application_queue;

    /* players is a string array divorced from the idea of a player. 
       This allows for repeat plays with the same player names */
    List<string> players = [];


    public static void Main(string[] args) {
        UIPrompt.Test(args);
        //new Application().Run();
    }

    void Run() {
        ui_queue = new CommandQueue();
        application_queue = new CommandQueue();
        ui_thread = new UIThread(UI_TYPE, ui_queue, application_queue).Start();

        application_queue.push((int)Commands.ID_START, []);
        bool running = true;
        while (running) {
            if (!application_queue.empty()){
                int id = application_queue.pop(out int[] args);
                switch ((Commands)id) {
                case Commands.ID_QUIT:
                    ui_thread.Stop();
                    ui_thread.Join();
                    running = false; 
                    break;
                case Commands.ID_ADD_PLAYER: 
                    players.Add(CommandQueue.unpackString(args));
                    break;
                case Commands.ID_START:
                    game_backend = (IGameInstance)Activator.CreateInstance(GAME_TYPE);
                    game_backend.Setup(players.ToArray(), ui_queue);
                    break;
                default:
                    if (game_backend != null)
                        game_backend.ProcessCommand((int)id, args);
                    break;
                }
            }
        }
    }
}