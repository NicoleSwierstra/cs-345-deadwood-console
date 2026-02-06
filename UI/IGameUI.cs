/*  Nicole Swierstra
 *  UI interface
 *  
 *  The interface that the UI thread uses. This setup should be able to work with the board
 *  Game when it's either an AWT UI or (hopefully) an interface to opengl drivers. 
 */

interface IGameUI
{
    /* needs reference back to the application so it can tell it what to do */
    public void Setup(CommandQueue applicationQueue);

    /* needs on update for like animations or something idk */
    public void OnUpdate();

    /* needs to process commands */
    public void ProcessCommand(int cmd_id, int[] args);

    /* like window.should_close in glfw or something */
    public bool ShouldEnd();

    /* ends the process */
    public void End();
}