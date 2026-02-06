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
        throw new NotImplementedException();
    }

    // Game logic methods
    private GameComRet processMove(int[] args) {
        int player_id = args[0], new_location = args[1];
        if (active_player != player_id) 
            return GameComRet.RET_ERROR;

        return GameComRet.RET_SUCCESS;
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
    }

    private void endDay()
    {

    }
}