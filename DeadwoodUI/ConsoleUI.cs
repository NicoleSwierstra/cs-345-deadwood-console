/*  Nicole Swierstra
 *  Console UI class
 *  implements the deadwood console UI
 *  
 *  All of the UI is in just a small handful of classes. Uh this might have been a bad idea tbh.
 */

struct PlayerNode {
    string Name;
    int color;
}

class DWConsoleUI : IGameUI {
    enum SelectorType {
        NONE_TYPE = -1,
        DELETE_PLAYER,
        MOVE_TYPE,
        TAKE_TYPE,
    }

    enum PromptType {
        NONE_TYPE = -1,
        MAIN_MENU,
        ADD_PLAYER,
        PLAYER_NAME,
        GAME_COMMAND
    }
    
    CommandQueue applicationQueue;
    ConsoleBoard cb;
    
    SelectorType selectorType;
    PromptType promptType;
    UISelector current_selector;
    UIPrompt current_prompt;
    List<string> current_players;

    bool should_end;

    public void End() {
        Console.WriteLine("Thank you for playing!");
    }

    void processPrompt(string prompt) {
        if (promptType == PromptType.MAIN_MENU) {
            switch (prompt.ToLower()) {
            case "add":
                promptType = PromptType.PLAYER_NAME;
                current_prompt = UIPrompt.fromMsg("Enter player's name:");
                break;
            case "remove":
                promptType = PromptType.NONE_TYPE;
                selectorType = SelectorType.DELETE_PLAYER;
                current_selector = UISelector.fromList(current_players, "Choose a player to remove:");
                break;
            case "start":
                foreach (string s in current_players) {
                    applicationQueue.push((int)Application.Commands.ID_ADD_PLAYER, CommandQueue.packString(s));
                }
                applicationQueue.push((int)Application.Commands.ID_START, []);
            case "exit":
                break;
            }
        }
    }

    void processSelection(string selection) {
        
    }

    public void OnUpdate() {
        if (!Console.KeyAvailable) return;
        ConsoleKeyInfo k = Console.ReadKey();


        if (selectorType == SelectorType.NONE_TYPE) {
            if (Console.KeyAvailable) {
                current_prompt.update(k.Key, k.KeyChar);
            }
            if (current_prompt.hasPrompt()) {
                
            }
            return;
        }
        if (Console.KeyAvailable) {
            current_selector.update(k.Key);
        }
    }

    public void ProcessCommand(int cmd_id, int[] args) {
    
    }

    public void Setup(CommandQueue applicationQueue) {
        cb = ConsoleBoard.fromXML("res/gamedata/board.xml");
        selectorType = SelectorType.NONE_TYPE;
    }

    public bool ShouldEnd()
    {
        return should_end;
    }
}