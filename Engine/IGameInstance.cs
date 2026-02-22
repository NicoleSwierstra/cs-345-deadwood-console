/*  Nicole Swierstra
 *
 *  Game instance - Generic interface between the application and the game backend. Any class that
 *  implements the Game Instance interface can technically be run by the program, as long as the
 *  UI that is accessed via the UI Queue passed to the game instance knows what happens with the
 *  game.
 */

public enum GameComRet {
    RET_ERROR = -1, RET_SUCCESS, RET_ENDED
}

interface IGameInstance {
    void Setup(string[] players, CommandQueue ui_queue);
    void End();
    GameComRet ProcessCommand(int cmd_id, int[] args);
}