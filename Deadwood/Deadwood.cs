/* Nicole Swierstra, Daniil Bolgar
 * Deadwood Game
 */

class DeadwoodGame : IGameInstance {
    public enum Actions {
        ID_MOVE = 0x10, ID_TAKE, ID_UPGRADE, ID_REHEARSE, ID_ACT, ID_FORCE_END
    };

    private CommandQueue ui_queue;
    private int active_player;
    private Player[] players; //2-8 players
    private Board board;
    private Deck deck;

    private int current_day;

    /* ===== game consts ===== */
    /* TODO: these are stored in the xml for the board for some reason. Parse those Ig */
    readonly int[] rank_cost_dollars = [
        0,
        4,
        10,
        18,
        28,
        40
    ];

    readonly int[] rank_cost_credits = {
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
        /* disallowing any funny buisness with some basic sanitization */
       
        switch ((Actions)cmd_id) {
        case Actions.ID_MOVE:
            return processMove(args);
        case Actions.ID_TAKE:
            return processMove(args);
        case Actions.ID_UPGRADE:
            return processMove(args);
        case Actions.ID_ACT:
            return processMove(args);
        case Actions.ID_REHEARSE:
            return processMove(args);
        case Actions.ID_FORCE_END:
            End();
            return GameComRet.RET_ENDED;
        default:
            return pass(cmd_id, args);
        }
    }

    public void End() {
    }

    public void Setup(string[] players, CommandQueue ui_queue) {
        this.ui_queue = ui_queue;
        this.players = new Player[players.Length];
        for(int i = 0; i < players.Length; i++) {
            this.players[i] = new Player(players[i]);
        }

        deck = Deck.fromXML("res/gamedata/cards.xml").shuffled(); /* shuffled only once at the beginning of the game */
        board = Board.fromXML("res/gamedata/board.xml");

        endDay();
    }

    // Game logic methods
    private GameComRet processMove(int[] args) {
        int player_id = args[0], new_location = args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;
        int[] adj = board.getAdjacent(players[player_id].getLocation());
        
        for (int i = 0; i < adj.Length; i++) 
            if (adj[i] == new_location) {
                players[player_id].move(new_location);
                return GameComRet.RET_SUCCESS;
            }

        return GameComRet.RET_ERROR;
    }

    private GameComRet processTake(int[] args) {
        int player_id = args[0], role_to_take = args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;

        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processUpgrade(int[] args) {
        int player_id = args[0], rank_num = args[2];
        Player.UpgradeType type = (Player.UpgradeType)args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;
        if (!board.isOffice(players[player_id].getLocation()))
            return GameComRet.RET_ERROR;

        int cost = rank_cost_credits[rank_num];
        if (type == Player.UpgradeType.DOLLARS) 
            cost = rank_cost_dollars[rank_num];

        if (!players[player_id].upgrade(rank_num, type, cost))
            return GameComRet.RET_ERROR;

        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processRehearse(int[] args) {
        int player_id = args[0];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;

        return GameComRet.RET_SUCCESS;
    }

    private GameComRet processAct(int[] args) {
        int player_id = args[0], dice_roll = args[1];
        return GameComRet.RET_SUCCESS;
    }

    private void endTurn() {
        players[active_player].endTurn();
        active_player++;
        active_player %= players.Length;
        players[active_player].reset();
    }

    private void endDay() {
        foreach (Player p in players) {
            p.reset();
            p.move(board.getTrailer());
            p.reset();
        }

        // TODO: deal cards
        for (int i = 0; i < 10; i++) {
            
        }
    }
}