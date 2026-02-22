/* Nicole Swierstra
 * Weird extensible game engine thing 
 *
 * idk tbh 
 */

class Application {

    /* in the future these could be dll files or something idk - dynamically loading the game would be kinda sick */
    Type GAME_TYPE = typeof(Deadwood.DeadwoodGame);
    Type UI_TYPE = typeof(Deadwood.DWConsoleUI);
    
    public enum Commands {
        QUIT = 0x0,
        ADD_PLAYER,
        REMOVE_PLAYER,
        CLEAR_PLAYERS,
        START,
    }
    
    IGameInstance game_backend;
    UIThread ui_thread;

    CommandQueue ui_queue, application_queue;

    /* players is a string array divorced from the idea of a player. 
       This allows for repeat plays with the same player names */
    List<string> players = [];


    public static void Main(string[] args) {
        //UIPrompt.Test(args);
        new Application().Run();
    }

    void Run() {
        ui_queue = new CommandQueue();
        application_queue = new CommandQueue();
        ui_thread = new UIThread(UI_TYPE, ui_queue, application_queue).Start();

        bool running = true;
        while (running) {
            if (!application_queue.empty()){
                int id = application_queue.pop(out int[] args);
                switch ((Commands)id) {
                case Commands.QUIT:
                    ui_thread.Stop();
                    ui_thread.Join();
                    running = false; 
                    break;
                case Commands.ADD_PLAYER:
                    string p_name = CommandQueue.unpackString(args);
                    players.Add(p_name);
                    break;
                case Commands.CLEAR_PLAYERS:
                    players.Clear();
                    break;
                case Commands.START:
                    game_backend = (IGameInstance)Activator.CreateInstance(GAME_TYPE);
                    game_backend.Setup(players.ToArray(), ui_queue);
                    break;
                default:
                    if (game_backend != null) {
                        GameComRet r = game_backend.ProcessCommand((int)id, args);
                        if (r == GameComRet.RET_ERROR)
                            ui_queue.push((int)UI_Commands.CMD_FAILURE, []);
                        else if (r == GameComRet.RET_SUCCESS)
                            ui_queue.push((int)UI_Commands.CMD_SUCCESS, []);
                        else if (r == GameComRet.RET_ENDED) {
                            //ui_queue.push((int)UI_Commands.) IDK TBH
                        }
                    }
                    break;
                }
            }
        }
    }
}