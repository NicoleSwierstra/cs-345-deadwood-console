/*  Nicole Swierstra
 *  Console UI class
 *  implements the deadwood console UI
 *  
 *  All of the UI is in just a small handful of classes. Uh this might have been a bad idea tbh, I will fix this.
 */

/* TODO: change to this */
struct PlayerNode {
    public string Name;
    public int color;
    
    public PlayerNode(string name, int col) {
        Name = name; 
        color = col;
    }

    public override string ToString()
    {
        return $"\x1b[38;5;{DWConsoleUI.PLAYER_COLORS[color]}m{Name}\x1b[0m";
    }
}

class DWConsoleUI : IGameUI {
    /* curses console colors */
    public static readonly int[] PLAYER_COLORS = [1, 4, 3, 2, 5, 6, 202, 15];

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
        GAME_COMMAND,
        END_DAY,
    }

    public enum Commands {
        INVALID_INPUT = 0x20,
        PLAYER_TURN,
        REVEAL_NEIGHBORS,
        END_DAY,
        END_GAME,
    }

    CommandQueue applicationQueue;
    ConsoleBoard cb;
    
    SelectorType selectorType;
    PromptType promptType;
    UISelector current_selector;
    UIPrompt current_prompt;
    List<PlayerNode> current_players;
    int active_player;

    bool should_end;

    public void End() {
        Console.WriteLine("Thank you for playing!");
    }

    void showMainMenu() {
        selectorType = SelectorType.NONE_TYPE;
        current_selector = null;
        
        promptType = PromptType.MAIN_MENU;
        string message = "Welcome to the console version of Deadwood!\n\n\t[add] Player\n\t[remove] Player\n\t[start] Game\n\t[quit] Game.\n\nPlayers:\n";
        int i = 0;
        foreach (PlayerNode p in current_players) {
            message += $"\x1b[38;5;{PLAYER_COLORS[p.color]}m[{i}]\x1b[0m: {p.Name}\n";
            i++;
        }

        current_prompt = UIPrompt.fromMsg(message);
    }

    void showPlayerChoice(string preamble) {
        current_selector = null;
        selectorType = SelectorType.NONE_TYPE;

        promptType = PromptType.GAME_COMMAND;
        PlayerNode p = current_players[active_player];
        string message = preamble + $"{p}'s turn.\n\n\t[move] spaces\n\t[take] role\n\t[upgrade] player\n\t[rehearse] role\n\t[act] in role\n";
        current_prompt = UIPrompt.fromMsg(message);
    }

    void showPlayerMove(int[] args) {
        string message = $"{current_players[active_player]} is on tile: {cb.getTileName(args[0])}";
        List<string> tiles = [];
        for(int i = 1; i < args.Length; i++) {
            tiles.Add($"{cb.getTileName(args[i])}");
        }
        current_selector = UISelector.fromList(tiles, message);
    }

    void appendPlayer(string name) {
        int first_availible_color;
        for (first_availible_color = 0; first_availible_color < 8; first_availible_color++) {
            if (current_players.FindIndex(x => x.color == first_availible_color) == -1) 
                break;
        }

        current_players.Add(new PlayerNode(name, first_availible_color));
    }

    /* I 100% could have done this better - scuffed ass */
    void processPrompt(string prompt) {
        if (promptType == PromptType.MAIN_MENU) {
            switch (prompt.ToLower()) {
            case "a":
            case "add":
                if (current_players.Count == 8){
                    Console.WriteLine("Maximum # of players reached.");
                    current_prompt.Clear();
                    return; 
                }
                promptType = PromptType.ADD_PLAYER;
                current_prompt = UIPrompt.fromMsg("Enter player's name:");
                break;
            case "rm":
            case "remove":
                promptType = PromptType.NONE_TYPE;
                selectorType = SelectorType.DELETE_PLAYER;
                current_selector = UISelector.fromList(current_players.Select(x => x.Name).ToList(), "Choose a player to remove:");
                current_prompt = null;
                break;
            case "s":
            case "start":
                applicationQueue.push((int)Application.Commands.ID_CLEAR_PLAYERS, []);
                foreach (PlayerNode p in current_players) {
                    applicationQueue.push((int)Application.Commands.ID_ADD_PLAYER, CommandQueue.packString(p.Name));
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
            appendPlayer(prompt);
            showMainMenu();
        } else if (promptType == PromptType.GAME_COMMAND) {
            switch (prompt.ToLower()) {
                case "move":
                    selectorType = SelectorType.MOVE_TYPE;
                    applicationQueue.push((int)DeadwoodGame.Actions.ID_TILEINFO, [active_player]);
                    /* wait until it gets it back in commands */
                    break;
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
        switch((Commands)cmd_id) {
            case Commands.INVALID_INPUT:
                Console.WriteLine("InvalidInput. Idk what exactly but good luck.");
                return;
            case Commands.PLAYER_TURN:
                active_player = args[0];
                showPlayerChoice("");
                break;
            case Commands.REVEAL_NEIGHBORS:
                showPlayerMove(args);
                break; 
            case Commands.END_DAY:
                active_player = 0;
                showPlayerChoice(args[0] == 0 ? "The game has begun! Good Luck!\n\n" : "End of day " + args[0] + "\n\n");
                break;
        }
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