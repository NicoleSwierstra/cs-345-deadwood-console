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
       
        switch ((GameActions)cmd_id) {
        case GameActions.ID_MOVE:
            return processMove(args);
        case GameActions.ID_TAKE:
            return processTake(args);
        case GameActions.ID_UPGRADE:
            return processUpgrade(args);
        case GameActions.ID_ACT:
            return processAct(args);
        case GameActions.ID_REHEARSE:
            return processRehearse(args);
        case GameActions.ID_TILEINFO:
            return sendTileInfo(args);        
        case GameActions.ID_FORCE_END:
            End();
            return GameComRet.RET_ENDED;
        default:
            return GameComRet.RET_ERROR; /* pass(cmd_id, args); */
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
    private GameComRet sendTileInfo(int[] args) {
        List<int> data = [players[args[0]].getLocation()];
        foreach (int i in board.getAdjacent(data[0])) {
            data.Add(i);
        }

        ui_queue.push((int)ClientCommands.REVEAL_NEIGHBORS, data.ToArray());

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
            p.endTurn();
            p.move(board.getTrailer());
            p.endTurn();
        }

        // TODO: deal cards
        for (int i = 0; i < 10; i++) {
            
        }

        active_player = 0;
        ui_queue.push((int)ClientCommands.END_DAY, [current_day]);
        
        current_day++;
    }
}