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
        GAME_COMMAND
    }

    enum Commands {
        INVALID_INPUT = 0x20,
        PLAYER_TURN,
        REVEAL_NEIGHBORS,
        END_GAME,
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

    void showMainMenu() {
        selectorType = SelectorType.NONE_TYPE;
        current_selector = null;
        
        promptType = PromptType.MAIN_MENU;
        string message = "Welcome to the console version of Deadwood!\n\n\t(add) Player\n\t(remove) Player\n\t(start) Game\n\t(quit) Game.\n\nPlayers:\n";
        int i = 0;
        foreach (string p in current_players) {
            message += $"[{i}]: {p}\n";
            i++;
        }

        current_prompt = UIPrompt.fromMsg(message);
    }

    /* I 100% could have done this better - scuffed ass */
    void processPrompt(string prompt) {
        if (promptType == PromptType.MAIN_MENU) {
            switch (prompt.ToLower()) {
            case "a":
            case "add":
                promptType = PromptType.ADD_PLAYER;
                current_prompt = UIPrompt.fromMsg("Enter player's name:");
                break;
            case "rm":
            case "remove":
                promptType = PromptType.NONE_TYPE;
                selectorType = SelectorType.DELETE_PLAYER;
                current_selector = UISelector.fromList(current_players, "Choose a player to remove:");
                current_prompt = null;
                break;
            case "s":
            case "start":
                foreach (string s in current_players) {
                    applicationQueue.push((int)Application.Commands.ID_ADD_PLAYER, CommandQueue.packString(s));
                }
                applicationQueue.push((int)Application.Commands.ID_START, []);
                break;
            case "q":
            case "quit":
            case "exit":
                applicationQueue.push((int)Application.Commands.ID_QUIT, []);
                break;
            default:
                Console.WriteLine("\rInvalid Arg!");
                current_prompt.Clear();
            break;
            }
        } else if (promptType == PromptType.ADD_PLAYER) {
            current_players.Add(prompt);
            showMainMenu();
        } else if (promptType == PromptType.GAME_COMMAND) {
            switch (prompt.ToLower()) {
                default:
                    throw new NotImplementedException();
            }
        } else {
            throw new NotImplementedException();
        }
    }

    void processSelection(int selection) {
        if (selectorType == SelectorType.DELETE_PLAYER) {
            current_players.RemoveAt(selection);
            showMainMenu();
        } else if (selectorType == SelectorType.MOVE_TYPE) {
            throw new NotImplementedException();
        } else if (selectorType == SelectorType.TAKE_TYPE) {
            throw new NotImplementedException();
        }
    }

    public void OnUpdate() {
        if (selectorType == SelectorType.NONE_TYPE) {
            if (Console.KeyAvailable) {
                ConsoleKeyInfo k = Console.ReadKey(true);
                current_prompt.update(k);
                if (current_prompt.hasPrompt()) {
                    processPrompt(current_prompt.getPrompt());
                }
            }
            return;
        }
        if (Console.KeyAvailable) {
            current_selector.update(Console.ReadKey(true));
            if (current_selector.hasSelected()) {
                processSelection(current_selector.getSelection());
            }
        }
    }

    public void ProcessCommand(int cmd_id, int[] args) {
    
    }

    public void Setup(CommandQueue applicationQueue) {
        cb = ConsoleBoard.fromXML("res/gamedata/board.xml");
        current_players = [];
        this.applicationQueue = applicationQueue;
        showMainMenu();
    }

    public bool ShouldEnd()
    {
        return should_end;
    }
}