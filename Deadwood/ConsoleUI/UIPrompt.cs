/* Nicole Swierstra
 *
 * Not sure I'm cooking anything tbh 
 */

class UIPrompt {
    string message;

    string prompt;
    bool has_prompted;

    private UIPrompt(string message, string prompt) {
        this.prompt = prompt;
        this.message = message;
        this.has_prompted = false;
    }

    public static UIPrompt fromMsg(string message) {
        UIPrompt p = new UIPrompt(message, "");
        p.render();
        return p;
    }

    public void update(ConsoleKeyInfo key_press) {
        if (has_prompted) return;
        
        if (key_press.Key == ConsoleKey.Enter) {
            has_prompted = true;
            Console.WriteLine();
        } else if (key_press.Key == ConsoleKey.Spacebar) {
            prompt += " "; /* space isn't considered a key character */
            render();
        } else if (key_press.Key == ConsoleKey.Backspace) {
            prompt = prompt.Substring(0, Math.Max(prompt.Length - 1, 0));
            render();
        } else if (key_press.KeyChar != 0) {
            prompt += key_press.KeyChar;
            render();
        } 
    }

    public void render() {
        Console.Clear();
        Console.WriteLine(message);
        Console.Write("> " + prompt + "\x1b[0K");
    }

    public bool hasPrompt() {
        return has_prompted;
    }
    
    public string getPrompt() {
        return prompt;
    }

    public void Clear() {
        prompt = "";
        has_prompted = false;
    }

    public static void Test(string[] args) {
        UIPrompt p = UIPrompt.fromMsg("enter command:"); 

        while(!p.hasPrompt()){
            if (Console.KeyAvailable){
                ConsoleKeyInfo ki = Console.ReadKey(true);
                p.update(ki);
            }
        }

        Console.WriteLine("\x1b[0GYou have prompted: " + p.getPrompt());
    }
}