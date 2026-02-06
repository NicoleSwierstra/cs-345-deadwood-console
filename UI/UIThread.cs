/* 
 * Nicole Swierstra
 * UI
 * 
 * This is the code for the UI thread that will display stuff to the user. Note I'm imagining
 * the future AWT/OpenGL version of this, I guess it makes a lot less sense for a console App.
 */

class UIThread {
    volatile private bool keep_running;
    volatile CommandQueue uiQueue, appQueue;
    volatile IGameUI ui;
    Thread ui_thread;

    public UIThread(Type gameui, CommandQueue uiQueue, CommandQueue applicationQueue) {
        /* this seems like kind of a bad idea idk */
        ui = (IGameUI)Activator.CreateInstance(gameui);
        this.uiQueue = uiQueue;
        ui.Setup(applicationQueue);
    }

    public UIThread Start() {
        ui_thread = new Thread(new ThreadStart(Run));
        ui_thread.Start();
        keep_running = true;
        return this; /* for swizzling/chaining whatever */
    }

    public void Stop() {
        keep_running = false;
    }

    public void Join() {
        ui_thread.Join();
    }

    private void Run() {
        while(!ui.ShouldEnd() && keep_running) {
            /* one at a time is probably fine tbh */
            if (!uiQueue.empty()) {
                int id = uiQueue.pop(out int[] args);
                ui.ProcessCommand(id, args);
            }

            ui.OnUpdate();
        }

        ui.End();
        /* TODO: schedule an event to send a force end message to the application */
    }
}