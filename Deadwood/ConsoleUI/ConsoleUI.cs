/*  Nicole Swierstra
 *  Console UI class
 *  implements the deadwood console UI
 *  
 *  All of the UI is in just a small handful of classes. Uh this might have been a bad idea tbh, I will fix this.
 */

namespace Deadwood;

struct PlayerNode {
    public string Name;
    public int color;
    public bool remote;
    
    public PlayerNode(string name, int col, bool rem) {
        Name = name; 
        color = col;
        remote = rem;
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

    CommandQueue applicationQueue;
    ConsoleBoard cb;
    
    SelectorType selectorType;
    PromptType promptType;
    UISelector current_selector;
    UIPrompt current_prompt;
    List<PlayerNode> all_players;
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
        foreach (PlayerNode p in all_players) {
            message += $"\x1b[38;5;{PLAYER_COLORS[p.color]}m[{i}]\x1b[0m: {p.Name}\n";
            i++;
        }

        current_prompt = UIPrompt.fromMsg(message);
    }

    void showPlayerChoice(string preamble) {
        current_selector = null;
        selectorType = SelectorType.NONE_TYPE;

        promptType = PromptType.GAME_COMMAND;
        PlayerNode p = all_players[active_player];
        string message = preamble + $"{p}'s turn.\n\n\t[move] spaces\n\t[take] role\n\t[upgrade] player\n\t[rehearse] role\n\t[act] in role\n";
        current_prompt = UIPrompt.fromMsg(message);
    }

    void showPlayerMove(int[] args) {
        string message = $"{all_players[active_player]} is on tile: {cb.getTileName(args[0])}";
        List<string> tiles = ["Cancel"];
        List<int> positions = [-1];
        for(int i = 1; i < args.Length; i++) {
            tiles.Add($"{cb.getTileName(args[i])}");
            positions.Add(args[i]);
        }
        current_selector = UISelector.fromList(tiles, positions, message);
    }

    void appendPlayer(string name) {
        int first_availible_color;
        /* TODO: let players choose their colors */
        for (first_availible_color = 0; first_availible_color < 8; first_availible_color++) {
            if (all_players.Any(x => x.color == first_availible_color)) 
                continue;
            else break;
        }

        all_players.Add(new PlayerNode(name, first_availible_color, false));
    }

    /* I 100% could have done this better - scuffed ass implementation */
    void processPrompt(string prompt) {
        if (promptType == PromptType.MAIN_MENU) {
            switch (prompt.ToLower()) {
            case "a":
            case "add":
                if (all_players.Count == 8){
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
                current_selector = UISelector.fromList(all_players.Select(x => x.Name).ToList(), "Choose a player to remove:");
                current_prompt = null;
                break;
            case "s":
            case "start":
                if (all_players.Count < 2) {
                    Console.WriteLine("\rNeed 2 or more players to start.");
                    current_prompt.Clear();
                    return;
                }
                applicationQueue.push((int)Application.Commands.CLEAR_PLAYERS, []);
                foreach (PlayerNode p in all_players) {
                    applicationQueue.push((int)Application.Commands.ADD_PLAYER, CommandQueue.packString(p.Name));
                }
                applicationQueue.push((int)Application.Commands.START, []);
                break;
            case "q":
            case "quit":
            case "exit":
                applicationQueue.push((int)Application.Commands.QUIT, []);
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
                    applicationQueue.push((int)GameActions.TILEINFO, [active_player]);
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
            all_players.RemoveAt(selection);
            showMainMenu();
        } else if (selectorType == SelectorType.MOVE_TYPE) {
            Console.Write(selection);
            Thread.Sleep(1000);
            if (selection == -1) {
                showPlayerChoice("Move cancelled.\n");
                return;
            }
            applicationQueue.push((int)GameActions.MOVE, [active_player, selection]);
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
        switch((ClientCommands)cmd_id) {
            case (ClientCommands)UI_Commands.CMD_FAILURE:
                Console.WriteLine("InvalidInput. Idk what exactly but good luck.");
                return;
            case (ClientCommands)UI_Commands.CMD_SUCCESS:
                Console.WriteLine("Command executed successfully.");
                return;
            case ClientCommands.ADD_REMOTE_PLAYER:
            case ClientCommands.RM_REMOTE_PLAYER:
            case ClientCommands.UPDATE_CURRENCY:
            case ClientCommands.UPDATE_LOCATION:
                throw new NotImplementedException();
            case ClientCommands.PLAYER_TURN:
                active_player = args[0];
                showPlayerChoice("");
                break;
            case ClientCommands.REVEAL_NEIGHBORS:
                showPlayerMove(args);
                break; 
            case ClientCommands.REVEAL_CARD:
                throw new NotImplementedException();
            case ClientCommands.END_DAY:
                active_player = 0;
                showPlayerChoice(args[0] == 0 ? "The game has begun! Good Luck!\n\n" : "End of day " + args[0] + "\n\n");
                break;
            case ClientCommands.END_GAME:
                should_end = true;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Setup(CommandQueue applicationQueue) {
        cb = ConsoleBoard.fromXML("res/gamedata/board.xml");
        all_players = [];
        this.applicationQueue = applicationQueue;
        showMainMenu();
    }

    public bool ShouldEnd()
    {
        return should_end;
    }

    /* Console ui is never locked - no animations */
    public bool IsLocked()
    {
        return false;
    }
}