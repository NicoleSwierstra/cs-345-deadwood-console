class Tile
{
    private Role[] extras;
    public enum type
    {
        LOCATION,
        TRAILER,
        OFFICE,
        CASTING,
        SALOON,
        BANK
    }

    public int[] neighbors;
    public SceneCard active_scene;
    public int shots_remaining;
}