/*  Nicole Swierstra
 *  Console UI class
 *  implements the deadwood console UI
 *  
 *  All of the UI is in just a small handful of classes. Uh this might have been a bad idea tbh.
 */

class DWConsoleUI : IGameUI {
    enum SelectorType {
        NONE_TYPE = -1,
        MOVE_TYPE = 0,
        TAKE_TYPE = 1,
    }    
    
    CommandQueue applicationQueue;
    ConsoleBoard cb;
    
    SelectorType selectorType;
    UISelector current_selector;
    UIPrompt current_prompt;

    bool should_end;

    public void End() {
        Console.WriteLine("Thank you for playing!");
    }

    public void OnUpdate() {
        if (selectorType == SelectorType.NONE_TYPE) {

            return;
        }
        if (Console.KeyAvailable){
            current_selector.update(Console.ReadKey().Key);
        }
    }

    public void ProcessCommand(int cmd_id, int[] args) {
    
    }

    public void Setup(CommandQueue applicationQueue) {
        cb = ConsoleBoard.fromXML("res/gamedata/board.xml");
        selectorType = SelectorType.NONE_TYPE;
    }

    public bool ShouldEnd()
    {
        return should_end;
    }
}