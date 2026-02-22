/*  Nicole Swierstra
 *  UI interface
 *  
 *  The interface that the UI thread uses. This setup should be able to work with the board
 *  Game when it's either an AWT UI or (hopefully) an interface to opengl drivers. 
 */

public enum UI_Commands {
    /*
     * Sent when the last command sent to the game is invalid. It is up to the UI to interpret what that means.
     * Args:
     *    Maybe there could be a type here in the future but as of right now it has no arguments.
     */
    CMD_FAILURE = 0x20,
    
    /*
     * Sent when the last command sent to the game was executed successfully. It is up to the UI to interpret what that means.
     * 
     * Args:
     *    Maybe there could be a type here in the future but as of right now it has no arguments.
     */
    CMD_SUCCESS,
}

interface IGameUI
{
    /* needs reference back to the application so it can tell it what to do */
    public void Setup(CommandQueue applicationQueue);

    /* needs on update for like animations or something idk */
    public void OnUpdate();

    /* needs to process commands */
    public void ProcessCommand(int cmd_id, int[] args);

    /* can the ui currently process commands */
    public Boolean IsLocked();

    /* like window.should_close in glfw or something */
    public bool ShouldEnd();

    /* ends the process */
    public void End();
}