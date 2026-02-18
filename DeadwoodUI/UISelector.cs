/* Nicole Swierstra
 *
 * Pretty sure I'm cooking with something here but idk what it is yet - maximum of 10 options so it may be nothing lol
 */

class UISelector {
    bool selected;
    int selection;

    string message;
    List<string> options;

    private UISelector(List<string> options, string message, int sel) {
        selection = sel;
        selected = false;
        this.options = options;
        this.message = message;
    }

    public static UISelector fromList(List<string> options, string message) {
        UISelector s = new UISelector(options, message, -1);
        s.render();

        return s;
    }

    public void update(ConsoleKey key) {
        int n = key - ConsoleKey.D0;
        if (n >= 0 && n < options.Count){
            selected = true;
            selection = n;
            return;
        }

        if (key == ConsoleKey.DownArrow) {
            selection++;
            if (selection >= options.Count) selection = 0;
            render();            
        }

        if (key == ConsoleKey.UpArrow) {
            selection--;
            if (selection < 0) selection = options.Count - 1;
            render();
        }

        if (key == ConsoleKey.Enter && selection >= 0){
            selected = true;
        }
    }

    public void render() {
        Console.Clear();
        Console.WriteLine(message);
        
        for (int i = 0; i < options.Count; i++) {
            if (i == selection)
                Console.WriteLine($"\t\x1b[4m[{i}] {options[i]}\x1b[24m");
            else 
                Console.WriteLine($"\t[{i}] {options[i]}");
        }
    }

    public bool has_selected() {
        return selected;
    }

    public int getSelection() {
        return selection;
    }

    public static void Test(string[] args)
    {
        UISelector sel = UISelector.fromList(["hello", "world", "test", "123"], "select an option:"); 

        while(!sel.has_selected()){
            if (Console.KeyAvailable){
                sel.update(Console.ReadKey().Key);
            }
        }

        Console.WriteLine("\x1b[0GYou have selected: " + sel.getSelection());
    }
}