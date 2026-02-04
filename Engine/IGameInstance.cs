interface IGameInstance {
    void Setup(string[] players, UIQueue queue);
    void End();
    bool ProcessCommand(int cmd_id, int[] args);
}