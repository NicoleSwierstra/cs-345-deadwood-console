class Player
{
    public string name;
    public int rank;
    public int credits;
    public int dollars;
    public int rehearsal_tokens;
    public int location;
    public int active_role;
    public bool has_moved;

    public Player(string name)
    {
        this.name = name;
        this.rank = 1;
        this.credits = 0;
        this.dollars = 0;
        this.rehearsal_tokens = 0;
        this.location = 0; // Assuming starting location is 0
        this.active_role = -1; // No active role
        this.has_moved = false;
    }

    public bool upgrade(int new_rank, int cost)
    {
        if (new_rank > this.rank && cost <= this.dollars)
        {
            this.rank = new_rank;
            this.dollars -= cost;
            return true;
        }
        return false;
    }

    public bool move(int new_location)
    {
        this.location = new_location;
        this.has_moved = true;
        return true;
    }
}