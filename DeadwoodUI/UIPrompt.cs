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

    public void update(ConsoleKey key, char key_char) {
        if (has_prompted) return;
        
        if (key == ConsoleKey.Enter) {
            has_prompted = true;
            Console.WriteLine();
        } else if (key == ConsoleKey.Spacebar) {
            prompt += " "; /* space isn't considered a key character */
            render();
        } else if (key == ConsoleKey.Backspace) {
            prompt = prompt.Substring(0, Math.Max(prompt.Length - 1, 0));
            render();
        } else if (key_char != 0) {
            prompt += key_char;
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

    public static void Test(string[] args) {
        UIPrompt p = UIPrompt.fromMsg("enter command:"); 

        while(!p.hasPrompt()){
            if (Console.KeyAvailable){
                ConsoleKeyInfo ki = Console.ReadKey();
                p.update(ki.Key, ki.KeyChar);
            }
        }

        Console.WriteLine("\x1b[0GYou have prompted: " + p.getPrompt());
    }
}