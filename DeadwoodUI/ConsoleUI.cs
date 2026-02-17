/*  Nicole Swierstra
 *  Console UI class
 *  implements the deadwood console UI
 *  
 *  All of the UI is in just a small handful of classes. Uh this might have been a bad idea tbh.
 */

class DWConsoleUI : IGameUI
{
    CommandQueue applicationQueue;

    public void End()
    {
        Console.WriteLine("Thank you for playing!");
    }

    public void OnUpdate() {
    }

    public void ProcessCommand(int cmd_id, int[] args)
    {
        //throw new NotImplementedException();
    }

    public void Setup(CommandQueue applicationQueue)
    {
        //throw new NotImplementedException();
    }

    public bool ShouldEnd()
    {
        return false;
    }
}