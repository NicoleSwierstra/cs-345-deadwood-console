/*  Nicole Swierstra
 *  Queue class
 *  implements a command queue
 *  
 *  The most lightweight way for the different modules of our game to communicate is by using a
 *  Queue of generic commands. This has the upshot of making both server side networking much 
 *  easier, if we decide to do that, and letting us completely unlink threads without worrying
 *  about communication between them.
 */

using System.Collections;

class CommandQueue {
    private struct Command {
        public int cmd_id;
        public int[] args;

        public Command(int id, int[] argv) {
            args = argv;
            cmd_id = id;
        }
    }

    volatile Queue<Command> commands;

    CommandQueue() {
        commands = new Queue<Command>();
    }

    public void push(int id, int[] args) {
        commands.Enqueue(new Command(id, args));
    }

    /* returns true if the queue is not empty */
    public bool empty() {
        return commands.Count == 0;
    }

    /* gets args of and removes the top item from the queue */
    public int pop(out int[] args) {
        Command next = commands.Dequeue();
        args = next.args;
        return next.cmd_id;
    }

    /* packing methods to pass stuff like player names - 
     * null terminated and fitted to 4 bytes */
    public static int[] packString(string s) {
        int[] r = new int[(s.Length / 2) + 1]; /* strings are utf16 in c# */
        int i;
        for (i = 0; i < s.Length; i++) {
            if (i % 2 == 0) /* i think they're initialized to zero but i guess i don't know lol */
                r[i] = 0;
            char c = s[i];
            r[i / 2] |= c << 16 * (i % 2); 
        }
        /* if for some reason the array isn't zero'd out add null term here */

        return r;
    }

    /* unpacks a string packed with the packString method */
    public static string unpackString(int[] arr) {
        string s = "";
        for (int i = 0; i < arr.Length * 2; i++) {
            char c = (char)(0xFFFF & (arr[i] << 16 * (i % 2)));
            if (c == '\0') break;
            s += c;
        }
        return s;
    }
}