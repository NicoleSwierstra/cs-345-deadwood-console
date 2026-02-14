/* idk tbh */

class Application {
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

    Type game_type = typeof(DeadwoodGame);

    public static void Main(string[] args) {
        new Application().Run();
    }

    void Run() {
        ui_queue = new CommandQueue();
        application_queue = new CommandQueue();
        ui_thread = new UIThread(typeof(DWConsoleUI), ui_queue, application_queue).Start();

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
                    game_backend = (IGameInstance)Activator.CreateInstance(game_type);
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