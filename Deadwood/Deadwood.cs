
class DeadwoodGame : IGameInstance {
    private Func<int, int[], bool>[] commands;
    private UIQueue out_queue;
    private int active_player;
    private Player[] players; //2-8 players
    private Board board;
    private Deck deck;


    DeadwoodGame() {
        commands = [
            CMD_Pass,
            CMD_Pass,
            CMD_Pass,
            /* ... whatever */
        ];
    } 


    private bool CMD_Pass(int id, int[] args) {
        return true;
    }

    public bool ProcessCommand(int cmd_id, int[] args) {
        return commands[cmd_id](cmd_id, args);
    }

    public void End()
    {
        throw new NotImplementedException();
    }

    public void Setup(string[] players, UIQueue queue)
    {
        out_queue = queue;
        throw new NotImplementedException();
    }

    // Game logic methods
    private void processMove()
    {
        
    }

    private void processTake()
    {

    }

    private void processUpgrade()
    {

    }

    private void processRehearse()
    {

    }

    private void processAct()
    {

    }

    private void endTurn()
    {

    }

    private void endDay()
    {

    }
}