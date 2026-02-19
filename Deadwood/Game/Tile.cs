
namespace Deadwood;
class Tile {
    /* sort of duplicate with the board's tile. probably unneccessary */
    public readonly int location;
    private Role[] extras;
    public int[] neighbors;
    public SceneCard active_scene;
    public int shots_remaining;
    private int total_shots;

    public Tile(int loc, int[] neigh, int shots, Role[] extras) {
        location = loc;
        neighbors = neigh;
        total_shots = shots;
        this.extras = extras;
    }

    public void SetScene(SceneCard newScene) {
        active_scene = newScene;
        shots_remaining = total_shots;
    }

    public override string ToString() {
        string s = "{" + location + ", " + total_shots + ", {" + neighbors[0];
        for(int i = 1; i < neighbors.Length; i++) s += ", " + neighbors[i];
        s += "},\n";
        foreach (Role r in extras) s += "\t" + r + ",\n";

        return s + "}";
    }
}