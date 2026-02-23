/* Nicole Swierstra, Daniil Bolgar
 * Deadwood Game
 */

namespace Deadwood;

class DeadwoodGame : IGameInstance {
    private CommandQueue ui_queue;
    private int active_player;
    private Player[] players; //2-8 players
    private Board board;
    private Deck deck;
    private int current_day;

    private int game_length;

    static Random rand = new Random(unchecked((int)DateTime.Now.Ticks * 2));
    private bool ended = false;

    /* ===== game consts ===== */
    /* TODO: these are stored in the xml for the board for some reason. Parse those Ig */
    readonly int[] rank_cost_dollars = [
        0,
        0,
        4,
        10,
        18,
        28,
        40
    ];

    readonly int[] rank_cost_credits = {
        0,
        0,
        5,
        10,
        15,
        20,
        25
    };

    /* blank constructor - must be implemented for  */
    public DeadwoodGame() {
        
    } 

    private GameComRet pass(int id, int[] args) {
        ui_queue.push(id, args); 
        return GameComRet.RET_SUCCESS; /* we ignore it if it's not for us */
    }

    public GameComRet ProcessCommand(int cmd_id, int[] args) {
        if (ended) return GameComRet.RET_ENDED;

        switch ((GameActions)cmd_id) {
        case GameActions.MOVE:
            return processMove(args);
        case GameActions.TAKE:
            return processTake(args);
        case GameActions.UPGRADE:
            return processUpgrade(args);
        case GameActions.ACT:
            return processAct(args);
        case GameActions.REHEARSE:
            return processRehearse(args);
        case GameActions.END_TURN:
            if (active_player == args[0]) {
                endTurn();
                return GameComRet.RET_SUCCESS;
            }
            return GameComRet.RET_ERROR;
        case GameActions.TILE_INFO:
            return sendTileInfo(args);
        case GameActions.CARD_INFO:
            return sendCardInfo(args);        
        case GameActions.FORCE_END:
            End();
            return GameComRet.RET_ENDED;
        default:
            return GameComRet.RET_ERROR; /* pass(cmd_id, args); */
        }
    }

    public void End() {
        ui_queue.push((int)ClientCommands.END_GAME, new List<Player>(players).Select(p => p.getScore()).ToArray());
        ended = true;
    }

    public void Setup(string[] players, CommandQueue ui_queue) {
        ended = false;
        this.ui_queue = ui_queue;
        this.players = new Player[players.Length];
        for(int i = 0; i < players.Length; i++) {
            this.players[i] = new Player(players[i]);
        }

        deck = Deck.fromXML("res/gamedata/cards.xml").shuffled(); /* shuffled only once at the beginning of the game */
        board = Board.fromXML("res/gamedata/board.xml");

        int num_players = players.Length;
        int starting_credits = 0;
        int starting_rank = 1;
        if (num_players == 2 || num_players == 3) {
            game_length = 3;
        } else if (num_players == 4) {
            game_length = 4;
        } else if (num_players == 5) {
            starting_credits = 2;
            game_length = 4;
        } else if (num_players == 6) {
            starting_credits = 4;
            game_length = 4;
        } else if (num_players == 7 || num_players == 8) {
            game_length = 4;
            starting_rank = 2;
        } else {
            throw new ArgumentOutOfRangeException("players", "Player count must be between 2 and 8.");
        }

        for (int i = 0; i < this.players.Length; i++) {
            for (int c = 0; c < starting_credits; c++) {
                this.players[i].incCredits();
            }
            if (starting_rank > 1) {
                this.players[i].upgrade(2, UpgradeType.DOLLARS, 0);
            }
        }

        endDay();
    }

    // Game logic methods
    private GameComRet processMove(int[] args) {
        int player_id = args[0], new_location = args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;
        int[] adj = board.getAdjacent(players[player_id].getLocation());

        if (players[player_id].getRole() != -1)
            return GameComRet.RET_ERROR;
        
        for (int i = 0; i < adj.Length; i++) 
            if (adj[i] == new_location) {
                if (players[player_id].move(new_location)) {
                    ui_queue.push((int)ClientCommands.UPDATE_LOCATION, [player_id, new_location]);
                    return GameComRet.RET_SUCCESS;
                }
            }

        return GameComRet.RET_ERROR;
    }

    //args[0] = player_id | args[1] = role_to_take
    private GameComRet processTake(int[] args) {
        int player_id = args[0], role_to_take= args[1];
        if (active_player != player_id) {
            return GameComRet.RET_ERROR;
        } 

        Player player = players[player_id];

        //player already has a role
        if (player.getRole() != -1) {   
            return GameComRet.RET_ERROR;
        }

        Tile tile = board.getTile(player.getLocation());
        if (tile.GetScene() == null) {
            return GameComRet.RET_ERROR;        
        }

        Role[] extras = tile.GetExtras();
        Role role;
        //role index offset (0 - extras.Length) = off-card, beyond that = on-card
        if (role_to_take < extras.Length) {
            role = extras[role_to_take];
        } else {
            role = tile.GetScene().getRoles()[role_to_take - extras.Length];
        }
        if (player.getRank() < role.rank) {
            return GameComRet.RET_ERROR;      
        }

        foreach(Player p in players) {
            if (p.getRole() == role_to_take && p.getLocation() == player.getLocation()) {
                return GameComRet.RET_ERROR;
            }
        }
        
        ui_queue.push((int)ClientCommands.UPDATE_ROLE, [player_id, player.getLocation(), role_to_take]);
        player.setRole(role_to_take);
        endTurn();
        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processUpgrade(int[] args) {
        int player_id = args[0], rank_num = args[2];
        UpgradeType type = (UpgradeType)args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;
        if (!board.isOffice(players[player_id].getLocation()))
            return GameComRet.RET_ERROR;

        int cost = rank_cost_credits[rank_num];
        if (type == UpgradeType.DOLLARS) 
            cost = rank_cost_dollars[rank_num];

        if (!players[player_id].upgrade(rank_num, type, cost))
            return GameComRet.RET_ERROR;

        Player p = players[player_id];
        ui_queue.push((int)ClientCommands.UPDATE_CURRENCY, [player_id, p.getDollars(), p.getCredits(), p.getTokens(), p.getRank(), (int)DataChangeReason.UPGRADE]);
        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processRehearse(int[] args) {
        int player_id = args[0];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;

        Player player = players[player_id];
        // no role check
        if (player.getRole() == -1) {
            return GameComRet.RET_ERROR;
        }

        // guaranteed success check
        int budget = board.getTile(player.getLocation()).GetScene().getBudget();
        if (player.getTokens() >= budget -1) {
            return GameComRet.RET_ERROR;    //must act
        }

        player.incTokens();
        
        Player p = players[player_id];
        ui_queue.push((int)ClientCommands.UPDATE_CURRENCY, [
            player_id, 
            p.getDollars(), 
            p.getCredits(), 
            p.getTokens(), 
            p.getRank(), 
            (int)DataChangeReason.REHEARSAL
        ]);
        endTurn();
        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processAct(int[] args) {
        int player_id = args[0], dice_roll = args[1];
        if (active_player != player_id) {
            return GameComRet.RET_ERROR;
        }
        Player player = players[player_id];
        if (player.getRole() == -1) {
            return GameComRet.RET_ERROR;
        }

        int roll = dice_roll + player.getTokens();
        Tile tile = board.getTile(player.getLocation());
        SceneCard scene = tile.GetScene();
        Player p = players[player_id];

        //off-card/on-card check
        bool on_card = true;
        if (player.getRole() < tile.GetExtras().Length) { //off-card
            on_card = false;
        }
        if (roll >= scene.getBudget()) { //success
            tile.shots_remaining--;
            if (on_card) {
                player.incCredits();
                player.incCredits();
            } else {                //off-card success
                player.incCredits();
                player.incDollars(1);
            }
            ui_queue.push((int)ClientCommands.UPDATE_CURRENCY, [player_id, p.getDollars(), p.getCredits(), p.getTokens(), p.getRank(), (int)DataChangeReason.ACT_SUCCESS]);
            if (tile.shots_remaining == 0) {
                wrapScene(tile);
            }
        } else {
            if (!on_card) {
                player.incDollars(1);
            }
            ui_queue.push((int)ClientCommands.UPDATE_CURRENCY, [player_id, p.getDollars(), p.getCredits(), p.getTokens(), p.getRank(), (int)DataChangeReason.ACT_FAILURE]);
        }

        endTurn();
        return GameComRet.RET_SUCCESS;
    }
    

    private void wrapScene(Tile tile) {
        SceneCard scene = tile.GetScene();
        Role[] extras = tile.GetExtras();
        Role[] onCardRoles = scene.getRoles();
        bool hasOnCard = false;

        //check on-card players
        foreach (Player p in players) {
            if (p.getLocation() == tile.location && p.getRole() >= extras.Length && p.getRole() != -1) {
                hasOnCard = true;
                break;
            }
        }
        if (hasOnCard) {
            //roll dice and sort descending order
            int[] dice = new int[scene.getBudget()];
            for (int i = 0; i < dice.Length; i++) {
                dice[i] = rand.Next(1, 7);
            }
            Array.Sort(dice);
            Array.Reverse(dice);

            int[] payouts = new int[onCardRoles.Length];
            for (int i = 0; i < dice.Length; i++) {
                payouts[i % onCardRoles.Length] += dice[i];
            }

            //on-card players
            foreach(Player p in players) {
                //on-card offset
                if (p.getLocation() == tile.location && p.getRole() >= extras.Length && p.getRole() != -1) {
                    p.incDollars(payouts[p.getRole() - extras.Length]);
                }
            }
            foreach(Player p in players) {
                //off-card offset
                if (p.getLocation() == tile.location && p.getRole() >= 0 && p.getRole() < extras.Length) {
                    p.incDollars(extras[p.getRole()].rank);
                }
            }
        }
        //clear roles
        foreach(Player p in players) {
            if (p.getLocation() == tile.location) {
                p.setRole(-1);
                ui_queue.push((int)ClientCommands.UPDATE_CURRENCY, [players.IndexOf(p), p.getDollars(), p.getCredits(), p.getTokens(), p.getRank(), (int)DataChangeReason.ACT_WRAP]);
                ui_queue.push((int)ClientCommands.UPDATE_ROLE, [players.IndexOf(p), p.getLocation(), -1]);
            }
        }
        //remove scene
        tile.SetScene(null);

        //check for endDay
        int activeScenes = 0;
        foreach(Tile t in board.getTiles()) {
            if (t.GetScene() != null) {
                activeScenes++;
            }
        }
        if(activeScenes <= 1) {
            endDay();
        }
    }

    private GameComRet sendTileInfo(int[] args) {
        List<int> data = [players[args[0]].getLocation()];
        foreach (int i in board.getAdjacent(data[0])) {
            data.Add(i);
        }

        ui_queue.push((int)ClientCommands.REVEAL_NEIGHBORS, data.ToArray());

        return GameComRet.RET_SUCCESS;
    }

    private GameComRet sendCardInfo(int[] args) {
        int location = players[args[0]].getLocation();
        if (!board.getTile(location).isSet())
            ui_queue.push((int)ClientCommands.REVEAL_CARD, [location, -1]);
        else
            ui_queue.push((int)ClientCommands.REVEAL_CARD, [location, deck.idOf(board.getActiveScene(location))] );

        return GameComRet.RET_SUCCESS;
    }

    private void endTurn() {
        players[active_player].endTurn();
        active_player++;
        active_player %= players.Length;
        ui_queue.push((int)ClientCommands.PLAYER_TURN, [active_player]);
    }

    private void endDay() {
        foreach (Player p in players) {
            p.setLocation(board.getTrailer());
            p.resetForDay();
        }

        foreach (Tile t in board.getTiles()) {
            t.Reset();
            if (t.isSet()) {   //is set
                t.SetScene(deck.dealTop());
            } 
        }

        active_player = 0;
        ui_queue.push((int)ClientCommands.END_DAY, [current_day]);

        current_day++;
        if (current_day > game_length) {
            End();
        } 
    }
}