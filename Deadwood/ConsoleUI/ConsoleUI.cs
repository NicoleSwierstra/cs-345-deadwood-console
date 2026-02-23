/*  Nicole Swierstra
 *  Console UI class
 *  
 * implements the deadwood console UI. 
 * All of the UI is in just a small handful of classes. Is it spagetti? yeah, I got kinda sick and ran out of energy to figure out some sophisticated console UI.
 * I thought I might have time to implement networking, so that's why the playernode has a boolean for being a "remote" player.
 * 
 * There is much more of a procedural and functional implementation of this code than should be submitted for an OOP class. Sorry. 
 */

namespace Deadwood;

class PlayerNode {
    public string Name;
    public int color;
    public bool remote;

    /* data */
    bool in_role;
    int location = 10;

    public PlayerNode(string name, int col, bool rem) {
        Name = name; 
        color = col;
        remote = rem;
        in_role = false;
    }

    public override string ToString()
    {
        return $"\x1b[38;5;{DWConsoleUI.PLAYER_COLORS[color]}m{Name}\x1b[0m";
    }

    public void SetRole(int r) {
        this.in_role = r != -1; 
    }

    public bool inRole() { 
        return this.in_role;
    }

    /* bodged in so last minute lmao */
    public int getLocation() {
        return location;
    }

    public void setLocation(int n_loc) {
        location = n_loc;
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
        UPGRADE_TYPE,
    }

    enum PromptType {
        NONE_TYPE = -1,
        MAIN_MENU,
        ADD_PLAYER,
        GAME_COMMAND,
        UPGRADE,
        END_DAY,
    }

    CommandQueue applicationQueue;
    ConsoleBoard cb;
    ConsoleDeck cd;
    
    SelectorType selectorType;
    PromptType promptType;
    UISelector current_selector;
    UIPrompt current_prompt;
    List<PlayerNode> all_players;
    int active_player;

    bool should_end;

    const float DICE_TIME_MULTIPLIER = 300;
    const int DICE_ROLLS = 25;
    Random diceRand = new Random(unchecked(((int)DateTime.Now.Ticks) * 389));

    public void Setup(CommandQueue applicationQueue) {
        cb = ConsoleBoard.fromXML("res/gamedata/board.xml");
        cd = ConsoleDeck.fromXML("res/gamedata/cards.xml");
        all_players = [];
        this.applicationQueue = applicationQueue;
        showMainMenu();
    }

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
        string m = p.inRole() ? "\x1b[38;5;237m" : "\x1b[0m";
        string r = p.inRole() ? "\x1b[0m" : "\x1b[38;5;237m";
        string message = preamble + $"{p}'s turn.\n\n" +
            m + "\t[move] spaces\x1b[0m\n" + 
            m + "\t[take] role\x1b[0m\n" + 
            m + "\t[upgrade] player\x1b[0m\n" + 
            r + "\t[rehearse] role\x1b[0m\n" + 
            r + "\t[act] in role\x1b[0m\n" +
            "\t[end turn]\n\t[end game]\n\t[info] of all players\n";

        current_prompt = UIPrompt.fromMsg(message);
    }

    void showPlayerMove(int[] args) {
        string message = $"{all_players[active_player]} is on tile: {cb.getTileName(args[0])}";
        List<string> tiles = ["Cancel"];
        List<int> positions = [-1];
        for(int i = 1; i < args.Length; i++) {
            tiles.Add(cb.getTileName(args[i]) + ((cb.getTileCard(args[i]) == -1) ? "" : $" ({cd.getCardName(cb.getTileCard(args[i]))})"));
            positions.Add(args[i]);
        }
        current_selector = UISelector.fromList(tiles, positions, message);
    }

    void showPlayerTake(int[] args) {
        if (args[1] == -1) {
            showPlayerChoice($"There is no set for the {cb.getTileName(args[0])} tile.\n");
            return;
        }
        string message = $"{all_players[active_player]} is on tile: {cb.getTileName(args[0])}, with card: {cd.getCardName(args[1])}";
        List<Role> roles_to_add = [.. cb.GetRoles(args[0])];
        roles_to_add.AddRange(cd.getRoles(args[1]));
        List<string> rolesstr = ["Cancel"];
        List<int> r_ids = [-1];
        for(int i = 0; i < roles_to_add.Count; i++) {
            r_ids.Add(i);
            rolesstr.Add($"({roles_to_add[i].rank}) {roles_to_add[i].name}: {roles_to_add[i].line}");
        }
        current_selector = UISelector.fromList(rolesstr, r_ids, message);
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
            PlayerNode player = all_players[active_player];
            switch (prompt.ToLower()) {
                case "move":
                    if(player.inRole()) {
                        Console.WriteLine("\rCan't move when in role!");
                        current_prompt.Clear();
                        return;
                    }
                    selectorType = SelectorType.MOVE_TYPE;
                    applicationQueue.push((int)GameActions.TILE_INFO, [active_player]);
                    break;
                case "take":
                    if(player.inRole()) {
                        Console.WriteLine("\rCan't take when in role!");
                        current_prompt.Clear();
                        return;
                    }
                    selectorType = SelectorType.TAKE_TYPE;
                    applicationQueue.push((int)GameActions.CARD_INFO, [active_player]);
                    break;
                case "upgrade":
                    if(player.inRole()) {
                        Console.WriteLine("\rCan't upgrade when in role!");
                        current_prompt.Clear();
                        return;
                    }
                    selectorType = SelectorType.UPGRADE_TYPE;
                    current_selector = UISelector.fromList(
                        [
                            "Cancel",
                            "level 2, $4",
                            "level 3, $10",
                            "level 4, $18",
                            "level 5, $28",
                            "level 6, $40",
                            "level 2, 5c",
                            "level 3, 10c",
                            "level 4, 15c",
                            "level 5, 20c",
                            "level 6, 25c",
                        ], "Choose your upgrade:");
                    break;
                case "rehearse":
                    if(!player.inRole()) {
                        Console.WriteLine("\rCan't rehearse when not in role!");
                        current_prompt.Clear();
                        return;
                    }
                    applicationQueue.push((int)GameActions.REHEARSE, [active_player]);
                    break;
                case "act":
                    if(!player.inRole()) {
                        Console.WriteLine("\rCan't act when not in role!");
                        current_prompt.Clear();
                        return;
                    }
                    int roll = diceRand.Next() % 6;
                    roll += DICE_ROLLS;
                    for(float i = 0; i < DICE_ROLLS; i ++) {
                        /* this shouldn't be blocking tbh, kinda an L on my part */
                        float t = (float)i / (float)DICE_ROLLS; 
                        roll--;
                        Console.Write("\x1b[0G" + (roll % 6 + 1));
                        Thread.Sleep((int)(t * t * DICE_TIME_MULTIPLIER));
                    }
                    applicationQueue.push((int)GameActions.ACT, [active_player, roll + 1]);
                    break;
                case "end turn":
                    applicationQueue.push((int)GameActions.END_TURN, [active_player]);
                    break;
                case "end game":
                    applicationQueue.push((int)GameActions.FORCE_END, []);
                    break;
                case "info":
                    string infostr = "\n";
                    foreach(PlayerNode pn in all_players) {
                        infostr += (pn == all_players[active_player]) ? "* " : "- ";
                        infostr += $"{pn} is at {cb.getTileName(pn.getLocation())}\n";
                    }
                    Console.WriteLine(infostr);
                    current_prompt.Clear();
                    break;
                default:
                    Console.WriteLine("\rInvalid Arg!");
                    current_prompt.Clear();
                    break;
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
            if (selection == -1) {
                showPlayerChoice("Move cancelled.\n");
                return;
            }
            applicationQueue.push((int)GameActions.MOVE, [active_player, selection]);
        } else if (selectorType == SelectorType.TAKE_TYPE) {
            if (selection == -1) {
                showPlayerChoice("Take cancelled.\n");
                return;
            }
            applicationQueue.push((int)GameActions.TAKE, [active_player, selection]);
        } else if (selectorType == SelectorType.UPGRADE_TYPE) {
            int s = selection - 1;
            if (s == -1) {
                showPlayerChoice("Upgrade cancelled.\n");
                return;   
            }
            UpgradeType ut = (s < 6) ? UpgradeType.DOLLARS : UpgradeType.CREDITS;
            applicationQueue.push((int)GameActions.UPGRADE, [active_player, (int)ut, (s % 5) + 2]);
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
        string updateStr = "";
        switch((ClientCommands)cmd_id) {
            case (ClientCommands)UI_Commands.CMD_FAILURE:
                showPlayerChoice("InvalidInput. Idk what exactly because I am a terrible user interface.\n");
                return;
            case (ClientCommands)UI_Commands.CMD_SUCCESS:
                //showPlayerChoice("Command Successful!\n");
                return;
            /* Networking is not implemented */
            case ClientCommands.ADD_REMOTE_PLAYER:
            break;
            case ClientCommands.RM_REMOTE_PLAYER:
            break;
            case ClientCommands.UPDATE_CURRENCY:
                int player  = args[0];
                int dollars = args[1];
                int credits = args[2];
                int r_tok   = args[3];
                int rank    = args[4];
                DataChangeReason reason  = (DataChangeReason)args[5];
                updateStr = $"{all_players[player]} ({rank}) now has ${dollars}, {credits}c, {r_tok} tokens\n";
                /* this would benefit from persistance but the console implementation was kinda lazy */
                if (reason == DataChangeReason.REHEARSAL) {
                    Console.WriteLine($"\n{all_players[player]} now has {r_tok} tokens.");
                    Thread.Sleep(1000);
                } else if (reason == DataChangeReason.ACT_SUCCESS) {
                    Console.WriteLine($"\nSuccess! {all_players[player]} now has ${dollars} and {credits} credits");
                    Thread.Sleep(1000);
                } else if (reason == DataChangeReason.ACT_FAILURE) {
                    Console.WriteLine($"\nFailure! {all_players[player]} now has ${dollars} and {credits} credits");
                    Thread.Sleep(1000);
                } else if (reason == DataChangeReason.ACT_WRAP) {
                    Console.WriteLine($"\n{all_players[player]} now has ${dollars} and {credits} credits");
                } else if (player == active_player) {
                    showPlayerChoice(updateStr); 
                } else
                    Console.WriteLine(updateStr);
                break;
            case ClientCommands.UPDATE_ROLE:
                all_players[args[0]].SetRole(args[2]);
                if (args[2] == -1 && args[0] == active_player){
                    Console.WriteLine($"{cd.getCardName(cb.getTileCard(args[1]))} has wrapped. {all_players[args[0]]} is now in {cb.getTileName(args[1])}.");
                    Thread.Sleep(3000);
                }
                break;
            case ClientCommands.UPDATE_LOCATION:
                all_players[args[0]].setLocation(args[1]);
                updateStr = "Player " + all_players[args[0]] + " has moved to " + cb.getTileName(args[1]) + "\n"; 
                if (args[0] == active_player)
                    showPlayerChoice(updateStr);
                else
                    Console.WriteLine(updateStr);
                break;
            case ClientCommands.PLAYER_TURN:
                active_player = args[0];
                showPlayerChoice("");
                break;
            case ClientCommands.REVEAL_NEIGHBORS:
                showPlayerMove(args);
                break; 
            case ClientCommands.REVEAL_CARD:
                cb.setTileCard(args[0], args[1]);
                showPlayerTake(args);
                break;
            case ClientCommands.END_DAY:
                cb.resetCards();
                active_player = 0;
                showPlayerChoice(args[0] == 0 ? "The game has begun! Good Luck!\n\n" : "End of day " + args[0] + "\n\n");
                break;
            case ClientCommands.END_GAME:
                for (int i = 0; i < args.Length; i++) {
                    Console.WriteLine($"{all_players[i]}: {args[i]} points");
                }

                should_end = true;
                break;
            default:
                throw new NotImplementedException();
        }
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