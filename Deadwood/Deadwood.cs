
class DeadwoodGame : IGameInstance {
    private Func<int, int[], bool>[] commands;
    private UIQueue out_queue;

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
}